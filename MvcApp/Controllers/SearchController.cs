using System;
using System.Linq;
using System.Threading.Tasks;
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
            return View();
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
            var resp = await helper.SearchDocuments(null);
            return View("../Home/Index", resp);
        }

        public async Task<ActionResult> Get(object id)
        {
            return View();
        }

        public async Task<ActionResult> Search(dynamic requestBody)
        {
            return View();
        }
    }
}