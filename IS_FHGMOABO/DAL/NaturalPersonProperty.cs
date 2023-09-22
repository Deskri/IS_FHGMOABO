using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("NaturalPersonProperty")]
    public class NaturalPersonProperty
    {
        public int NaturalPersonsId { get; set; }
        public NaturalPerson NaturalPerson { get; set; }

        public int PropertiesId { get; set; }
        public Property Property { get; set; }
    }
}
