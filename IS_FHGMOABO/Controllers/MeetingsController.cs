using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.MeetingsModels;
using IS_FHGMOABO.Services.Meetings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                            Percentage = model.Meeting.Questions[i].Percentage,
                        });
                    }
                    else
                    {
                        meeting.Questions.Add(new Question()
                        {
                            Number = i + 1,
                            Agenda = model.Meeting.Questions[i].Agenda,
                            Proposed = model.Meeting.Questions[i].Proposed,
                            Percentage = model.Meeting.Questions[i].Percentage,
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
    }
}
