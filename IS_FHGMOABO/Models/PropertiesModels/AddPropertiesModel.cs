using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class AddPropertiesModel
    {
        public AddFullPropertyModel AddFullProperty { get; set; }
    }

    public class AddFullPropertyModel
    {
        public bool IsNaturalPerson { get; set; }

        public int RoomId { get; set; }

        public LegalPerson? LegalPerson { get; set; }

        public NaturalPerson? NaturalPerson { get; set; }

        public PropertyDate? PropertyDate { get; set; }

        public StateRegistration? StateRegistration { get; set; }

        public List<Room>? Rooms { get; set; }
    }
}
