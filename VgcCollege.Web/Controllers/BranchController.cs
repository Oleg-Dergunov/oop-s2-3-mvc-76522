using Microsoft.AspNetCore.Mvc;

namespace VgcCollege.Web.Controllers
{
    public class BranchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
