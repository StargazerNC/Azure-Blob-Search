using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace AzureBlobSearch
{
    class Program
    {
        private const string connString = "<connection string>";
        private const string searcgServiceName = "<service name>";
        private const string apiKey = "<api key>";

        static void Main(string[] args)
        {
            var requestCreateDataSource = new { 
                name= "blob-test-ds",
                description= "Optional. Anything you want, or nothing at all",
                type = "azureblob",
                credentials = new { connectionString = connString },
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
                                           schedule = new { interval = "PT1H", startTime = "2015-01-01T00:00:00Z"},
                                           parameters = new { maxFailedItems = 10, maxFailedItemsPerBatch = 5, base64EncodeKeys = false}
                                       };

            Uri dataSourcesUrl = new Uri(string.Format("https://{0}.search.windows.net/datasources?api-version=2015-02-28-Preview", "ncoimbra"));
            Uri indexersUrl = new Uri(string.Format("https://{0}.search.windows.net/indexers?api-version=2015-02-28-Preview", "ncoimbra"));

            HttpClient client = new HttpClient(); // If you'll make many requests you'll want to reuse this instance
            HttpRequestMessage requestDs = new HttpRequestMessage(HttpMethod.Post, dataSourcesUrl);
            HttpRequestMessage requestIdx = new HttpRequestMessage(HttpMethod.Post, indexersUrl);

            //request.Headers.Add("Content-Type", "application/json");
            requestDs.Headers.Add("api-key", apiKey);
            requestIdx.Headers.Add("api-key", apiKey);

            Console.WriteLine("Select action, please: ");
            Console.WriteLine("1 - Create DataSource ");
            Console.WriteLine("2 - Create Indexer ");
            Console.WriteLine("3 - ... ");

            char key = Console.ReadKey().KeyChar;
            switch(key)
            {
                case '1':
                    CreateDataSource(client, requestDs, requestCreateDataSource);
                    break;
                case '2':
                    CreateIndexer(client, requestIdx, requestCreateIndexer);
                    break;
                case '3':
                    break;
            }

            Console.ReadLine();
        }
 
        private static void CreateDataSource(HttpClient client, HttpRequestMessage request, dynamic requestCreateDataSource)
        {
            if (requestCreateDataSource != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestCreateDataSource), Encoding.UTF8, "application/json");
            }
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.SendAsync(request).Result;

            Console.WriteLine(response);
        }

        private static void CreateIndexer(HttpClient client, HttpRequestMessage request, dynamic requestCreateDataSource)
        {
           if (requestCreateDataSource != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestCreateDataSource), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = client.SendAsync(request).Result;

            Console.WriteLine(response);
        }
    }
}
