using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Models.MeetingsModels
{
    public class VotingResultsModel
    {
        public List<UpdateBulletin>? Bulletins { get; set; }
        public List<string> TableTitles { get; set; } = new List<string>()
        {
            "Квартира / помещение",
            "Собственник",
            "Площадь в собственности",
        };

        public int MeetingID { get; set; }

        public class UpdateBulletin : Bulletin
        {
            public List<VotingResult>? UpdateVotingResults { get; set; }
        }
    }
}
