using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace SimpleHttpClient
{
    public class SimpleHttpBase
    {
        private readonly object _disposeLock = new object();
        private bool _disposed;

        private readonly HttpClient _client;

        public SimpleHttpBase()
        {
            this._client = new HttpClient();
        }

        /// <summary>
        /// Get or Post only using ApiMethod which contains MethodParameterSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(ApiMethod method)
        {
            return GetAsync<T>(method, CancellationToken.None);
        }

        /// <summary>
        /// Simple Get with string url
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(string url)
        {
            var method = new ApiMethod(url, HttpMethod.Get);
            return GetAsync<T>(method, CancellationToken.None);
        }

        /// <summary>
        /// Base method to do both Get or Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(ApiMethod method, CancellationToken cancellationToken, IEnumerable<JsonConverter> converters = null)
        {
            if (_disposed)
                throw new ObjectDisposedException("SimpleHttpClient");

            if (method == null)
                throw new ArgumentNullException("method");

            // Build api call url
            var apiRequestUrl = new StringBuilder(method.Url);
            if (method.HttpMethod == HttpMethod.Get && method.Parameters.Count > 0)
            {
                // Add the request parameters to the query string since we're in HTTP GET
                apiRequestUrl.Append("?");
                apiRequestUrl.Append(method.Parameters.ToFormUrlEncoded());
            }

            var apiUrl = apiRequestUrl.ToString();

            using (var request = new HttpRequestMessage(method.HttpMethod, apiUrl))
            {
                var content =
                    new FormUrlEncodedContent(
                        method.Parameters.Select(
                            p => new KeyValuePair<string, string>(p.Name, ((StringMethodParameter)p).Value)));

                using (var response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using (var reader = new JsonTextReader(new StreamReader(stream)))
                    {
                        var serializer = CreateSerializer(converters);
                        if (response.IsSuccessStatusCode)
                        {
                            return serializer.Deserialize<T>(reader);
                        }
                        else
                        {
                            throw new Exception(String.Format("Success Status Code was not recieved. Status Code {0} with ReasonPhrase {1} returned", response.StatusCode, response.ReasonPhrase));
                        }
                    }
                }
            }

        }




        /// <summary>
        /// Return the json.net json serializer
        /// </summary>
        /// <param name="converters"></param>
        /// <returns></returns>
        private JsonSerializer CreateSerializer(IEnumerable<JsonConverter> converters)
        {
            var serializer = new JsonSerializer();
            if (converters != null)
            {
                foreach (JsonConverter converter in converters)
                    serializer.Converters.Add(converter);
            }

            return serializer;
        }




        #region IDisposable Implementation
        /// <summary>
        /// Dipose Pattern to free up unmanaged resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_disposeLock)
                {
                    _disposed = true;
                    _client.Dispose();
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        #endregion
    }
}
