using IS_FHGMOABO.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;

namespace IS_FHGMOABO.Models.HomeModels
{
    public class IndexModel
    {
        public List<Meeting> СurrentMeetings { get; set; }
        public int AdoptedMeetingsCount { get; set; }
        public int UnacceptedMeetingsCount { get; set; }
        public List<House> Houses { get; set; }
        public List<DateAndCount> MeetingsDuringTheYears { get; set; }
    }

    public class DateAndCount
    {
        public int Date { get; set; }
        public int Count { get; set; }
    }
}
