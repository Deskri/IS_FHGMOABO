using IS_FHGMOABO.DAL;
using System.ComponentModel.DataAnnotations;

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
        public List<House>? Houses { get; set; }

        [Display(Name = "Дом")]
        public int? HouseId { get; set; }

        [Display(Name = "Помещение")]
        public string? Room { get; set; }

        [Display(Name = "Вид собственности")]
        public string? PropertyType { get; set; }
    }
}
