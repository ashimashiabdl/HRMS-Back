using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class UserSignatureDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public long AspNetUsersId { get; set; }
        public long tempFileId { get; set; }
        public string? AspNetUsers { get; set; }
        public long SignatureImageId { get; set; }
        public string? SignatureImage { get; set; }
        public bool Enabled { get; set; }
        public string? SignDescription { get; set; }
        public string? SignTitle { get; set; }
        public string? SignImage { get; set; }
        public string? SignBase64Image { get; set; }
    }
}
