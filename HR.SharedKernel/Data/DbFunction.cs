using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    public static class DbFunction
    {
        public static long CallDBFunction(string ConnectionString, string schema, string FunctionName, SqlParameter[] paramss)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = schema + "." + FunctionName;
                command.Parameters.AddRange(paramss);
            

                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.BigInt);
                returnValue.Direction = ParameterDirection.ReturnValue;

               connection.Open();
                command.ExecuteNonQuery();

                return Convert.ToInt64(returnValue.Value);

            }
        }
    }
}
