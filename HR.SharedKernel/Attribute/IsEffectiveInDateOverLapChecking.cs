using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Attribute
{
    public class IsEffectiveInDateOverLapChecking : ValidationAttribute
    {
        public bool IsEffective { get; set; }
        public IsEffectiveInDateOverLapChecking()
        {
            this.IsEffective = false;
        }
        public override bool IsValid(object? value)
        {
            return true;
        }
    }
}
