using IS_FHGMOABO.DAL;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class EditPropertiesModel
    {
        public int Id { get; set; }

        [Display(Name = "Юридическое лицо")]
        public EditLegalPerson? LegalPerson { get; set; }

        [Display(Name = "Физическое лицо")]
        public List<EditNaturalPerson>? NaturalPerson { get; set; }

        public string Type { get; set; }

        [Display(Name = "Адрес")]
        public string? RoomId { get; set; }

        public List<Room>? Rooms { get; set; }

        public PropertyDate? PropertyDate { get; set; }

        public StateRegistration? StateRegistration { get; set; }

        public decimal Share {  get; set; }

        public Share? EditShare { get; set; }
    }

    public class EditLegalPerson : LegalPerson
    {
        public int Id { get; set; }
    }

    public class EditNaturalPerson : NaturalPerson
    {
        public int Id { get; set; }
    }
}
