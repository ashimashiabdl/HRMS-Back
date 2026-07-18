using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Attribute
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ControllerGroup(string EnglishKey,string PersioanKey) : System.Attribute
    {
        public string EnglishKey { get; } = EnglishKey;
        public string PersioanKey { get; } = PersioanKey;
    }

}
