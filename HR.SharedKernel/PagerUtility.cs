using HR.SharedKernel.DTOs;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using HR.SharedKernel.Extensions;
using NLog;

namespace HR.SharedKernel
{
    public static class PagerUtility<T> where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static IQueryable<T> GetPagedDataLog(IQueryable<T> data, out int rowcount, int currentPage = 0, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "")
        {
            var methodName = $"{nameof(PagerUtility<T>)}.{nameof(GetPagedDataLog)}";
            rowcount = 0;

            try
            {
                // Input validation
                if (data == null)
                {
                    Logger.Warn($"{methodName} - Input data is null. Returning empty result.");
                    return Enumerable.Empty<T>().AsQueryable();
                }

                // Validate and sanitize pagination parameters
                if (currentPage < 0)
                {
                    Logger.Warn($"{methodName} - Invalid currentPage: {currentPage}. Setting to 0.");
                    currentPage = 0;
                }

                if (pageSize <= 0)
                {
                    Logger.Warn($"{methodName} - Invalid pageSize: {pageSize}. Setting to 10.");
                    pageSize = 10;
                }

                if (pageSize > 1000)
                {
                    Logger.Warn($"{methodName} - PageSize {pageSize} exceeds maximum (1000). Setting to 1000.");
                    pageSize = 1000;
                }

                var startIndex = pageSize * currentPage;

                Logger.Info($"{methodName} - Processing pagination for type {typeof(T).Name}: Page={currentPage}, PageSize={pageSize}, Filter='{filter}', SortColumn='{activeSortColumn}', SortDirection='{Sortdirection}'");

                // Apply sorting
                try
                {
                    if (string.IsNullOrWhiteSpace(activeSortColumn))
                    {
                        activeSortColumn = "CreatedOn";
                        Sortdirection = "desc";
                        Logger.Debug($"{methodName} - No sort column specified. Using default: CreatedOn desc");
                    }

                    // Validate that the sort column exists
                    var sortProperty = typeof(T).GetProperty(activeSortColumn, 
                        System.Reflection.BindingFlags.IgnoreCase | 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Instance);

                    if (sortProperty == null)
                    {
                        Logger.Warn($"{methodName} - Sort column '{activeSortColumn}' not found on type {typeof(T).Name}. Skipping sorting.");
                    }
                    else
                    {
                        bool isDescending = !string.Equals(Sortdirection, "asc", StringComparison.OrdinalIgnoreCase);
                        data = data.OrderBy(activeSortColumn, isDescending);
                        Logger.Debug($"{methodName} - Applied sorting: {activeSortColumn} {(isDescending ? "DESC" : "ASC")}");
                    }
                }
                catch (Exception sortEx)
                {
                    Logger.Error(sortEx, $"{methodName} - Error applying sort on column '{activeSortColumn}'. Continuing without sorting.");
                }

                // Apply filtering (without Include for Log entities)
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    try
                    {
                        Logger.Debug($"{methodName} - Applying filter: '{filter}'");

                        // Note: GetPagedDataLog does not include navigation properties
                        // This is intentional for Log entities to avoid circular references
                        var filtered = data.AsEnumerable().Filter(filter);
                        data = filtered.AsQueryable();

                        Logger.Debug($"{methodName} - Filter applied successfully");
                    }
                    catch (Exception filterEx)
                    {
                        Logger.Error(filterEx, $"{methodName} - Error applying filter '{filter}'. Returning unfiltered data.");
                    }
                }

                // Calculate total count before pagination
                try
                {
                    rowcount = data.Count();
                    Logger.Debug($"{methodName} - Total rows after filtering: {rowcount}");
                }
                catch (Exception countEx)
                {
                    Logger.Error(countEx, $"{methodName} - Error calculating row count. Setting to 0.");
                    rowcount = 0;
                }

                // Apply pagination
                IQueryable<T> pagedData;
                try
                {
                    pagedData = data
                        .Skip(startIndex)
                        .Take(pageSize);

                    Logger.Info($"{methodName} - Pagination completed successfully. Returning page {currentPage} with {pageSize} items (Total: {rowcount})");
                }
                catch (Exception pageEx)
                {
                    Logger.Error(pageEx, $"{methodName} - Error applying pagination (Skip: {startIndex}, Take: {pageSize}). Returning empty result.");
                    return Enumerable.Empty<T>().AsQueryable();
                }

