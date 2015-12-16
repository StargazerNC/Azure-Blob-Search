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

        https://msdn.microsoft.com/en-us/magazine/dd419663.aspx?tduid=(85cd6a29db21a2b98f6cea1effa2f1fa)(256380)(2459594)(TnL5HPStwNw-Ain82ukW2sP_XtlgT9b5JA)()#id0090009
        http://martinfowler.com/eaaDev/uiArchs.html
        http://stackoverflow.com/questions/667781/what-is-the-difference-between-mvc-and-mvvm

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