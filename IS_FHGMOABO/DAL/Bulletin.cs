using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.DAL
{
    [Table("Bulletins")]
    public class Bulletin
    {
        /// <summary>
        /// Идентификатор бюллетеня
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Meeting
        /// </summary>
        [ForeignKey("Meeting")]
        public int MeetingId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Meeting
        /// </summary>
        [InverseProperty("Bulletins")]
        public Meeting? Meeting { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Room
        /// </summary>
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Room
        /// </summary>
        [InverseProperty("Bulletins")]
        public Room? Room { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Property
        /// </summary>
        [ForeignKey("Property")]
        public int? PropertyId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Property
        /// </summary>
        [InverseProperty("Bulletins")]
        public Property? Property { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с VotingResult
        /// </summary>
        public ICollection<VotingResult>? VotingResults { get; set; }
    }
}
