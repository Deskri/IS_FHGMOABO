using IS_FHGMOABO.DBConection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IS_FHGMOABO.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public RoomController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<IActionResult> Index(int id)
        {
            var model = await _applicationDBContext.Rooms.Where(x => x.HouseId == id).ToListAsync();
            return View(model);
        }
    }
}
