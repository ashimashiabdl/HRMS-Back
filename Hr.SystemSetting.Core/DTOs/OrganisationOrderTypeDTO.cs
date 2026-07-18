using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationOrderTypeDTO : BaseDTO
    {
        public long OrderTypeGroupId { get; set; }
        /// <summary>
        /// حکم استخدامی هست یا حقوقی
        /// </summary>
        public long OrderTypeId { get; set; }
        /// <summary>
        /// نوع حکم به لحاظ خاتمه دهنده یا آغاز کننده بودن
        /// </summary>
        public long OrderDirectionTypeId { get; set; }
        public bool IsBatch { get; set; }
        public bool IsPrintable { get; set; }
        public string? OrderTypeGroupTitle { get; set; }
        public string? OrderTypeTitle { get; set; }
        public string? OrderDirectionTypeTitle { get; set; }
        [StringLength(128)]
        public string? Code { get; set; }
    }
}
