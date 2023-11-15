using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Models.MeetingsModels
{
    public class VotingResultsModel
    {
        public List<Bulletin>? Bulletins { get; set; }
        public List<string> TableTitles { get; set; } = new List<string>() 
        {
            "Квартира / помещение",
            "Собственник",
            "Площадь в собственности",
        };
    }
}
