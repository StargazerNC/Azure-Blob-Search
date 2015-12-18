using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MvcApp.App_Start;
using System.Collections.Generic;

namespace MvcApp.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public async Task<ActionResult> CreateDataSource()
        {
            return View();
        }

        public async Task<ActionResult> UpdateDataSource()
        {
            var requestBody = new
            {
                name = "blob-test-ds",
                description = "Optional. Anything you want, or nothing at all",
                type = "azureblob",
                credentials = new { connectionString = ConfigurationManager.AppSettings["Azure.Storage.ConnectionString"] },
                container = new { name = "coiso" }
                //dataChangeDetectionPolicy = new { Optional. See below for details }, 
                //dataDeletionDetectionPolicy = new { Optional. See below for details }
            };

            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.UpdateDataSource("blob-test-ds", requestBody);
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> CreateIndexer()
        {
            var requestBody = new
            {
                name = "test-indexer",
                description = "Test indexer",
                dataSourceName = "blob-test-ds",
                targetIndexName = "test-index",
                schedule = new { interval = "PT1H", startTime = "2015-01-01T00:00:00Z" },
                parameters = new { maxFailedItems = 10, maxFailedItemsPerBatch = 5, base64EncodeKeys = true },
                fieldMappings = new [] {
                                        new { sourceFieldName = "fileId", targetFieldName = "id" },
                                        new { sourceFieldName = "metadata_title", targetFieldName =  "title" }
                                        }
            };

            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.CreateIndexer(requestBody);
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> UpdateIndexer()
        {
            var requestBody = new
            {
                name = "test-indexer",
                description = "Test indexer",
                dataSourceName = "blob-test-ds",
                targetIndexName = "test-index",
                schedule = new { interval = "PT1H", startTime = "2015-01-01T00:00:00Z" },
                parameters = new { maxFailedItems = 10, maxFailedItemsPerBatch = 5, base64EncodeKeys = false },
                fieldMappings = new [] {
                                        new { sourceFieldName = "fileId", targetFieldName = "id" },
                                        new { sourceFieldName = "metadata_title", targetFieldName =  "title" }
                                        }
            };

            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.UpdateIndexer("test-indexer", requestBody);
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> RunIndexer()
        {
            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.RunIndexer("test-indexer");
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> DeleteIndexer()
        {
            return View();
        }

        public async Task<ActionResult> GetAll()
        {
            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.SearchDocuments("*");
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> Get()
        {
            AzureSearchHelper helper = new AzureSearchHelper();

            var id = "2";

            var resp = await helper.GetByAzureId(id);
            return View("../Home/Index", resp.Content);
        }

        public async Task<ActionResult> Search(string searchTerms = null)
        {
            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.SearchDocuments(searchTerms);
            return View("../Home/Index", resp);
        }

        private static int docId = 1;

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> PostFile(HttpPostedFileBase file)
        {
            
            if (file.ContentLength > 0)
            {
                var storage = new AzureBlobsStorageHelper();
                await storage.WriteFromStream(file.FileName, "coiso", file.InputStream, docId.ToString());
            }
            docId+=1;
            return RedirectToAction("../Home/Index", docId);
        }

        public async Task<ActionResult> DeleteDocuments()
        {
            var helper = new AzureSearchHelper();

            dynamic docs = await helper.SearchDocuments("*");

            dynamic documents = docs.value;

            var deletePayloads = new List<string>();

            string payload = string.Empty;

            foreach (var doc in documents)
            {
                payload = "\t\t{\r\n";
                payload += "\t\t\t\"@search.action\": \"delete\",\r\n";
                payload += string.Format("\t\t\t\"id\": \"{0}\"\r\n", doc.id);
                payload += "\t\t}";

                deletePayloads.Add(payload);
            }

            var requestBody = new StringBuilder();

            requestBody.AppendLine("{");
            requestBody.AppendLine("\t\"value\" : [");
            
            string payloads = string.Join(",\r\n", deletePayloads.ToArray());

            requestBody.AppendLine(payloads);

            requestBody.AppendLine("\t]");
            requestBody.AppendLine("}");

            System.Diagnostics.Debug.WriteLine(requestBody.ToString());

            await helper.DeleteDocuments("test-index", requestBody.ToString());
            
            return View("../Home/Index", docs);

        }
    }
}