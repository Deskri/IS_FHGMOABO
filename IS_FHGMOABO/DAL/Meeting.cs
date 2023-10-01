using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("Meetings")]
    public class Meeting
    {
        /// <summary>
        /// Идентификатор собраний
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с House
        /// </summary>
        [ForeignKey("House")]
        public int? HouseId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с House
        /// </summary>
        [InverseProperty("Meetings")]
        public House? House { get; set; }

        /// <summary>
        /// Дата создания собрания
        /// </summary>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Формат собрания
        /// </summary>
        [Required]
        public string Format { get; set; }

        /// <summary>
        /// Статус собрания
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// Секретарь собрания
        /// </summary>
        [Required]
        public string Secretary { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Chairperson
        /// </summary>
        public ICollection<Chairperson>? Chairpersons { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Chairperson
        /// </summary>
        public ICollection<CountingCommitteeMember>? CountingCommitteeMembers { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Chairperson
        /// </summary>
        public ICollection<Question>? Questions { get; set; }
    }
}
