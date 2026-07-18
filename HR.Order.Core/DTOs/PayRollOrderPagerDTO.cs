using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class PayRollOrderPagerDTO : BaseDTO
    {
        public int currentPage { set; get; }
        public int pageSize { set; get; }
        public int year { set; get; }
        public int month { set; get; }
        public string? filter { set; get; }
        public string? activeSortColumn { set; get; }
        public string? Sortdirection { set; get; }
        public long? CostCenterId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public long? OrderTypeId { get; set; }
        public long currentUserId { get; set; }

    }
}
