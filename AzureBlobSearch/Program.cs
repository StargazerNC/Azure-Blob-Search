using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureBlobSearch
{
    internal class Program
    {
        private const string ConnString = "<connection string>";
        private const string SearchServiceName = "service name";
        private const string ApiKey = "key here";

        private static void Main(string[] args)
        {
            var requestCreateDataSource = new
            {
                name = "blob-test-ds",
                description = "Optional. Anything you want, or nothing at all",
                type = "azureblob",
                credentials = new { connectionString = ConnString },
                container = new { name = "coiso" }
                //dataChangeDetectionPolicy = new { Optional. See below for details }, 
                //dataDeletionDetectionPolicy = new { Optional. See below for details }
            };

            var requestCreateIndexer = new
            {
                name = "myindexer",
                description = "a cool indexer",
                dataSourceName = "blob-test-ds",
                targetIndexName = "testindex",
                schedule = new { interval = "PT1H", startTime = "2015-01-01T00:00:00Z" },
                parameters = new { maxFailedItems = 10, maxFailedItemsPerBatch = 5, base64EncodeKeys = false }
            };

            var requestGetDocuments = new
            {
                name = "myindexer",
                description = "a cool indexer",
                dataSourceName = "blob-test-ds",
                targetIndexName = "testindex",
                schedule = new { interval = "PT1H", startTime = "2015-01-01T00:00:00Z" },
                parameters = new { maxFailedItems = 10, maxFailedItemsPerBatch = 5, base64EncodeKeys = false }
            };

            string indexName = "testindex";

            var dataSourcesUrl = new Uri(string.Format("https://{0}.search.windows.net/datasources?api-version=2015-02-28-Preview", SearchServiceName));
            var indexersUrl = new Uri(string.Format("https://{0}.search.windows.net/indexers?api-version=2015-02-28-Preview", SearchServiceName));
            var getDocsUrl = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs?search=*&api-version=2015-02-28-Preview", SearchServiceName, indexName));

            var client = new HttpClient(); // If you'll make many requests you'll want to reuse this instance
            var requestDs = new HttpRequestMessage(HttpMethod.Post, dataSourcesUrl);
            var requestIdx = new HttpRequestMessage(HttpMethod.Post, indexersUrl);
            var requestGet = new HttpRequestMessage(HttpMethod.Get, getDocsUrl);

            //request.Headers.Add("Content-Type", "application/json");
            requestDs.Headers.Add("api-key", ApiKey);
            requestIdx.Headers.Add("api-key", ApiKey);
            requestGet.Headers.Add("api-key", ApiKey);

            Console.WriteLine("Select action, please: ");
            Console.WriteLine("1 - Create DataSource ");
            Console.WriteLine("2 - Create Indexer ");
            Console.WriteLine("3 - Get ");

            char key = Console.ReadKey().KeyChar;
            do
            {
                switch(key)
                {
                    case '1':
                        CreateDataSource(client, requestDs, requestCreateDataSource);
                        break;
                    case '2':
                        CreateIndexer(client, requestIdx, requestCreateIndexer);
                        break;
                    case '3':
                        SearchDocuments(client, requestGet, requestGetDocuments);
                        break;
                }

                key = Console.ReadKey().KeyChar;
            }
            while (key != 'q');
        }
 
        private async static Task CreateDataSource(HttpClient client, HttpRequestMessage request, dynamic requestCreateDataSource)
        {
            if (requestCreateDataSource != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestCreateDataSource), Encoding.UTF8, "application/json");
            }
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.SendAsync(request);
            dynamic body = await response.Content.ReadAsAsync<object>();

            Console.WriteLine(response);
            Console.WriteLine("Details: " + body);
        }

        private async static Task CreateIndexer(HttpClient client, HttpRequestMessage request, dynamic requestCreateDataSource)
        {
            if (requestCreateDataSource != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestCreateDataSource), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await client.SendAsync(request);
            dynamic body = await response.Content.ReadAsAsync<object>();

            Console.WriteLine(response);
            Console.WriteLine("Details: " + body);
        }

        private async static Task SearchDocuments(HttpClient client, HttpRequestMessage request, dynamic requestCreateDataSource)
        {
            //if (requestCreateDataSource != null)
            //{
            //    request.Content = new StringContent(JsonConvert.SerializeObject(requestCreateDataSource), Encoding.UTF8, "application/json");
            //}
            HttpResponseMessage response = client.SendAsync(request).Result;
            dynamic body = await response.Content.ReadAsAsync<object>();
            Console.WriteLine(string.Format("Response: {0}", response.StatusCode));
            Console.WriteLine("---");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Result RAW: " + body);
            Console.WriteLine("---");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Result: " + body.value[0].id);
        }
    }
}
