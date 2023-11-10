using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("VotingResults")]
    public class VotingResult
    {
        /// <summary>
        /// Идентификатор результата голосования
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Question
        /// </summary>
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Question
        /// </summary>
        [InverseProperty("VotingResults")]
        public Question? Question { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Question
        /// </summary>
        [ForeignKey("Bulletin")]
        public int BulletinId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Question
        /// </summary>
        [InverseProperty("VotingResults")]
        public Bulletin? Bulletin { get; set; }

        /// <summary>
        /// Результат голосования
        /// </summary>
        public int? Result { get; set; }
    }
}
