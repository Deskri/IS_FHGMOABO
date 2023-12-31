﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace IS_FHGMOABO.DAL
{
    [Table("Questions")]
    public class Question
    {
        /// <summary>
        /// Идентификатор вопроса голосования
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
        [InverseProperty("Questions")]
        public Meeting? Meeting { get; set; }

        /// <summary>
        /// Порядковый номер вопроса
        /// </summary>
        [Required]
        public int Number { get; set; }

        /// <summary>
        /// Повестка дня
        /// </summary>
        [Required]
        public string Agenda { get; set; }

        /// <summary>
        /// Предложено
        /// </summary>
        [Required]
        public string Proposed { get; set; }

        /// <summary>
        /// Тип вопроса для принятия вопроса
        /// </summary>
        [Required]
        public short DecisionType { get; set; }

        /// <summary>
        /// Номер приложения
        /// </summary>
        public int? AttachmentNumber { get; set; }

        /// <summary>
        /// Приложение
        /// </summary>
        public string? Attachment { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с VotingResult
        /// </summary>
        public ICollection<VotingResult>? VotingResults { get; set; }

        public MeetingResult? MeetingResult { get; set; }
    }
}
