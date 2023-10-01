using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.HouseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static IS_FHGMOABO.Models.HouseModels.EditHouseModel;

namespace IS_FHGMOABO.Controllers
{
    public class HouseController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public HouseController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexHouseModel();
            model.Houses = await _applicationDBContext.Houses
                                                            .Include(x => x.Rooms)
                                                            .ToListAsync();

            foreach (var house in model.Houses)
            {
                if (house.Rooms == null || house.Rooms.Count == 0)
                {
                    house.RoomsCount = 0;
                }
                else
                {
                    house.RoomsCount = house.Rooms.Count();
                }

                // Необходимо для сериализации, чтобы избавиться от зациклености
                foreach (var room in house.Rooms)
                {
                    room.House = null;
                }
            }

            var serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexHouseModel", serializedModel);

            return View(model);
        }

        public IActionResult Details()
        {
            return PartialView("Details", new House());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("Add", new AddHouseModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddHouseModel _addHouseModel)
        {
            if (ModelState.IsValid)
            {
                var house = new House()
                {
                    Number = _addHouseModel.Number,
                    Type = _addHouseModel.Type.ToString(),
                    Street = _addHouseModel.Street,
                    Region = _addHouseModel.Region,
                    InhabitedLocality = _addHouseModel.InhabitedLocality,
                    District = _addHouseModel.District,
                    Subject = _addHouseModel.Subject,
                    Country = _addHouseModel.Country,
                    HouseCadastralNumber = _addHouseModel.HouseCadastralNumber,
                    PlotCadastralNumber = _addHouseModel.PlotCadastralNumber,
                    HousesPassportedFloorArea = _addHouseModel.HousesPassportedFloorArea,
                    PlotPassportedFloorArea = _addHouseModel.PlotPassportedFloorArea,
                    ResidentialPremisesPassportedArea = _addHouseModel.ResidentialPremisesPassportedArea,
                    NonResidentialPremisesPassportedArea = _addHouseModel.NonResidentialPremisesPassportedArea,
                };

                await _applicationDBContext.AddAsync(house);
                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index", "House");
            }

            var serializedModel = HttpContext.Session.GetString("IndexHouseModel");
            var model = JsonConvert.DeserializeObject<IndexHouseModel>(serializedModel);

            model.AddHouse = _addHouseModel;

            serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexHouseModel", serializedModel);

            return View("Index", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var house = await _applicationDBContext.Houses.FindAsync(id);

            if (house != null)
            {
                _applicationDBContext.Remove(house);
                _applicationDBContext.SaveChanges();
            }

            return RedirectToAction("Index", "House");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var house = await _applicationDBContext.Houses.FindAsync(id);

            var model = new EditHouseModel()
            {
                Id = id,
                Number = house.Number,
                Street = house.Street,
                Region = house.Region,
                InhabitedLocality = house.InhabitedLocality,
                District = house.District,
                Subject = house.Subject,
                Country = house.Country,
                HouseCadastralNumber = house.HouseCadastralNumber,
                PlotCadastralNumber = house.PlotCadastralNumber,
                HousesPassportedFloorArea = house.HousesPassportedFloorArea,
                PlotPassportedFloorArea = house.PlotPassportedFloorArea,
                ResidentialPremisesPassportedArea = house.HousesPassportedFloorArea,
                NonResidentialPremisesPassportedArea = house.NonResidentialPremisesPassportedArea,
            };

            foreach (StreetType type in Enum.GetValues(typeof(StreetType)))
            {
                if (house.Type == type.ToString())
                {
                    model.Type = type;
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditHouseModel model)
        {
            if (ModelState.IsValid)
            {
                var house = await _applicationDBContext.Houses.FindAsync(model.Id);

                house.Number = model.Number;
                house.Type = model.Type.ToString();
                house.Street = model.Street;
                house.Region = model.Region;
                house.InhabitedLocality = model.InhabitedLocality;
                house.District = model.District;
                house.Subject = model.Subject;
                house.Country = model.Country;
                house.HouseCadastralNumber = model.HouseCadastralNumber;
                house.PlotCadastralNumber = model.PlotCadastralNumber;
                house.HousesPassportedFloorArea = model.HousesPassportedFloorArea;
                house.PlotPassportedFloorArea = model.PlotPassportedFloorArea;
                house.ResidentialPremisesPassportedArea = model.ResidentialPremisesPassportedArea;
                house.NonResidentialPremisesPassportedArea = model.NonResidentialPremisesPassportedArea;

                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index", "House");
            }

            return View("Edit", model);
        }
    }
}
