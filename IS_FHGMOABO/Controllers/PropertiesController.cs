using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.PropertiesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            model.AddProperties.AddFullProperty = new AddFullPropertyModel();
            model.AddProperties.AddSharedProperty = new AddSharedPropertyModel();
            model.AddProperties.AddJointProperty = new AddJointPropertyModel();

            var rooms = await _applicationDBContext.Rooms
                                            .Where(x => x.Deleted == null)
                                            .Include(x => x.House)
                                            .ToListAsync();

            // Необходимо для сериализации, чтобы избавиться от зациклености
            foreach (var room in rooms)
            {
                room.House.Rooms = null;
            }

            model.AddProperties.AddFullProperty.Rooms = rooms;
            model.AddProperties.AddSharedProperty.Rooms = rooms;
            model.AddProperties.AddJointProperty.Rooms = rooms;

            var properties = await _applicationDBContext.Properties
                                                    .Include(x => x.NaturalPersons)
                                                    .Include(x => x.LegalPerson)
                                                    .Include(x => x.Room.House)
                                                    .ToListAsync();

            // Необходимо для сериализации, чтобы избавиться от зациклености
            foreach (var property in properties)
            {
                property.Room.Properties = null;

                if (property.LegalPerson != null)
                {
                    property.LegalPerson.Properties = null;
                }

                if (property.NaturalPersons != null)
                {
                    foreach (var person in property.NaturalPersons)
                    {
                        person.Properties = null;
                    }
                }
            }

            model.Properties = properties;

            var serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexPropertiesModel", serializedModel);

            return View("Index", model);
        }

        public async Task<IActionResult> Delete (int id)
        {
            var property = await _applicationDBContext.Properties
                                                    .Include(x => x.NaturalPersons)
                                                    .Include(x => x.LegalPerson)
                                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (property.LegalPerson != null)
            {
                _applicationDBContext.Remove(property.LegalPerson);
            }

            if (property.NaturalPersons.Count != 0)
            {
                foreach (var person in property.NaturalPersons)
                {
                    _applicationDBContext.Remove(person);
                }
            }
            _applicationDBContext.Remove(property);
            _applicationDBContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddFullProperty()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFullProperty(AddFullPropertyModel _add)
        {
            try
            {
                var _roomId = Int32.Parse(_add.RoomId);

                var rooms = await _applicationDBContext.Rooms.FirstOrDefaultAsync(x => x.Id == _roomId && x.Deleted == null);

                if (rooms == null)
                {
                    ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
                }
                else
                {
                    var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _roomId
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

                if (_add.IsNaturalPerson)
                {
                    if (_add.NaturalPerson.FirstName == null)
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
                            RoomId = _roomId,
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

                        _add.IsReturn = false;

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
                            RoomId = _roomId,
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

                        _add.IsReturn = false;

                        return RedirectToAction("Index", "Properties");
                    }
                }
            }
            catch (FormatException)
            {
                ModelState.AddModelError("RoomId", "Введите корректное значение.");
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("RoomId", "Введите значение.");
            }

            _add.IsReturn = true;

            var serializedModel = HttpContext.Session.GetString("IndexPropertiesModel");
            var model = JsonConvert.DeserializeObject<IndexPropertiesModel>(serializedModel);

            var indexRooms = model.AddProperties.AddFullProperty.Rooms;

            model.AddProperties.AddFullProperty = _add;
            model.AddProperties.AddSharedProperty = new AddSharedPropertyModel();
            model.AddProperties.AddJointProperty = new AddJointPropertyModel();

            model.AddProperties.AddFullProperty.Rooms = indexRooms;
            model.AddProperties.AddSharedProperty.Rooms = indexRooms;
            model.AddProperties.AddJointProperty.Rooms = indexRooms;

            serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexPropertiesModel", serializedModel);

            return View("Index", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddSharedProperty()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSharedProperty(AddSharedPropertyModel _add)
        {
            try
            {
                var _roomId = Int32.Parse(_add.RoomId);

                var rooms = await _applicationDBContext.Rooms.FirstOrDefaultAsync(x => x.Id == _roomId && x.Deleted == null);

                decimal share = 0;

                if (_add.Share.Dividend == null
                    || _add.Share.Dividend <= 0)
                {
                    ModelState.AddModelError("Share.Dividend", "Делимое доли должно быть внесено и не должно быть равно или меньше 0.");
                }
                if (_add.Share.Divisor == null
                    || _add.Share.Divisor <= 0)
                {
                    ModelState.AddModelError("Share.Divisor", "Делитель доли должен быть внесен и не должен быть равен или меньше 0.");
                }
                if (_add.Share.Dividend != null
                    && _add.Share.Divisor != null
                    && _add.Share.Dividend > 0
                    && _add.Share.Divisor > 0)
                {
                    share = (decimal)_add.Share.Dividend / _add.Share.Divisor;

                    if (share > 1)
                    {
                        ModelState.AddModelError("Share", "Делимое не может быть больше делителя.");
                    }
                }

                if (rooms == null)
                {
                    ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
                }
                else if (_add.Share.Dividend != null
                    && _add.Share.Divisor != null
                    && _add.Share.Dividend > 0
                    && _add.Share.Divisor > 0
                    && share <= 1)
                {
                    var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _roomId
                                                                                && x.EndDate == null)
                                                                                .ToListAsync();

                    decimal shareAllRooms = 0;

                    foreach (var property in properties)
                    {
                        shareAllRooms += property.Share;
                    }

                    if (shareAllRooms + share > 1)
                    {
                        ModelState.AddModelError("Rooms", "В этом помещение уже есть право собственности или сумма долей превышает единицу.");
                    }
                }

                if (_add.PropertyDate.DateOfTaking > DateTime.Now)
                {
                    ModelState.AddModelError("PropertyDate.DateOfTaking", "Дата вступления в собственность не должна быть больше текущего времени.");
                }

                if (_add.IsNaturalPerson)
                {
                    if (_add.NaturalPerson.FirstName == null)
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
                            RoomId = _roomId,
                            Type = "Долевая собственность",
                            Share = (decimal)_add.Share.Dividend / _add.Share.Divisor,
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

                        _add.IsReturn = false;

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
                            RoomId = _roomId,
                            Type = "Долевая собственность",
                            Share = (decimal)_add.Share.Dividend / _add.Share.Divisor,
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

                        _add.IsReturn = false;

                        return RedirectToAction("Index", "Properties");
                    }
                }
            }
            catch (FormatException)
            {
                ModelState.AddModelError("RoomId", "Введите корректное значение.");
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("RoomId", "Введите значение.");
            }

            _add.IsReturn = true;

            var serializedModel = HttpContext.Session.GetString("IndexPropertiesModel");
            var model = JsonConvert.DeserializeObject<IndexPropertiesModel>(serializedModel);

            var indexRooms = model.AddProperties.AddFullProperty.Rooms;

            model.AddProperties.AddFullProperty = new AddFullPropertyModel();
            model.AddProperties.AddSharedProperty = _add;
            model.AddProperties.AddJointProperty = new AddJointPropertyModel();

            model.AddProperties.AddFullProperty.Rooms = indexRooms;
            model.AddProperties.AddSharedProperty.Rooms = indexRooms;
            model.AddProperties.AddJointProperty.Rooms = indexRooms;

            serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexPropertiesModel", serializedModel);

            return View("Index", model);

        }

        [Authorize]
        [HttpGet]
        public IActionResult AddJointProperty()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddJointProperty(AddJointPropertyModel _add)
        {
            try
            {
                var _roomId = Int32.Parse(_add.RoomId);

                var rooms = await _applicationDBContext.Rooms.FirstOrDefaultAsync(x => x.Id == _roomId && x.Deleted == null);

                decimal share = 0;

                if (_add.Share.Dividend == null
                    || _add.Share.Dividend <= 0)
                {
                    ModelState.AddModelError("Share.Dividend", "Делимое доли должно быть внесено и не должно быть равно или меньше 0.");
                }
                if (_add.Share.Divisor == null
                    || _add.Share.Divisor <= 0)
                {
                    ModelState.AddModelError("Share.Divisor", "Делитель доли должен быть внесен и не должен быть равен или меньше 0.");
                }
                if (_add.Share.Dividend != null
                    && _add.Share.Divisor != null
                    && _add.Share.Dividend > 0
                    && _add.Share.Divisor > 0)
                {
                    share = (decimal)_add.Share.Dividend / _add.Share.Divisor;

                    if (share > 1)
                    {
                        ModelState.AddModelError("Share", "Делимое не может быть больше делителя.");
                    }
                }

                if (rooms == null)
                {
                    ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
                }
                else if (_add.Share.Dividend != null
                    && _add.Share.Divisor != null
                    && _add.Share.Dividend > 0
                    && _add.Share.Divisor > 0
                    && share <= 1)
                {
                    var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _roomId
                                                                                && x.EndDate == null)
                                                                                .ToListAsync();

                    decimal shareAllRooms = 0;

                    foreach (var property in properties)
                    {
                        shareAllRooms += property.Share;
                    }

                    if (shareAllRooms + share > 1)
                    {
                        ModelState.AddModelError("Rooms", "В этом помещение уже есть право собственности или сумма долей превышает единицу.");
                    }
                }

                if (_add.PropertyDate.DateOfTaking > DateTime.Now)
                {
                    ModelState.AddModelError("PropertyDate.DateOfTaking", "Дата вступления в собственность не должна быть больше текущего времени.");
                }

                if (_add.FirstPerson.FirstName == null)
                {
                    ModelState.AddModelError("FirstPerson.FirstName", "Имя собственника должно быть заполнено.");
                }
                if (_add.FirstPerson.LastName == null)
                {
                    ModelState.AddModelError("FirstPerson.LastName", "Фамилия собственника должна быть заполнена.");
                }
                if (_add.FirstPerson.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("FirstPerson.DateOfBirth", "Дата рождения не должна быть больше текущего времени.");
                }

                if (_add.SecondPerson.FirstName == null)
                {
                    ModelState.AddModelError("SecondPerson.FirstName", "Имя собственника должно быть заполнено.");
                }
                if (_add.SecondPerson.LastName == null)
                {
                    ModelState.AddModelError("SecondPerson.LastName", "Фамилия собственника должна быть заполнена.");
                }
                if (_add.SecondPerson.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("SecondPerson.DateOfBirth", "Дата рождения не должна быть больше текущего времени.");
                }

                if (ModelState.IsValid)
                {
                    var property = new Property()
                    {
                        RoomId = _roomId,
                        Type = "Совместная собственность",
                        Share = (decimal)_add.Share.Dividend / _add.Share.Divisor,
                        DateOfTaking = _add.PropertyDate.DateOfTaking,
                        EndDate = _add.PropertyDate.EndDate,
                        TypeOfStateRegistration = _add.StateRegistration.Type,
                        StateRegistrationNumber = _add.StateRegistration.Number,
                        ByWhomIssued = _add.StateRegistration.ByWhomIssued,

                        NaturalPersons = new List<DAL.NaturalPerson>()
                    };

                    var firstPerson = new DAL.NaturalPerson()
                    {
                        FirstName = _add.FirstPerson.FirstName,
                        LastName = _add.FirstPerson.LastName,
                        Patronymic = _add.FirstPerson.Patronymic,
                        DateOfBirth = _add.FirstPerson.DateOfBirth,
                    };

                    var secondPerson = new DAL.NaturalPerson()
                    {
                        FirstName = _add.SecondPerson.FirstName,
                        LastName = _add.SecondPerson.LastName,
                        Patronymic = _add.SecondPerson.Patronymic,
                        DateOfBirth = _add.SecondPerson.DateOfBirth,
                    };

                    property.NaturalPersons.Add(firstPerson);
                    property.NaturalPersons.Add(secondPerson);
                    await _applicationDBContext.AddAsync(property);
                    await _applicationDBContext.SaveChangesAsync();

                    _add.IsReturn = false;

                    return RedirectToAction("Index", "Properties");
                }
            }
            catch (FormatException)
            {
                ModelState.AddModelError("RoomId", "Введите корректное значение.");
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("RoomId", "Введите значение.");
            }

            _add.IsReturn = true;

            var serializedModel = HttpContext.Session.GetString("IndexPropertiesModel");
            var model = JsonConvert.DeserializeObject<IndexPropertiesModel>(serializedModel);

            var indexRooms = model.AddProperties.AddFullProperty.Rooms;

            model.AddProperties.AddFullProperty = new AddFullPropertyModel();
            model.AddProperties.AddSharedProperty = new AddSharedPropertyModel();
            model.AddProperties.AddJointProperty = _add;

            model.AddProperties.AddFullProperty.Rooms = indexRooms;
            model.AddProperties.AddSharedProperty.Rooms = indexRooms;
            model.AddProperties.AddJointProperty.Rooms = indexRooms;

            serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexPropertiesModel", serializedModel);

            return View("Index", model);
        }
    }
}