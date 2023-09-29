using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.PropertiesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Index(PropertiesFilter filter)
        {
            var model = new IndexPropertiesModel();

            model.AddProperties = new AddPropertiesModel();

            model.AddProperties.AddFullProperty = new AddFullPropertyModel();
            model.AddProperties.AddSharedProperty = new AddSharedPropertyModel();
            model.AddProperties.AddJointProperty = new AddJointPropertyModel();

            var rooms = await _applicationDBContext.Rooms
                                            .Where(x => x.Deleted == null)
                                            .Include(x => x.House)
                                            .OrderBy(x => x.House.Type)
                                            .ThenBy(x => x.House.Street)
                                            .ThenBy(x => x.House.Number)
                                            .ThenBy(x => x.Type)
                                            .ThenBy(x => x.Number)
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
            if (properties != null)
            {
                properties = properties.OrderBy(x => x.Room.House.Type)
                                        .ThenBy(x => x.Room.House.Street)
                                        .ThenBy(x => x.Room.House.Number)
                                        .ThenBy(x => x.Room.Type)
                                        .ThenBy(x => int.Parse(Regex.Match(x.Room.Number, @"\d+").Value))
                                        .ThenBy(x => Regex.Match(x.Room.Number, @"\D+").Value)
                                        .ToList();
            }

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

            if (filter.HouseId != null)
            {
                properties = properties.Where(x => x.Room.HouseId == filter.HouseId).ToList();
            }

            if (filter.Room != null)
            {
                properties = properties.Where(x => x.Room.Number == filter.Room).ToList();
            }

            if (filter.PropertyType != null)
            {
                properties = properties.Where(x => x.Type == filter.PropertyType).ToList();
            }

            model.Filter = new PropertiesFilter();
            model.Filter.Houses = await _applicationDBContext.Houses.ToListAsync();

            model.Properties = properties;

            var serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("IndexPropertiesModel", serializedModel);

            return View("Index", model);
        }

        public async Task<IActionResult> Delete(int id)
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
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _applicationDBContext.Properties
                                        .Include(x => x.NaturalPersons)
                                        .Include(x => x.LegalPerson)
                                        .Include(x => x.Room.House)
                                        .FirstOrDefaultAsync(x => x.Id == id);

            var rooms = await _applicationDBContext.Rooms
                                .Where(x => x.Deleted == null)
                                .Include(x => x.House)
                                .OrderBy(x => x.House.Type)
                                .ThenBy(x => x.House.Street)
                                .ThenBy(x => x.House.Number)
                                .ThenBy(x => x.Type)
                                .ThenBy(x => x.Number)
                                .ToListAsync();

            // Необходимо для сериализации, чтобы избавиться от зациклености
            foreach (var room in rooms)
            {
                room.House.Rooms = null;
                room.Properties = null;
            }

            var model = new EditPropertiesModel()
            {
                Id = property.Id,
                RoomId = property.RoomId.ToString(),
                Rooms = rooms,
                NaturalPerson = new List<Models.PropertiesModels.EditNaturalPerson>(),
                Type = property.Type,
                Share = property.Share * 100,
                EditShare = new Share(),
                PropertyDate = new PropertyDate()
                {
                    DateOfTaking = property.DateOfTaking,
                    EndDate = property.EndDate,
                },
                StateRegistration = new StateRegistration()
                {
                    Type = property.TypeOfStateRegistration,
                    Number = property.StateRegistrationNumber,
                    ByWhomIssued = property.ByWhomIssued,
                }
            };

            if (property.LegalPerson != null)
            {
                model.LegalPerson = new Models.PropertiesModels.EditLegalPerson()
                {
                    Id = property.LegalPerson.Id,
                    Name = property.LegalPerson.Name,
                };
            }

            foreach (var person in property.NaturalPersons)
            {
                var modelPerson = new Models.PropertiesModels.EditNaturalPerson()
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Patronymic = person.Patronymic,
                    DateOfBirth = person.DateOfBirth,
                };
                model.NaturalPerson.Add(modelPerson);
            }

            var serializedModel = JsonConvert.SerializeObject(model.Rooms);
            HttpContext.Session.SetString("EditRooms", serializedModel);

            return View("Edit", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EditPropertiesModel model)
        {
            try
            {
                var _roomId = Int32.Parse(model.RoomId);

                var rooms = await _applicationDBContext.Rooms.FirstOrDefaultAsync(x => x.Id == _roomId && x.Deleted == null);

                decimal share = 0;

                if (model.Type == "Собственность")
                {
                    if (rooms == null)
                    {
                        ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
                    }
                    else
                    {
                        var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _roomId
                                                                                    && x.EndDate == null
                                                                                    && x.Id != model.Id)
                                                                                    .ToListAsync();
                        share = model.Share / 100;

                        if (properties.Count != 0)
                        {
                            ModelState.AddModelError("Rooms", "В этом помещение уже есть право собственности.");
                        }
                    }
                }
                else
                {
                    if (model.EditShare.Dividend == null && model.EditShare.Divisor == null || model.EditShare.Dividend == 0 && model.EditShare.Divisor == 0)
                    {
                        share = model.Share / 100;
                    }
                    else if (model.EditShare.Dividend < 0)
                    {
                        ModelState.AddModelError("EditShare.Dividend", "Делимое не может быть меньше 0.");
                    }
                    else if (model.EditShare.Divisor < 0)
                    {
                        ModelState.AddModelError("EditShare.Divisor", "Делитель не может быть меньше 0.");
                    }
                    else if (model.EditShare.Dividend == 0 || model.EditShare.Divisor == 0 || model.EditShare.Dividend / model.EditShare.Divisor > 1)
                    {
                        ModelState.AddModelError("EditShare", "Делимое не может быть больше делителя или одно из значение не может быть равное 0.");
                    }
                    else
                    {
                        share = (decimal)model.EditShare.Dividend / model.EditShare.Divisor;
                    }


                    if (rooms == null)
                    {
                        ModelState.AddModelError("RoomId", "Помещение в системе не найдено.");
                    }
                    else if (share > 0 && share <= 1)
                    {
                        var properties = await _applicationDBContext.Properties.Where(x => x.RoomId == _roomId
                                                                                    && x.EndDate == null
                                                                                    && x.Id != model.Id)
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
                }
                if (model.PropertyDate.DateOfTaking > DateTime.Now)
                {
                    ModelState.AddModelError("PropertyDate.DateOfTaking", "Дата вступления в собственность не должна быть больше текущего времени.");
                }
                if (model.NaturalPerson == null)
                {
                    if (model.LegalPerson.Name == null)
                    {
                        ModelState.AddModelError("LegalPerson.Name", "Наименование организации должно быть заполнено.");
                    }
                }
                if (model.NaturalPerson != null)
                {
                    for (var i = 0; i < model.NaturalPerson.Count; i++)
                    {
                        if (model.NaturalPerson[i].FirstName == null)
                        {
                            ModelState.AddModelError($"NaturalPerson[{i}].FirstName", "Имя собственника должно быть заполнено.");
                        }

                        if (model.NaturalPerson[i].LastName == null)
                        {
                            ModelState.AddModelError($"NaturalPerson[{i}].LastName", "Фамилия собственника должна быть заполнена.");
                        }

                        if (model.NaturalPerson[i].DateOfBirth > DateTime.Now)
                        {
                            ModelState.AddModelError($"NaturalPerson[{i}].DateOfBirth", "Дата рождения не должна быть больше текущего времени.");
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    var property = await _applicationDBContext.Properties
                                                              .Include(x => x.NaturalPersons)
                                                              .Include(x => x.LegalPerson)
                                                              .FirstOrDefaultAsync(x => x.Id == model.Id);


                    property.RoomId = _roomId;
                    property.Type = model.Type;
                    property.Share = share;
                    property.DateOfTaking = model.PropertyDate.DateOfTaking;
                    property.EndDate = model.PropertyDate.EndDate;
                    property.TypeOfStateRegistration = model.StateRegistration.Type;
                    property.StateRegistrationNumber = model.StateRegistration.Number;
                    property.ByWhomIssued = model.StateRegistration.ByWhomIssued;

                    if (model.NaturalPerson == null)
                    {
                        property.LegalPerson.Name = model.LegalPerson.Name;
                    }
                    else if (model.NaturalPerson != null && model.NaturalPerson.Count > 0)
                    {
                        var i = 0;
                        foreach (var person in property.NaturalPersons)
                        {
                            person.FirstName = model.NaturalPerson[i].FirstName;
                            person.LastName = model.NaturalPerson[i].LastName;
                            person.Patronymic = model.NaturalPerson[i].Patronymic;
                            person.DateOfBirth = model.NaturalPerson[i].DateOfBirth;
                            i++;
                        }
                    }

                    await _applicationDBContext.SaveChangesAsync();

                    return RedirectToAction("Index");
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
            var serializedModel = HttpContext.Session.GetString("EditRooms");
            var deserializeModel = JsonConvert.DeserializeObject<List<Room>>(serializedModel);

            model.Rooms = deserializeModel;

            serializedModel = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("EditRooms", serializedModel);

            return View("Edit", model);
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