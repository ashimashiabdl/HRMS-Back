using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace HR.SharedKernel.Data
{
    //Generic UnitOfWork Class. 
    //While Creating an Instance of the UnitOfWork object, we need to specify the actual type for the TContext Generic Type
    //In our example, TContext is going to be EmployeeDBContext
    //new() constraint will make sure that this type is going to be a non-abstract type with a parameterless constructor
    
        public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : class, IAppDbContext

    {
        private bool _disposed;
        private string _errorMessage = string.Empty;

        //The following Object is going to hold the Transaction Object
        private IDbContextTransaction _objTran;
        private readonly UserResolverService _userService;
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        
        //Using the Constructor we are initializing the Context Property which is declared in the IUnitOfWork Interface
        //This is nothing but we are storing the DBContext (EmployeeDBContext) object in Context Property
        public UnitOfWork(TContext ctx, UserResolverService UserService, ILogger<UnitOfWork<TContext>> logger)
        {
            _userService = UserService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Context = ctx;
            
            _logger.LogDebug("UnitOfWork initialized for context type: {ContextType}", typeof(TContext).Name);
        }

        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //The Context property will return the DBContext object i.e. (EmployeeDBContext) object
        //This Property is declared inside the Parent Interface and Initialized through the Constructor
        public TContext Context { get; }

        //The CreateTransaction() method will create a database Transaction so that we can do database operations
        //by applying do everything and do nothing principle
        public void CreateTransaction()
        {
            try
            {
                //It will Begin the transaction on the underlying store connection
                _objTran = Context.Database.BeginTransaction();
                _logger.LogInformation("Database transaction started for context: {ContextType}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد تراکنش دیتابیس برای context: {ContextType}", typeof(TContext).Name);
                throw;
            }
        }

        //If all the Transactions are completed successfully then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            try
            {
                if (_objTran == null)
                {
                    _logger.LogWarning("تلاش برای Commit تراکنش در حالی که تراکنشی ایجاد نشده است");
                    throw new InvalidOperationException("تراکنشی برای Commit وجود ندارد. ابتدا باید CreateTransaction فراخوانی شود.");
                }

                //Commits the underlying store transaction
                _objTran.Commit();
                _logger.LogInformation("تراکنش دیتابیس با موفقیت Commit شد برای context: {ContextType}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در Commit تراکنش دیتابیس برای context: {ContextType}", typeof(TContext).Name);
                throw;
            }
            finally
            {
                if (_objTran != null)
                {
                    _objTran.Dispose();
                    _objTran = null;
                }
            }
        }

        //If at least one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            try
            {
                if (_objTran == null)
                {
                    _logger.LogWarning("تلاش برای Rollback تراکنش در حالی که تراکنشی ایجاد نشده است");
                    return; // No transaction to rollback, just return
                }

                //Rolls back the underlying store transaction
                _objTran.Rollback();
                _logger.LogInformation("تراکنش دیتابیس با موفقیت Rollback شد برای context: {ContextType}", typeof(TContext).Name);
                
                //The Dispose Method will clean up this transaction object and ensures Entity Framework
                //is no longer using that transaction.
                _objTran.Dispose();
                _objTran = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در Rollback تراکنش دیتابیس برای context: {ContextType}", typeof(TContext).Name);
                // Try to dispose even if rollback failed
                try
                {
                    _objTran?.Dispose();
                    _objTran = null;
                }
                catch (Exception disposeEx)
                {
                    _logger.LogError(disposeEx, "خطا در Dispose تراکنش پس از Rollback ناموفق");
                }
                throw;
            }
        }

        //The Save() Method Implement DbContext Class SaveChanges method 
        //So whenever we do a transaction we need to call this Save() method 
        //so that it will make the changes in the database permanently
        public async Task<int> Save()
        {
            try
            {
                _logger.LogDebug("شروع ذخیره تغییرات برای context: {ContextType}", typeof(TContext).Name);
                
                //Calling DbContext Class SaveChangesAsync method 
                var result = await Context.SaveChangesAsync();
                
                _logger.LogInformation("تغییرات با موفقیت ذخیره شد. تعداد رکوردهای تأثیرپذیرفته: {AffectedRows} برای context: {ContextType}", 
                    result, typeof(TContext).Name);
                
                return result;
            }
            catch (DbEntityValidationException dbEx)
            {
                _errorMessage = string.Empty;
                var validationErrorsList = new List<string>();
                
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var errorMsg = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                        _errorMessage = _errorMessage + errorMsg + Environment.NewLine;
                        validationErrorsList.Add(errorMsg);
                    }
                }
                
                _logger.LogError(dbEx, 
                    "خطای اعتبارسنجی در ذخیره تغییرات برای context: {ContextType}. خطاها: {ValidationErrors}", 
                    typeof(TContext).Name, string.Join(" | ", validationErrorsList));
                
                throw new Exception(_errorMessage, dbEx);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException)
            {
                _logger.LogError(ex, 
                    "خطای به‌روزرسانی دیتابیس برای context: {ContextType}. SQL Error Number: {SqlErrorNumber}, Message: {SqlMessage}", 
                    typeof(TContext).Name, sqlException.Number, sqlException.Message);
                
                switch (sqlException.Number)
                {
                    case 2627: // Unique constraint error
                        _logger.LogWarning("خطای محدودیت یکتایی (2627) برای context: {ContextType}", typeof(TContext).Name);
                        throw new Exception("امکان ثبت رکورد تکراری وجود ندارد", sqlException);
                    case 2601:  // Duplicated key row error
                        _logger.LogWarning("خطای کلید تکراری (2601) برای context: {ContextType}", typeof(TContext).Name);
                        throw new Exception("امکان ثبت رکورد تکراری وجود ندارد", sqlException);
                    case 547:   // Constraint check violation
                        _logger.LogWarning("خطای نقض محدودیت (547) برای context: {ContextType}", typeof(TContext).Name);
                        throw new Exception("نقض محدودیت دیتابیس رخ داده است", sqlException);
                    default:
                        _logger.LogError(ex, 
                            "خطای SQL ناشناخته (Number: {SqlErrorNumber}) برای context: {ContextType}", 
                            sqlException.Number, typeof(TContext).Name);
                        throw;
                }
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                _logger.LogError(concurrencyEx, 
                    "خطای همزمانی در به‌روزرسانی دیتابیس برای context: {ContextType}", 
                    typeof(TContext).Name);
                throw new Exception("خطای همزمانی: رکورد توسط کاربر دیگری تغییر یافته است. لطفاً صفحه را رفرش کنید و دوباره تلاش کنید.", concurrencyEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "خطای غیرمنتظره در ذخیره تغییرات برای context: {ContextType}. Message: {Message}, StackTrace: {StackTrace}", 
                    typeof(TContext).Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        //Disposing of the Context Object
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        // Roll back any transaction that was started but never committed/rolled back
                        if (_objTran != null)
                        {
                            try
                            {
                                _objTran.Rollback();
                                _logger.LogWarning("تراکنش باز مانده در Dispose با Rollback بسته شد برای context: {ContextType}", typeof(TContext).Name);
                            }
                            catch (InvalidOperationException rollbackEx)
                            {
                                // Transaction may already be completed (e.g. committed without clearing _objTran in older code paths)
                                _logger.LogDebug(rollbackEx,
                                    "تراکنش قبلاً بسته شده بود در Dispose برای context: {ContextType}",
                                    typeof(TContext).Name);
                            }
                            catch (Exception rollbackEx)
                            {
                                _logger.LogError(rollbackEx, "خطا در Rollback تراکنش در Dispose برای context: {ContextType}", typeof(TContext).Name);
                            }
                            finally
                            {
                                try
                                {
                                    _objTran.Dispose();
                                }
                                catch (Exception disposeEx)
                                {
                                    _logger.LogError(disposeEx, "خطا در Dispose تراکنش برای context: {ContextType}", typeof(TContext).Name);
                                }
                                _objTran = null;
                            }
                        }

                        // DbContext lifetime is managed by DI (scoped); do not dispose injected Context here.
                        _logger.LogDebug("UnitOfWork disposed برای context: {ContextType}", typeof(TContext).Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در Dispose UnitOfWork برای context: {ContextType}", typeof(TContext).Name);
                    }
                }
                _disposed = true;
            }
        }
    }
}