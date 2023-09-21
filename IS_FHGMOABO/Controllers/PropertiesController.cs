using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.PropertiesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index()
        {
            var model = new IndexPropertiesModel();
            model.AddProperties = new AddPropertiesModel();
            model.AddProperties.AddFullProperty = new AddFullPropertyModel()
            {
                            
            };
            model.AddProperties.AddFullProperty.Rooms = await _applicationDBContext.Rooms.Where(x => x.Deleted == null).ToListAsync();

            foreach(var room in model.AddProperties.AddFullProperty.Rooms)
            {
                room.House = await _applicationDBContext.Houses.FirstOrDefaultAsync(x => x.Id == room.HouseId);
            }

            return View(model);
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
        public async Task<IActionResult> AddFullProperty(AddFullPropertyModel _add)
        {
            var rooms = await _applicationDBContext.Rooms.FirstOrDefaultAsync(x => x.Id == _add.RoomId && x.Deleted == null);

            if (rooms == null)
            {
                ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
            }
            else
            {
                var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _add.RoomId 
                                                                            && x.EndDate == null)
                                                                            .ToListAsync();

                if (properties.Count != 0)
                {
                    ModelState.AddModelError("Rooms", "В этом помещение уже есть право собственности.");
                }
            }

            if (_add.PropertyDate.DateOfTaking > DateTime.Now)
            {
                ModelState.AddModelError("PropertyDate.DateOfTaking", "Дата вступления в собственность не должна быть больше текущего времени.");
            }

            if(_add.IsNaturalPerson)
            {
                if(_add.NaturalPerson.FirstName == null)
                {
                    ModelState.AddModelError("NaturalPerson.FirstName", "Имя собственника должно быть заполнено.");
                }

                if (_add.NaturalPerson.LastName == null)
                {
                    ModelState.AddModelError("NaturalPerson.LastName", "Фамилия собственника должна быть заполнена.");
                }

                if (_add.NaturalPerson.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("NaturalPerson.DateOfBirth", "Дата рождения не должна быть больше текущего времени.");
                }

                if (ModelState.IsValid)
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
            }
            else
            {
                if (_add.LegalPerson.Name == null)
                {
                    ModelState.AddModelError("LegalPerson.Name", "Наименование организации должно быть заполнено.");
                }

                if (ModelState.IsValid)
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

                        LegalPerson = new DAL.LegalPerson()
                    };

                    var organization = new DAL.LegalPerson()
                    {
                        Name = _add.LegalPerson.Name,
                    };

                    property.LegalPerson = organization;
                    await _applicationDBContext.AddAsync(property);
                    await _applicationDBContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Properties");
                }
            }

            var model = new IndexPropertiesModel();
            model.AddProperties = new AddPropertiesModel();
            model.AddProperties.AddFullProperty = _add;

            return View("Index", model);
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
