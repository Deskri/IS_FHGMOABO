using Microsoft.AspNetCore.Mvc;

namespace IS_FHGMOABO.Controllers
{
    public class PropertiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return PartialView();
        }
    }
}
