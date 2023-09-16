using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.HouseModels;
using IS_FHGMOABO.Models.RoomModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace IS_FHGMOABO.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public RoomController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var model = new IndexRoomModel();

            model.Rooms = await _applicationDBContext.Rooms
                        .Where(x => x.HouseId == id && x.Deleted == null)
                        .OrderBy(x => x.Type)
                        .ThenBy(x => x.Number)
                        .ToListAsync();

            model.House = await _applicationDBContext.Houses
                        .FirstOrDefaultAsync(x => x.Id == id);

            model.AddRoom = new AddRoomModel()
            {
                HouseId = id,
            };

            return View(model);
        }

        public async Task<IActionResult> Archive(int id)
        {
            var model = new IndexRoomModel();

            model.Rooms = await _applicationDBContext.Rooms
                        .Where(x => x.HouseId == id && x.Deleted != null)
                        .OrderBy(x => x.Type)
                        .ThenBy(x => x.Number)
                        .ToListAsync();

            model.House = await _applicationDBContext.Houses
                        .FirstOrDefaultAsync(x => x.Id == id);

            model.AddRoom = new AddRoomModel()
            {
                HouseId = id,
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("Add");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddRoomModel _addRoomModel)
        {
            bool _modelState = ModelState.IsValid;

            var sameNumber = await _applicationDBContext.Rooms
                        .FirstOrDefaultAsync(x => x.Number == _addRoomModel.Number
                                            && x.Type == _addRoomModel.Type.ToString()
                                            && x.Deleted == null);

            if (sameNumber != null)
            {
                ModelState.AddModelError("Number", "Номер помещения не должно повторяться.");
                _modelState = false;
            }

            if (_addRoomModel.TotalArea == 0)
            {
                ModelState.AddModelError("TotalArea", "Общая площадь помещения не должна быть равна 0.");
                _modelState = false;
            }

            if (_modelState)
            {
                var room = new Room()
                {
                    HouseId = _addRoomModel.HouseId,
                    Type = _addRoomModel.Type.ToString(),
                    Number = _addRoomModel.Number,
                    Purpose = _addRoomModel.Purpose.ToString(),
                    TotalArea = _addRoomModel.TotalArea,
                    LivingArea = _addRoomModel.LivingArea,
                    UsableArea = _addRoomModel.UsableArea,
                    Floor = _addRoomModel.Floor,
                    Entrance = _addRoomModel.Entrance,
                    CadastralNumber = _addRoomModel.CadastralNumber,
                    IsPrivatized = _addRoomModel.IsPrivatized,
                    Created = DateTime.Now,
                };

                await _applicationDBContext.AddAsync(room);
                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index", "Room", new { id = _addRoomModel.HouseId });
            }

            var model = new IndexRoomModel();
            model.AddRoom = _addRoomModel;
            model.Rooms = await _applicationDBContext.Rooms
                        .Where(x => x.HouseId == _addRoomModel.HouseId)
                        .OrderBy(x => x.Type)
                        .ThenBy(x => x.Number)
                        .ToListAsync();

            model.House = await _applicationDBContext.Houses
                        .FirstOrDefaultAsync(x => x.Id == _addRoomModel.HouseId);

            return View("Index", model);
        }
    }
}
