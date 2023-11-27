using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.MeetingsModels;
using IS_FHGMOABO.Services.Meetings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static IS_FHGMOABO.Models.MeetingsModels.VotingResultsModel;

namespace IS_FHGMOABO.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public MeetingsController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _applicationDBContext.Meetings
                                                   .Include(x => x.House)
                                                   .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddMeetingModel()
            {
                Houses = await _applicationDBContext.Houses.ToListAsync(),
            };

            MeetingsHelpers.SerializeHouses(model, HttpContext);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddMeetingModel model)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            MeetingsHelpers.ValidationAddMeeting(model, ModelState);

            if (ModelState.IsValid)
            {
                var meeting = new Meeting()
                {
                    HouseId = model.Meeting.HouseId,
                    StartDate = model.Meeting.StartDate,
                    Format = AddMeeting.GetEnumDescription(model.Meeting.Format),
                    Status = "Создано",
                    Secretary = $"{model.Meeting.Secretary.LastName} {model.Meeting.Secretary.FirstName} {model.Meeting.Secretary.Patronymic}",
                    Chairperson = $"{model.Meeting.Chairperson.LastName} {model.Meeting.Chairperson.FirstName} {model.Meeting.Chairperson.Patronymic}",
                };

                meeting.CountingCommitteeMembers = new List<CountingCommitteeMember>();

                foreach (var countingCommitteeMember in model.Meeting.CountingCommitteeMembers)
                {
                    meeting.CountingCommitteeMembers.Add(new CountingCommitteeMember()
                    {
                        FullName = $"{countingCommitteeMember.LastName} {countingCommitteeMember.FirstName} {countingCommitteeMember.Patronymic}",
                    });
                }

                meeting.Questions = new List<Question>();

                int attachmentNumber = 1;

                for (int i = 0; i < model.Meeting.Questions.Count; i++)
                {
                    if (model.Meeting.Questions[i].AttachmentString == null)
                    {
                        meeting.Questions.Add(new Question()
                        {
                            Number = i + 1,
                            Agenda = model.Meeting.Questions[i].Agenda,
                            Proposed = model.Meeting.Questions[i].Proposed,
                            DecisionType = model.Meeting.Questions[i].DecisionType,
                        });
                    }
                    else
                    {
                        meeting.Questions.Add(new Question()
                        {
                            Number = i + 1,
                            Agenda = model.Meeting.Questions[i].Agenda,
                            Proposed = model.Meeting.Questions[i].Proposed,
                            DecisionType = model.Meeting.Questions[i].DecisionType,
                            Attachment = model.Meeting.Questions[i].AttachmentString,
                            AttachmentNumber = attachmentNumber,
                        });

                        attachmentNumber++;
                    }
                }

                await _applicationDBContext.AddAsync(meeting);
                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCountingCommitteeMember(AddMeetingModel model)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            if (model.Meeting.CountingCommitteeMembers == null)
            {
                model.Meeting.CountingCommitteeMembers = new List<AddCountingCommitteeMember>();
            }

            model.Meeting.CountingCommitteeMembers.Add(new AddCountingCommitteeMember());

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteCountingCommitteeMember(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.CountingCommitteeMembers.RemoveAt(index);

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddQuestion(AddMeetingModel model)
        {
            if (model.Meeting.Questions == null)
            {
                model.Meeting.Questions = new List<AddQuestion>();
            }

            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions.Add(new AddQuestion());

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteQuestion(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions.RemoveAt(index);

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveAttachment(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions[index].AttachmentString = null;
            model.Meeting.Questions[index].AttachmentName = null;

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = new DetailsMeetingModel();

            model.Meeting = await _applicationDBContext.Meetings
                                                   .Where(x => x.Id == id)
                                                   .Include(x => x.Questions)
                                                   .Include(x => x.CountingCommitteeMembers)
                                                   .Include(x => x.House)
                                                   .FirstOrDefaultAsync();

            var totalHouseArea = model.Meeting.House.ResidentialPremisesPassportedArea + model.Meeting.House.NonResidentialPremisesPassportedArea;

            foreach (var question in model.Meeting.Questions)
            {
                var results = await _applicationDBContext.VotingResults
                                                         .Where(x => x.QuestionId == question.Id && (x.Result == 0 || x.Result == 1 || x.Result == 2))
                                                         .Include(x => x.Bulletin.Room)
                                                         .Include(x => x.Bulletin.Property)
                                                         .ToListAsync();

                model.QuestionResults.Add(MeetingsHelpers.ResponseСounter(results, question, totalHouseArea));
            }

            return View("Details", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Bulletins(int id)
        {
            var model = await _applicationDBContext.Bulletins
                                                       .Where(x => x.MeetingId == id)
                                                       .Include(x => x.Meeting)
                                                       .Include(x => x.Meeting.Questions)
                                                       .Include(x => x.Property)
                                                       .Include(x => x.Property.LegalPerson)
                                                       .Include(x => x.Property.NaturalPersons)
                                                       .Include(x => x.Room)
                                                       .Include(x => x.Room.House)
                                                       .ToListAsync();

            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Questions)
                                                     .FirstOrDefaultAsync();

            ViewData["MeetingId"] = id;
            ViewData["HeaderPage"] = $"Бюллетени общего собрания собственников от {meeting.StartDate.ToString("dd.MM.yyyy")} по адресу: {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}";

            return View("Bulletins", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> VotingResults(int id)
        {
            var model = new VotingResultsModel();

            var bulletins = await _applicationDBContext.Bulletins
                                                   .Where(x => x.MeetingId == id)
                                                   .Include(x => x.Property)
                                                   .Include(x => x.Property.LegalPerson)
                                                   .Include(x => x.Property.NaturalPersons)
                                                   .Include(x => x.Room)
                                                   .Include(x => x.Room.House)
                                                   .ToListAsync();

            var meeting = await _applicationDBContext.Meetings
                                         .Where(x => x.Id == id)
                                         .Include(x => x.House)
                                         .Include(x => x.Questions)
                                         .FirstOrDefaultAsync();

            model.MeetingID = id;
            ViewData["HeaderPage"] = $"Подсчет голосов общего собрания собственников от {meeting.StartDate.ToString("dd.MM.yyyy")} по адресу: {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}";

            foreach (var question in meeting.Questions)
            {
                model.TableTitles.Add($"Вопрос {question.Number}");
            }

            model.Bulletins = new List<UpdateBulletin>();

            foreach (var bulletin in bulletins)
            {
                var results = await _applicationDBContext.VotingResults.Where(x => x.BulletinId == bulletin.Id).ToListAsync();

                model.Bulletins.Add(new UpdateBulletin()
                {
                    Id = bulletin.Id,
                    MeetingId = bulletin.MeetingId,
                    RoomId = bulletin.RoomId,
                    PropertyId = bulletin.PropertyId,
                    Meeting = bulletin.Meeting,
                    Room = bulletin.Room,
                    Property = bulletin.Property,
                    UpdateVotingResults = results,
                });
            }

            return View("VotingResults", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VotingResults(VotingResultsModel model)
        {
            foreach (var bulletin in model.Bulletins)
            {
                foreach (var updateVotingResult in bulletin.UpdateVotingResults)
                {
                    var _votingResults = await _applicationDBContext.VotingResults
                                                                    .Where(x => x.Id == updateVotingResult.Id)
                                                                    .FirstOrDefaultAsync();

                    _votingResults.Result = updateVotingResult.Result;

                    await _applicationDBContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", new { id = model.MeetingID });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var question = await _applicationDBContext.Questions
                                                      .Include(x => x.Meeting)
                                                      .Where(x => x.Id == id)
                                                      .FirstOrDefaultAsync();

            byte[] byteArray = Convert.FromBase64String(question.Attachment);
            MemoryStream memoryStream = new MemoryStream(byteArray);

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Приложение №{question.AttachmentNumber} ОCС от {question.Meeting.StartDate.ToString("dd.MM.yyyy")}.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNotification(int id)
        {
            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Questions)
                                                     .FirstOrDefaultAsync();

            var memoryStream = MeetingsDocuments.MeetingNotification(meeting);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Уведомление о проведении ОСС.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateVotingRegister(int id)
        {
            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Bulletins)
                                                     .FirstOrDefaultAsync();

            meeting.House.Rooms = await _applicationDBContext.Rooms
                                                             .Where(x => x.HouseId == meeting.HouseId)
                                                             .Include(x => x.Properties)
                                                             .ToListAsync();

            if (meeting.Bulletins == null || meeting.Bulletins.Count == 0)
            {
                foreach (var room in meeting.House.Rooms)
                {
                    if (!room.IsPrivatized)
                    {
                        meeting.Bulletins.Add(new Bulletin()
                        {
                            RoomId = room.Id,
                        });
                    }
                    else
                    {
                        foreach (var property in room.Properties)
                        {
                            if (property.EndDate == null)
                            {
                                meeting.Bulletins.Add(new Bulletin()
                                {
                                    RoomId = room.Id,
                                    PropertyId = property.Id,
                                });
                            }
                        }
                    }
                }
            }
            await _applicationDBContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateBulletins(int id)
        {
            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Bulletins)
                                                     .FirstOrDefaultAsync();

            meeting.House.Rooms = await _applicationDBContext.Rooms
                                                             .Where(x => x.HouseId == meeting.HouseId)
                                                             .Include(x => x.Properties)
                                                             .ToListAsync();

            if (meeting.Bulletins == null || meeting.Bulletins.Count == 0)
            {
                foreach (var room in meeting.House.Rooms)
                {
                    if (!room.IsPrivatized)
                    {
                        meeting.Bulletins.Add(new Bulletin()
                        {
                            RoomId = room.Id,
                        });
                    }
                    else
                    {
                        foreach (var property in room.Properties)
                        {
                            if (property.EndDate == null)
                            {
                                meeting.Bulletins.Add(new Bulletin()
                                {
                                    RoomId = room.Id,
                                    PropertyId = property.Id,
                                });
                            }
                        }
                    }
                }
            }
            await _applicationDBContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateVotingResults(int id)
        {
            var bulletins = await _applicationDBContext.Bulletins.Where(x => x.MeetingId == id)
                                                                 .Include(x => x.Meeting.Questions)
                                                                 .ToListAsync();

            foreach (var bulletin in bulletins)
            {
                foreach (var question in bulletin.Meeting.Questions)
                {
                    var result = new VotingResult()
                    {
                        BulletinId = bulletin.Id,
                        QuestionId = question.Id,
                    };

                    await _applicationDBContext.AddAsync(result);
                }
            }

            await _applicationDBContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateMeetingClosing(int id)
        {
            var meeting = await _applicationDBContext.Meetings.Where(x => x.Id == id)
                                                              .Include(x => x.House)
                                                              .Include(x => x.House.Rooms)
                                                              .Include(x => x.ArchivalInformationOfMeeting)
                                                              .Include(x => x.Questions)
                                                              .FirstOrDefaultAsync();

            var bulletins = await _applicationDBContext.Bulletins.Where(x => x.MeetingId == id)
                                                                 .Include(x => x.VotingResults)
                                                                 .Include(x => x.Room)
                                                                 .Include(x => x.Property)
                                                                 .ToListAsync();

            decimal NonOwnershipArea = 0;

            foreach (var room in meeting.House.Rooms)
            {
                if (!room.IsPrivatized)
                {
                    NonOwnershipArea += room.TotalArea;
                }
            }

            int _ownersParticipated = 0;
            decimal _participatingArea = 0;

            foreach (var bulletin in bulletins)
            {
                for (int i = 0; i < bulletin.VotingResults.Count; i++)
                {
                    if (bulletin.VotingResults.ToList()[i].Result != null)
                    {
                        _ownersParticipated++;
                        _participatingArea += bulletin.Room.TotalArea * bulletin.Property.Share;
                        i = bulletin.VotingResults.Count;
                    }
                }
            }

            meeting.ArchivalInformationOfMeeting = new ArchivalInformationOfMeeting()
            {
                MeetingId = id,
                TotalAreaHouse = meeting.House.NonResidentialPremisesPassportedArea + meeting.House.ResidentialPremisesPassportedArea,
                ResidentialAreaInOwnership = meeting.House.ResidentialPremisesPassportedArea - NonOwnershipArea,
                ResidentialAreaInNonOwnership = NonOwnershipArea,
                NonresidentialArea = meeting.House.NonResidentialPremisesPassportedArea,
                OwnersParticipated = _ownersParticipated,
                ParticipatingArea = _participatingArea,
            };

            foreach (var question in meeting.Questions)
            {
                question.MeetingResult = new MeetingResult()
                {
                    AreaFor = 0,
                    AreaAgainst = 0,
                    AreaAbstained = 0,
                };

                foreach (var bulletin in bulletins)
                {
                    var _votingResult = bulletin.VotingResults.Where(x => x.QuestionId == question.Id).FirstOrDefault();
                    switch (_votingResult.Result)
                    {
                        case 1:
                            question.MeetingResult.AreaFor += bulletin.Room.TotalArea * bulletin.Property.Share;
                            break;
                        case 0:
                            question.MeetingResult.AreaAgainst += bulletin.Room.TotalArea * bulletin.Property.Share;
                            break;
                        case 2:
                            question.MeetingResult.AreaAbstained += bulletin.Room.TotalArea * bulletin.Property.Share;
                            break;
                        default: 
                            break;
                    }
                }
            }

            await _applicationDBContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PrintBulletins(int id)
        {
            var bulletins = await _applicationDBContext.Bulletins
                                           .Where(x => x.MeetingId == id)
                                           .Include(x => x.Meeting)
                                           .Include(x => x.Meeting.Questions)
                                           .Include(x => x.Property)
                                           .Include(x => x.Property.LegalPerson)
                                           .Include(x => x.Property.NaturalPersons)
                                           .Include(x => x.Room)
                                           .Include(x => x.Room.House)
                                           .ToListAsync();

            var memoryStream = MeetingsDocuments.MeetingBulletins(bulletins);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Бюллетени общего собрания от {bulletins[0].Meeting.StartDate.ToString("dd.MM.yyyy")} по адресу {bulletins[0].Room.House.Type} {bulletins[0].Room.House.Street}, дом {bulletins[0].Room.House.Number}.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PrintVotingRegister(int id)
        {
            var bulletins = await _applicationDBContext.Bulletins
                                                       .Where(x => x.MeetingId == id)
                                                       .Include(x => x.Meeting)
                                                       .Include(x => x.Meeting.Questions)
                                                       .Include(x => x.Property)
                                                       .Include(x => x.Property.LegalPerson)
                                                       .Include(x => x.Property.NaturalPersons)
                                                       .Include(x => x.Room)
                                                       .Include(x => x.Room.House)
                                                       .ToListAsync();

            var memoryStream = MeetingsDocuments.MeetingVotingRegister(bulletins);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Реестр голосования собственников помещений от {bulletins[0].Meeting.StartDate.ToString("dd.MM.yyyy")} по адресу {bulletins[0].Room.House.Type} {bulletins[0].Room.House.Street}, дом {bulletins[0].Room.House.Number}.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PrintEmptyBulletin(int id)
        {
            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Questions)
                                                     .FirstOrDefaultAsync();

            var memoryStream = MeetingsDocuments.EmptyMeetingBulletin(meeting);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Пустой бюллетень общего собрания от {meeting.StartDate.ToString("dd.MM.yyyy")} по адресу {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PrintBulletin(int id)
        {
            var bulletin = await _applicationDBContext.Bulletins
                                           .Where(x => x.Id == id)
                                           .Include(x => x.Meeting)
                                           .Include(x => x.Meeting.Questions)
                                           .Include(x => x.Property)
                                           .Include(x => x.Property.LegalPerson)
                                           .Include(x => x.Property.NaturalPersons)
                                           .Include(x => x.Room)
                                           .Include(x => x.Room.House)
                                           .FirstOrDefaultAsync();

            var memoryStream = MeetingsDocuments.MeetingBulletin(bulletin);

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Бюллетенm общего собрания от {bulletin.Meeting.StartDate.ToString("dd.MM.yyyy")} по адресу {bulletin.Room.House.Type} {bulletin.Room.House.Street}, дом {bulletin.Room.House.Number} {bulletin.Room.Type} {bulletin.Room.Number}.docx");
        }
    }
}