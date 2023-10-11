using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.DAL
{
    [Table("CountingCommitteeMembers")]
    public class CountingCommitteeMember
    {
        /// <summary>
        /// Идентификатор члена счетной комиссии
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
        [InverseProperty("CountingCommitteeMembers")]
        public Meeting? Meeting { get; set; }

        /// <summary>
        /// ФИО члена счетной комиссии
        /// </summary>
        [Required]
        public string FullName { get; set; }
    }
}
