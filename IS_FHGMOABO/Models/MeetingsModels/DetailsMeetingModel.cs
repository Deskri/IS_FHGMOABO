using IS_FHGMOABO.DAL;
using System.Diagnostics.Contracts;

namespace IS_FHGMOABO.Models.MeetingsModels
{
    public class DetailsMeetingModel
    {
        public Meeting? Meeting { get; set; }
        public List<QuestionResult>? QuestionResults { get; set; } = new List<QuestionResult>();
        public class QuestionResult
        {
            public int Number { get; set; }
            public string? Agenda { get; set; }
            public Responses For { get; set; } = new Responses();
            public Responses Against { get; set; } = new Responses();
            public Responses Abstained { get; set; } = new Responses();
        }

        public class Responses
        {
            public decimal Percentage { get; set; } = 0;
            public decimal Area { get; set; } = 0;
        }
    }
}
