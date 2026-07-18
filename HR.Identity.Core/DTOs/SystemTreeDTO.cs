using HR.Identity.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{

    public class SystemTreeDTOMINI {

        public long Id { get; set; }
        public string? HtmlTitle { get; set; }
        public string? icon { get; set; }

        public string? CardTitle { get; set; }

        public string? CardSubHeader { get; set; }

        public string? CardDescription { get; set; }
        public int MenuOrder { get; set; }

        public string? Route { get; set; }

    }
    public class SystemTreeDTO : BaseDTO
    {
        
        public long NodeTypeId { get; set; }
        public string? NodeType { get; set; }
        [StringLength(128)]
        public string? HtmlTitle { get; set; }
        [StringLength(256)]
        public string? CardTitle { get; set; }
        [StringLength(256)]
        public string? CardSubHeader { get; set; }
        [StringLength(256)]
        public string? CardDescription { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? ParentId { get; set; }
        public bool HideRelatedLink { get; set; }
        public string? Parent { get; set; }
        [StringLength(64)]
        public string? Icon { get; set; }
        public bool IsManagedByAdmins { get; set; }
        public int MenuOrder { get; set; }
        [StringLength(256)]
        public string? Route { get; set; }
    }
}
