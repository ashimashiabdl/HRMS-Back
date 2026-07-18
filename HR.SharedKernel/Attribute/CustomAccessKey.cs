using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Attribute;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CustomAccessKey(string AccessKey) : System.Attribute
{
    public string AccessKey { get; } = AccessKey;
}
