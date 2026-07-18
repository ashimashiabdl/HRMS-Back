using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HR.SharedKernel.Security;

namespace HR.SharedKernel.Dapper
{
    public class Dapper : IDapper
    {
        private readonly IConfiguration _config;
        private string Connectionstring = "HRMSConnection";

        public Dapper(IConfiguration config)
        {
            _config = config;
        }
        public void Dispose()
        {

        }

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            var raw = _config.GetConnectionString(Connectionstring);
            var cs = ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            using IDbConnection db = new SqlConnection(cs);
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            var raw = _config.GetConnectionString(Connectionstring);
            var cs = ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            using IDbConnection db = new SqlConnection(cs);
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }

        public DbConnection GetDbconnection()
        {
            var raw = _config.GetConnectionString(Connectionstring);
            var cs = ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            return new SqlConnection(cs);
        }

        public T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            var raw = _config.GetConnectionString(Connectionstring);
            var cs = ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            using IDbConnection db = new SqlConnection(cs);
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            var raw = _config.GetConnectionString(Connectionstring);
            var cs = ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            using IDbConnection db = new SqlConnection(cs);
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }
    }
}
