using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class IndexPropertiesModel
    {
        public AddPropertiesModel AddProperties { get; set; }

        public List<Property> Properties { get; set; }

        public PropertiesFilter Filter { get; set; }
    }

    public class PropertiesFilter
    {
        List<House> Houses { get; set; }

        public int HouseId { get; set; }

        public string Room { get; set; }

        public string PropertyType { get; set; }
    }
}
