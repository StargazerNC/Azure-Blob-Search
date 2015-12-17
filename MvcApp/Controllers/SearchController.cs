using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                                        //new { sourceFieldName = "metadata_storage_name", targetFieldName = "id" },
                                        new { sourceFieldName =  "metadata_title", targetFieldName =  "title" }
                                        }
            };

            AzureSearchHelper helper = new AzureSearchHelper();
            var resp = await helper.CreateIndexer(requestBody);
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> UpdateIndexer()
        {
            return View();
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

        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                AzureBlobsStorageHelper storage = new AzureBlobsStorageHelper();
                await storage.WriteFromStream(file.FileName, "coiso", file.InputStream, "file1");
            }

            return RedirectToAction("Index");
        }
    }
}