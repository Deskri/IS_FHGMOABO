﻿using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class AddPropertiesModel
    {
        public AddFullPropertyModel AddFullProperty { get; set; }

        public AddSharedPropertyModel AddSharedProperty { get; set; }

        public AddJointPropertyModel AddJointProperty {  get; set; } 
    }

    public class AddFullPropertyModel : AddProperty
    {
        public bool IsNaturalPerson { get; set; }

        public LegalPerson? LegalPerson { get; set; }

        public NaturalPerson? NaturalPerson { get; set; }
    }

    public class AddSharedPropertyModel : AddProperty
    {
        public bool IsNaturalPerson { get; set; }

        public LegalPerson? LegalPerson { get; set; }

        public NaturalPerson? NaturalPerson { get; set; }

        public Share? Share { get; set; }
    }

    public class AddJointPropertyModel : AddProperty
    {
        public LegalPerson? FirstPerson { get; set; }

        public LegalPerson? SecondPerson { get; set; }

        public Share? Share { get; set; }
    }

    public class AddProperty
    {
        public string? RoomId { get; set; }

        public PropertyDate? PropertyDate { get; set; }

        public StateRegistration? StateRegistration { get; set; }

        public List<Room>? Rooms { get; set; }
    }
}
