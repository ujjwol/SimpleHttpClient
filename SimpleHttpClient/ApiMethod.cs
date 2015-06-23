using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpClient
{
    public class ApiMethod
    {
        private readonly string _url;
        private readonly HttpMethod _httpMethod;
        private readonly MethodParameterSet _parameters;

        public ApiMethod(string url, HttpMethod httpMethod, MethodParameterSet parameters = null)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (url.Length == 0)
                throw new ArgumentException("Method URL cannot be empty.", "url");

            if (httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Post)
                throw new ArgumentException("The http method must be either GET or POST.", "httpMethod");

            this._url = url;
            this._httpMethod = httpMethod;
            this._parameters = parameters ?? new MethodParameterSet();
        }

        public string Url { get { return _url; } }

        public HttpMethod HttpMethod { get { return _httpMethod; } }

        public MethodParameterSet Parameters { get { return _parameters; } }
    }
}
