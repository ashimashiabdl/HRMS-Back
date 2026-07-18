using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class GetOrderListByEmployeeIdRequest  : BaseOrderRequest
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public long CurrentUserId { get; set; }
    }
}
