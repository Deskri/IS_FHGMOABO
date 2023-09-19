using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IS_FHGMOABO.Controllers
{
    public class PropertiesController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddFullPropertyNaturalPerson() 
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddFullPropertyLegalPerson()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddSharedPropertyNaturalPerson()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddSharedPropertyLegalPerson()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddJointProperty()
        {
            return PartialView();
        }
    }
}
