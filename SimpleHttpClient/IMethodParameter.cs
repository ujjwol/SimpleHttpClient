using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpClient
{
    public interface IMethodParameter : IEquatable<IMethodParameter>
    {
        string Name { get; }

        HttpContent AsHttpContent();
    }
}
