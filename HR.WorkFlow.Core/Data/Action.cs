using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.Data
{
    [Table("Action", Schema = "wf")]
    public class Action : BaseEntity
    {
        public bool? AllowComment { get; set; }

        public bool? CommentIsMandatory { get; set; }

        public bool IsDefualt { get; set; }
    }
}
