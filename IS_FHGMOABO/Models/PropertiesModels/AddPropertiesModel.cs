namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class AddPropertiesModel
    {
        public AddFullPropertyNaturalPersonModel AddFullPropertyNaturalPerson { get; set; }
    }

    public class AddFullPropertyNaturalPersonModel
    {
        public int RoomId { get; set; }

        public NaturalPerson NaturalPerson { get; set; }

        public PropertyDate PropertyDate { get; set; }

        public StateRegistration StateRegistration { get; set; }
    }
}
