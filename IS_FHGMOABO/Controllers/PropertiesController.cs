using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.PropertiesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IS_FHGMOABO.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public PropertiesController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

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
        [HttpPost]
        public async Task<IActionResult> AddFullPropertyNaturalPerson(AddFullPropertyNaturalPersonModel _add)
        {
            var property = new Property()
            {
                RoomId = _add.RoomId,
                Type = "Собственность",
                Share = 1,
                DateOfTaking = _add.PropertyDate.DateOfTaking,
                EndDate = _add.PropertyDate.EndDate,
                TypeOfStateRegistration = _add.StateRegistration.Type,
                StateRegistrationNumber = _add.StateRegistration.Number,
                ByWhomIssued = _add.StateRegistration.ByWhomIssued,

                NaturalPersons = new List<DAL.NaturalPerson>()
            };

            var person = new DAL.NaturalPerson()
            {
                FirstName = _add.NaturalPerson.FirstName,
                LastName = _add.NaturalPerson.LastName,
                Patronymic = _add.NaturalPerson.Patronymic,
                DateOfBirth = _add.NaturalPerson.DateOfBirth,
            };

            property.NaturalPersons.Add(person);
            await _applicationDBContext.AddAsync(property);
            await _applicationDBContext.SaveChangesAsync();

            return RedirectToAction("Index", "Properties");
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
