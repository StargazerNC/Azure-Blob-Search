using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MvcApp.App_Start
{
    public class AzureSearchHelper
    {
        private readonly string mApiKey = "";
        private readonly string mStorageConnectionString="";
        private readonly string mSearchServiceName ="";

        private HttpClient mClient = new HttpClient();

        public AzureSearchHelper()
        {
            mApiKey = ConfigurationManager.AppSettings["Azure.Search.ApiKey"];
            mStorageConnectionString = ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"];
            mSearchServiceName = ConfigurationManager.AppSettings["Azure.Search.ServiceName"];
        }

        public async Task CreateDataSource(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/datasources?api-version=2015-02-28-Preview", mSearchServiceName));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                request.Headers.Add("api-key", mApiKey);
            }
            mClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await mClient.SendAsync(request);
            dynamic body = await response.Content.ReadAsAsync<object>();

            System.Diagnostics.Debug.WriteLine(response);
        }

        public  async Task CreateIndexer(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexers?api-version=2015-02-28-Preview", mSearchServiceName));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await mClient.SendAsync(request);
            dynamic body = await response.Content.ReadAsAsync<object>();

            System.Diagnostics.Debug.WriteLine(response);
        }

        public async Task<dynamic> SearchDocuments(dynamic requestBody)
        {
            var uri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs?search=*&api-version=2015-02-28-Preview", mSearchServiceName, "testindex"));
            var requestGet = new HttpRequestMessage(HttpMethod.Get, uri);
            requestGet.Headers.Add("api-key", mApiKey);

            HttpResponseMessage response = await mClient.SendAsync(requestGet);

            dynamic body = await response.Content.ReadAsAsync<object>();

            //System.Diagnostics.Debug.WriteLine("Response: " + response);
            //System.Diagnostics.Debug.WriteLine("Result: " + body.value[0].id);

            return body;
        }
    }
}