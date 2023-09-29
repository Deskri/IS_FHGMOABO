using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.HouseModels;
using IS_FHGMOABO.Models.PropertiesModels;
using IS_FHGMOABO.Models.RoomModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using static IS_FHGMOABO.Models.RoomModels.EditRoomModel;

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
                        .Where(x => x.HouseId == id)
                        .OrderBy(x => x.Type)
                        .ThenBy(x => x.Number)
                        .ToListAsync();

            model.House = await _applicationDBContext.Houses
                        .FirstOrDefaultAsync(x => x.Id == id);

            // Необходимо для сериализации, чтобы избавиться от зациклености
            model.House.Rooms = null;

            model.AddRoom = new AddRoomModel()
            {
                HouseId = id,
            };

            var serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexRoomModel", serializedModel);

            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int idIndex)
        {
            var room = await _applicationDBContext.Rooms.FindAsync(id);

            if (room != null)
            {
                _applicationDBContext.Remove(room);
                _applicationDBContext.SaveChanges();
            }

            return RedirectToAction("Index", "Room", new { id = idIndex });
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
                                                                            && x.HouseId == _addRoomModel.HouseId
                                                                            && x.Type == _addRoomModel.Type.ToString());

            if (sameNumber != null)
            {
                ModelState.AddModelError("Number", "Номер помещения не должен повторяться.");
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
                };

                await _applicationDBContext.AddAsync(room);
                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index", "Room", new { id = _addRoomModel.HouseId });
            }

            var serializedModel = HttpContext.Session.GetString("IndexRoomModel");
            var model = JsonConvert.DeserializeObject<IndexRoomModel>(serializedModel);

            return View("Index", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _applicationDBContext.Rooms.FindAsync(id);

            var model = new EditRoomModel()
            {
                HouseId = room.HouseId,
                Number = room.Number,
                TotalArea = room.TotalArea,
                LivingArea = room.LivingArea,
                UsableArea = room.UsableArea,
                Floor = room.Floor,
                Entrance = room.Entrance,
                CadastralNumber = room.CadastralNumber,
                IsPrivatized = room.IsPrivatized,
            };

            foreach (RoomType type in Enum.GetValues(typeof(RoomType)))
            {
                if (room.Type == type.ToString())
                {
                    model.Type = type;
                }
            }

            foreach (RoomPurpose purpose in Enum.GetValues(typeof(RoomPurpose)))
            {
                if (room.Type == purpose.ToString())
                {
                    model.Purpose = purpose;
                }
            }

            return View(model);
        }
    }
}
