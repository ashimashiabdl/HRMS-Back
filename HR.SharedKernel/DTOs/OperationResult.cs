using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.DTOs
{
    public class OperationResult
    {
        public OperationResult(bool success, string messag, dynamic payload,int? rowcount)
        {
            this.Success = success;
            this.Message = messag;
            this.Payload = payload;
            this.rowcount = rowcount;
        }
        public bool Success { get; set; }
        public string Message { get; set; }

        public dynamic Payload { get; set; }
        public int? rowcount { get; set; }



        public static OperationResult Succeeded(string msg = "عملیات با موفقیت انجام شد", dynamic payload = null,int? rowCount = null)
        {
            return new OperationResult(true, msg, payload, rowCount);
        }
        public static OperationResult NotFound(string msg = "رکوردی یافت نشد", dynamic payload = null)
        {
            return new OperationResult(false, msg, payload,0);
        }
        public static OperationResult Failed(string msg = "عملیات ناموفق بود", dynamic payload = null)
        {
            return new OperationResult(false, msg, payload,0);
        }

        public static OperationResult Existed(string msg = "رکورد مورد نظر موجود می باشد", dynamic payload = null)
        {
            return new OperationResult(false, msg, payload, 0);
        }

        // Add More Results .......

    }
}
