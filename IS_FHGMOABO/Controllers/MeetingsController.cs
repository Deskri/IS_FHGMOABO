using IS_FHGMOABO.DBConection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IS_FHGMOABO.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public MeetingsController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
