using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("ArchivalInformationOfMeetings")]
    public class ArchivalInformationOfMeeting
    {
        /// <summary>
        /// Идентификатор архивной информации
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
        public Meeting? Meeting { get; set; }

        /// <summary>
        /// Общая площадь МКД
        /// </summary>
        [Required]
        public decimal TotalAreaHouse { get; set; }

        /// <summary>
        /// Общая площадь жилой площади в собственности
        /// </summary>
        [Required]
        public decimal ResidentialAreaInOwnership { get; set; }

        /// <summary>
        /// Общая площадь жилой площади не в собственности
        /// </summary>
        [Required]
        public decimal ResidentialAreaInNonOwnership { get; set; }

        /// <summary>
        /// Общая площадь нежилой площади
        /// </summary>
        [Required]
        public decimal NonresidentialArea { get; set; }

        /// <summary>
        /// Колличество собственников принявших участие в собрании
        /// </summary>
        [Required]
        public int OwnersParticipated { get; set; }

        /// <summary>
        /// Площадь собственников принявших участие в собрании
        /// </summary>
        [Required]
        public decimal ParticipatingArea { get; set; }
    }
}
