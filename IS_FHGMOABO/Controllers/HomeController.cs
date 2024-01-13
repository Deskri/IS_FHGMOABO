using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models;
using IS_FHGMOABO.Models.HomeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IS_FHGMOABO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public HomeController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var meeting = await _applicationDBContext.Meetings.Include(x => x.ArchivalInformationOfMeeting)
                                                              .Include(x => x.House)
                                                              .ToListAsync();

            var _currentMeetings = meeting.Where(x => x.Status != "Завершено").OrderBy(x => x.StartDate).ToList();

            var _house = await _applicationDBContext.Houses.Include(x => x.Rooms)
                                                           .ToListAsync();
            _house.OrderByDescending(x => x.Id);

            var _adoptedMeetings = meeting.Where(x => x.Status == "Завершено" 
            && x.ArchivalInformationOfMeeting.TotalAreaHouse/2 <= x.ArchivalInformationOfMeeting.ParticipatingArea).Count();
            var _unacceptedMeetings = meeting.Where(x => x.Status == "Завершено"
            && x.ArchivalInformationOfMeeting.TotalAreaHouse / 2 > x.ArchivalInformationOfMeeting.ParticipatingArea).Count();

            var _meetingsDuringTheYears = new List<DateAndCount>();
            for (int i = 0; i < 3; i++)
            {
                var date = DateTime.Now.Year - i;
                var count = meeting.Where(x => x.StartDate.Year == date).Count();
                _meetingsDuringTheYears.Add(new DateAndCount()
                {
                    Date = date,
                    Count = count,
                });
            }

            _meetingsDuringTheYears = _meetingsDuringTheYears.OrderBy(x => x.Date).ToList();

            var model = new IndexModel()
            {
                СurrentMeetings = _currentMeetings,
                AdoptedMeetingsCount = _adoptedMeetings,
                UnacceptedMeetingsCount = _unacceptedMeetings,
                Houses = _house,
                MeetingsDuringTheYears = _meetingsDuringTheYears,
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}