using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("LegalPersons")]
    public class LegalPerson
    {
        /// <summary>
        /// Идентификатор юридического лица
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Наименование организации
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с собственностью
        /// </summary>
        public ICollection<Property> Properties { get; set; }
    }
}
