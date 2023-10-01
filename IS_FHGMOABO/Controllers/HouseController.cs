using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.HouseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        public IActionResult Details () 
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
    }
}
