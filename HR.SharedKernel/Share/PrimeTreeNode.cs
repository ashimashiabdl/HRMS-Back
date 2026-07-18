using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Share
{
    public class PrimeTreeNode<T>
    {
        //[JsonIgnore]
        public long? Id { get; set; }
        public long? pid { get; set; }
        public string? name { get; set; }
        public string? title { get; set; }
        public string? OrganizationType { get; set; }
        public string? Place { get; set; }
        public bool? isPayLocation { get; set; }
        public string? img
        {
            get
            {
                return Id.ToString();
            }
        }
        //public string? icon { get; set; }
        //public string? expandedIcon { get; set; }
        //public string? collapsedIcon { get; set; }
        //public string? type { get; set; }
        //public string? key { get; set; }
        //public T? data { get; set; }
        public bool? leaf { get; set; }
        //public bool? expanded { get; set; }
        //public bool? partialSelected { get; set; }
        //public bool? draggable { get; set; }
        //public bool? droppable { get; set; }
        //public bool? selectable { get; set; }
        //public List<PrimeTreeNode<T>>? children { get; set; }
        //public PrimeTreeNode<T>? parent { get; set; }
    }
}
