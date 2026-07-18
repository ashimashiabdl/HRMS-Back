
using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Data.SqlClient;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class TempPersonnelFunctionservice : BaseService<TempPersonnelFunction, PayrollContext, TempPersonnelFunctionDTO>, IScopedServices
    {
        public TempPersonnelFunctionservice(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        /// <summary>
        /// انتقال اکسل از جدول موقت به جدول اصلی کارکرد
        /// </summary>
        public int TransferExcelTempFunctions_toMain(long PersonnelFunctionExcelFileId) 
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[Payroll].[Transfer_ExcelTemp_Functions_to_Main]", con);
                cmd.Parameters.AddWithValue("@PersonnelFunctionExcelFileId", PersonnelFunctionExcelFileId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    int totalAffected = 0;
                    if (rdr.Read())
                    {
                        // SP now returns detailed counts for both Functions and Leaves
                        // We use TotalRecordsAffected which is the sum of both
                        totalAffected = rdr["TotalRecordsAffected"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["TotalRecordsAffected"]);
                        
                        // Optional: Log individual counts for debugging
                        // int functionInserted = rdr["FunctionInsertedCount"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["FunctionInsertedCount"]);
                        // int leaveInserted = rdr["LeaveInsertedCount"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["LeaveInsertedCount"]);
                    }
                    con.Close();
                    return totalAffected;
                }
            }
        }

        public bool Validate(TempPersonnelFunction entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
