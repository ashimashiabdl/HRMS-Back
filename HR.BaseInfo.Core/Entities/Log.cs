using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities
{
    public class Log
    {
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public int Id { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public System.DateTime CreatedOn { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Level { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Message { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string StackTrace { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Exception { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Logger { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Url { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string IP { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string User { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public bool? Success { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public int? StatusCode { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string Method { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public string UserAgent { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public int? Port { get; set; }
        [IsEffectiveInGenericSearch(IsEffective =true)]
        public int? DurationMs { get; set; }
    }
}
