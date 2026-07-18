using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class ActionDTO : BaseDTO
    {
        public bool? AllowComment { get; set; }

        public bool? CommentIsMandatory { get; set; }

        public bool IsDefualt { get; set; }
    }
}
