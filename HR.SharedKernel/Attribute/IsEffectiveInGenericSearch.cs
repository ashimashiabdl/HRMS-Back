using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Attribute
{
    public class IsEffectiveInGenericSearch : ValidationAttribute
    {
        public bool IsEffective { get; set; }
        public IsEffectiveInGenericSearch()
        {
            this.IsEffective = false;
        }
        public override bool IsValid(object? value)
        {
            return true;
        }
    }
}
