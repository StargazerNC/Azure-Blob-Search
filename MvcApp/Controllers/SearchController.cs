using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MvcApp.App_Start;

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

            var id = "aAB0AHQAcABzADoALwAvAG4AYwBvAGkAbQBiAHIAYQAuAGIAbABvAGIALgBjAG8AcgBlAC4AdwBpAG4AZABvAHcAcwAuAG4AZQB0AC8AYwBvAGkAcwBvAC8AUgBlAGEAZABtAGUALgBkAG8AYwA1";

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
                AzureBlobsStorageHelper storage = new AzureBlobsStorageHelper();
                await storage.WriteFromStream(file.FileName, "coiso", file.InputStream, docId.ToString());
            }
            docId+=1;
            return RedirectToAction("../Home/Index", docId);
        }

        public async Task<ActionResult> DeleteDocuments()
        {
            AzureSearchHelper helper = new AzureSearchHelper();

            var docs = await helper.SearchDocuments("*");

            var documents = docs.value;

            //foreach(var doc in documents)
            //{
            //    ids.Add(doc.id.ToString());
            //}

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("{");
            builder.AppendLine("\"value\": [");

            foreach (var doc in documents)
            {
                builder.AppendLine("{");
                builder.AppendLine("\"@search.action\": \"delete\",");
                builder.AppendFormat("\"id\": \"{0}\"", doc.id);
                builder.AppendLine("},");
            }
            string requestBody = builder.ToString();
            string cleared = requestBody.Replace("\n", "").Replace("\r", "");
            cleared = cleared.Remove(cleared.Length - 1, 1);
            cleared += "]";
            cleared += "}";

            await helper.DeleteDocuments("test-index", cleared);
            
            return View("../Home/Index", docs);

        }
    }
}