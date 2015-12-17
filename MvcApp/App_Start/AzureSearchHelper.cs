using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MvcApp.App_Start
{
    public class AzureSearchHelper
    {
        #region Properties & Fields

        private readonly string mApiKey = "";
        private readonly string mStorageConnectionString="";
        private readonly string mSearchServiceName ="";

        private HttpClient mClient = new HttpClient();

        #endregion

        #region .ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchHelper"/> class.
        /// </summary>
        public AzureSearchHelper()
        {
            mApiKey = ConfigurationManager.AppSettings["Azure.Search.ApiKey"];
            mStorageConnectionString = ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"];
            mSearchServiceName = ConfigurationManager.AppSettings["Azure.Search.ServiceName"];
        }

        #endregion

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        #region Create Data Source

        /// <summary>
        /// Creates the data source.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> CreateDataSource(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/datasources?api-version=2015-02-28-Preview", mSearchServiceName));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            }
            //mClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("api-key", mApiKey);

            HttpResponseMessage response = await mClient.SendAsync(request);
            //dynamic body = await response.Content.ReadAsAsync<object>();

            return response.StatusCode;
        }

        #endregion

        #region Create an indexer

        /// <summary>
        /// Creates the indexer.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <returns></returns>
        public  async Task<HttpStatusCode> CreateIndexer(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexers?api-version=2015-02-28-Preview", mSearchServiceName));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            }
            request.Headers.Add("api-key", mApiKey);

            HttpResponseMessage response = await mClient.SendAsync(request);
            //dynamic body = await response.Content.ReadAsAsync<object>();

            return response.StatusCode;
        }

        #endregion

        #region Update an indexer

        /// <summary>
        /// Creates the indexer.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> UpdateIndexer(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexers?api-version=2015-02-28-Preview", mSearchServiceName));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri);

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            }
            request.Headers.Add("api-key", mApiKey);

            HttpResponseMessage response = await mClient.SendAsync(request);
            //dynamic body = await response.Content.ReadAsAsync<object>();

            return response.StatusCode;
        }

        #endregion

        #region Search for documents

        /// <summary>
        /// Searches for documents.
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> SearchDocuments(string searchTerms)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs?search={2}&api-version=2015-02-28-Preview", 
                mSearchServiceName, 
                "test-index",
                searchTerms));
            var requestGet = new HttpRequestMessage(HttpMethod.Get, uri);
            requestGet.Headers.Add("api-key", mApiKey);

            HttpResponseMessage response = await mClient.SendAsync(requestGet);

            dynamic body = await response.Content.ReadAsAsync<object>();

            return body;
        }

        #endregion

        #region Get a document by Id

        /// <summary>
        /// Gets the by azure identifier.
        /// </summary>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        public async Task<dynamic> GetByAzureId(string docId)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs/{2}/?api-version=2015-02-28-Preview", mSearchServiceName, "test-index", docId));
            var requestGet = new HttpRequestMessage(HttpMethod.Get, uri);
            requestGet.Headers.Add("api-key", mApiKey);
            
            HttpResponseMessage response = await mClient.SendAsync(requestGet);

            dynamic body = await response.Content.ReadAsAsync<object>();

            return body;
        }

        #endregion
    }
}