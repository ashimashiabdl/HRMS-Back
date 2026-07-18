using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class PlacesDTO : BaseDTO
    {
        public long? ParentPlaceId { get; set; }
        public  string? ParentPlace { get; set; }
        public long PlaceTypeId { get; set; }
        public  string? PlaceType { get; set; }
        [StringLength(3)]
        public string? TaxCode { get; set; }
        [StringLength(450)]
        public string? Description { get; set; }
    }
}
