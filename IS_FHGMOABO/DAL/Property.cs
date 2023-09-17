using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.DAL
{
    [Table("Properties")]
    public class Property
    {
        /// <summary>
        /// Идентификатор собственности
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с помещением
        /// </summary>
        [Required]
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с помещением
        /// </summary>
        [InverseProperty("Properties")]
        public Room Room { get; set; }

        /// <summary>
        /// Внешний ключ для связи с юридическим лицом
        /// </summary>
        [ForeignKey("LegalPerson")]
        public int LeagalPersonId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с юридическим лицом
        /// </summary>
        [InverseProperty("Properties")]
        public LegalPerson LegalPerson { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с физискими лицами
        /// </summary>
        public ICollection<NaturalPerson> NaturalPersons { get; set; }

        /// <summary>
        /// Вид собственности
        /// </summary>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Доля
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10, 9)")]
        public decimal Share { get; set; }

        /// <summary>
        /// Дата вступления в собственность
        /// </summary>
        [Required]
        public DateTime DateOfTaking { get; set; }

        /// <summary>
        /// Дата окончания собственности
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Вид государственной регистрации
        /// </summary>
        public string TypeOfStateRegistration { get; set; }

        /// <summary>
        /// Номер государственной регистрации
        /// </summary>
        public string StateRegistrationNumber { get; set; }

        /// <summary>
        /// Кем выдано право
        /// </summary>
        public string ByWhomIssued { get; set; }
    }
}
