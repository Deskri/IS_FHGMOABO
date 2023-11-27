using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("MeetingResults")]
    public class MeetingResult
    {
        /// <summary>
        /// Идентификатор результат собрания
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с Question
        /// </summary>
        [Required]
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Question
        /// </summary>
        public Question? Question { get; set; }

        /// <summary>
        /// Общая площадь проголосовавших "За" по вопросу
        /// </summary>
        [Required]
        public decimal AreaFor { get; set; }

        /// <summary>
        /// Общая площадь проголосовавших "Против" по вопросу
        /// </summary>
        [Required]
        public decimal AreaAgainst { get; set; }

        /// <summary>
        /// Общая площадь проголосовавших "Воздержался" по вопросу
        /// </summary>
        [Required]
        public decimal AreaAbstained { get; set; }
    }
}
