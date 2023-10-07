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
        public IActionResult Index()
        {
            return View();
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
        public IActionResult Add(AddMeetingModel model)
        {
            return RedirectToAction("Index");
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
