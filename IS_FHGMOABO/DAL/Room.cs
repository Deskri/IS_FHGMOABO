using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("Rooms")]
    public class Room
    {
        /// <summary>
        /// Идентификатор помещения
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с House
        /// </summary>
        [Required]
        [ForeignKey("House")]
        public int HouseId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с House
        /// </summary>
        [InverseProperty("Rooms")]
        public House House { get; set; }

        /// <summary>
        /// Тип помещения
        /// </summary>
        [Required]
        [MaxLength(127)]
        public string Type { get; set; }

        /// <summary>
        /// Номер помещения
        /// </summary>
        [Required]
        public string Number { get; set; }

        /// <summary>
        /// Назначение
        /// </summary>
        [Required]
        public string Purpose { get; set; }

        /// <summary>
        /// Общая площадь
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalArea { get; set; }

        /// <summary>
        /// Жилая площадь
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LivingArea { get; set; }

        /// <summary>
        /// Полезная площадь
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UsableArea { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        public int? Floor { get; set; }

        /// <summary>
        /// Подъезд
        /// </summary>
        public int? Entrance { get; set; }

        /// <summary>
        /// Кадастровый номер
        /// </summary>
        public string? CadastralNumber { get; set; }

        /// <summary>
        /// Приватизировано
        /// </summary>
        [Required]
        public bool IsPrivatized { get; set; }

        /// <summary>
        /// Дата добавления или изменения
        /// </summary>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Дата удаление или сохранения старых данных
        /// </summary>
        public DateTime? Deleted { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с Property
        /// </summary>
        public ICollection<Property> Properties { get; set; }
    }
}
