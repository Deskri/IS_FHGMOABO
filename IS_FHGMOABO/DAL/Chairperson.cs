using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("Chairpersons")]
    public class Chairperson
    {
        /// <summary>
        /// Идентификатор председателя
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Meeting
        /// </summary>
        [Required]
        [ForeignKey("Meeting")]
        public int MeetingId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Meeting
        /// </summary>
        [InverseProperty("Chairpersons")]
        public Meeting? Meeting { get; set; }

        /// <summary>
        /// ФИО председателя
        /// </summary>
        [Required]
        public string FullName { get; set; }
    }
}