                return pagedData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{methodName} - Unexpected error during pagination. Type: {typeof(T).Name}, Page: {currentPage}, PageSize: {pageSize}, Filter: '{filter}', SortColumn: '{activeSortColumn}'");
                rowcount = 0;
                throw new Exception($"Error in {methodName} for type {typeof(T).Name}: {ex.Message}", ex);
            }
        }


        public static IQueryable<T> GetPagedData(IQueryable<T> data, out int rowcount, int currentPage = 0, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "")
        {
            var methodName = $"{nameof(PagerUtility<T>)}.{nameof(GetPagedData)}";
            rowcount = 0;

            try
            {
                // Input validation
                if (data == null)
                {
                    Logger.Warn($"{methodName} - Input data is null. Returning empty result.");
                    return Enumerable.Empty<T>().AsQueryable();
                }

                // Validate and sanitize pagination parameters
                if (currentPage < 0)
                {
                    Logger.Warn($"{methodName} - Invalid currentPage: {currentPage}. Setting to 0.");
                    currentPage = 0;
                }

                if (pageSize <= 0)
                {
                    Logger.Warn($"{methodName} - Invalid pageSize: {pageSize}. Setting to 10.");
                    pageSize = 10;
                }

                if (pageSize > 1000)
                {
                    Logger.Warn($"{methodName} - PageSize {pageSize} exceeds maximum (1000). Setting to 1000.");
                    pageSize = 1000;
                }

                var startIndex = pageSize * currentPage;

                Logger.Info($"{methodName} - Processing pagination for type {typeof(T).Name}: Page={currentPage}, PageSize={pageSize}, Filter='{filter}', SortColumn='{activeSortColumn}', SortDirection='{Sortdirection}'");

                // Apply sorting
                try
                {
                    if (string.IsNullOrWhiteSpace(activeSortColumn))
                    {
                        activeSortColumn = "CreateDate";
                        Sortdirection = "desc";
                        Logger.Debug($"{methodName} - No sort column specified. Using default: CreateDate desc");
                    }

                    // Validate that the sort column exists
                    var sortProperty = typeof(T).GetProperty(activeSortColumn, 
                        System.Reflection.BindingFlags.IgnoreCase | 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Instance);

                    if (sortProperty == null)
                    {
                        Logger.Warn($"{methodName} - Sort column '{activeSortColumn}' not found on type {typeof(T).Name}. Skipping sorting.");
                    }
                    else
                    {
                        bool isDescending = !string.Equals(Sortdirection, "asc", StringComparison.OrdinalIgnoreCase);
                        data = data.OrderBy(activeSortColumn, isDescending);
                        Logger.Debug($"{methodName} - Applied sorting: {activeSortColumn} {(isDescending ? "DESC" : "ASC")}");
                    }
                }
                catch (Exception sortEx)
                {
                    Logger.Error(sortEx, $"{methodName} - Error applying sort on column '{activeSortColumn}'. Continuing without sorting.");
                }

                // Apply filtering
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    try
                    {
                        Logger.Debug($"{methodName} - Applying filter: '{filter}'");

                        // NOTE:
                        // فیلتر فعلی روی IEnumerable اعمال می‌شود؛
                        // برای جلوگیری از چندبار enumerate شدن، ابتدا بعد از فیلتر ToList می‌کنیم
                        // تا فقط یک بار داده از دیتابیس خوانده شود و سپس روی لیست صفحه‌بندی انجام شود.

                        // Include related entities for filtering
                        var properties = typeof(T).GetProperties();
                        var includedProperties = new List<string>();

                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.BaseType == typeof(T).BaseType)
                                {
                                    data = data.Include(property.Name);
                                    includedProperties.Add(property.Name);
                                }
                            }
                            catch (Exception includeEx)
                            {
                                Logger.Warn(includeEx, $"{methodName} - Failed to include navigation property '{property.Name}'. Continuing without it.");
                            }
                        }

                        if (includedProperties.Any())
                        {
                            Logger.Debug($"{methodName} - Included navigation properties: {string.Join(", ", includedProperties)}");
                        }

                        // Apply filter using extension method (in-memory)
                        var filteredEnumerable = data.AsEnumerable().Filter(filter);
                        var filteredList = filteredEnumerable.ToList();

                        // از این‌جا به بعد روی لیست کار می‌کنیم تا فقط یک بار enumerate شود
                        data = filteredList.AsQueryable();

                        Logger.Debug($"{methodName} - Filter applied successfully");
                    }
                    catch (Exception filterEx)
                    {
                        Logger.Error(filterEx, $"{methodName} - Error applying filter '{filter}'. Returning unfiltered data.");
                    }
                }

                // Calculate total count before pagination
                try
                {
                    rowcount = data.Count();
                    Logger.Debug($"{methodName} - Total rows after filtering: {rowcount}");
                }
                catch (Exception countEx)
                {
                    Logger.Error(countEx, $"{methodName} - Error calculating row count. Setting to 0.");
                    rowcount = 0;
                }

                // Apply pagination
                IQueryable<T> pagedData;
                try
                {
                    pagedData = data
                        .Skip(startIndex)
                        .Take(pageSize);

                    Logger.Info($"{methodName} - Pagination completed successfully. Returning page {currentPage} with {pageSize} items (Total: {rowcount})");
                }
                catch (Exception pageEx)
                {
                    Logger.Error(pageEx, $"{methodName} - Error applying pagination (Skip: {startIndex}, Take: {pageSize}). Returning empty result.");
                    return Enumerable.Empty<T>().AsQueryable();
                }

                return pagedData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{methodName} - Unexpected error during pagination. Type: {typeof(T).Name}, Page: {currentPage}, PageSize: {pageSize}, Filter: '{filter}', SortColumn: '{activeSortColumn}'");
                rowcount = 0;
                throw new Exception($"Error in {methodName} for type {typeof(T).Name}: {ex.Message}", ex);
            }
        }
    }
}
