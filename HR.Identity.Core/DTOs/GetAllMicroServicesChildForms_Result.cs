using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class GetAllMicroServicesChildForms_Result
    {
        public long Id { get; set; }
        public string title { get; set; }
        public Nullable<int> TreeNodeId { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Name { get; set; }
    }
}
