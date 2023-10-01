using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.HouseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            model.Houses = await _applicationDBContext.Houses.ToListAsync();

            foreach (var house in model.Houses)
            {
                var rooms = await _applicationDBContext.Rooms.Where(x => x.HouseId == house.Id).ToListAsync();

                if (rooms == null || rooms.Count == 0)
                {
                    house.RoomsCount = 0;
                }
                else
                {
                    house.RoomsCount = rooms.Count();
                }
            }

            return View(model);
        }

        public IActionResult Details () 
        { 
            return PartialView("Details", new House()); 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
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

            var model = new IndexHouseModel();
            model.AddHouse = _addHouseModel;
            model.Houses = await _applicationDBContext.Houses.ToListAsync();

            return View("Index", model);
        }
    }
}
