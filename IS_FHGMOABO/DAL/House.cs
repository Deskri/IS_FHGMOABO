using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS_FHGMOABO.DAL
{
    [Table("Houses")]
    public class House
    {
        /// <summary>
        /// Идентификатор дома
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Номер дома
        /// </summary>
        [Required]
        public string Number { get; set; }

        /// <summary>
        /// Тип улицы
        /// </summary>
        [Required]
        [MaxLength(127)]
        public string Type { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        [Required]
        public string Street { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Населеный пункт
        /// </summary>
        [Required]
        public string InhabitedLocality { get; set; }

        /// <summary>
        /// Округ
        /// </summary>
        public string? District { get; set; }

        /// <summary>
        /// Субъект
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Кадастровый номер дома
        /// </summary>
        public string? HouseCadastralNumber { get; set; }

        /// <summary>
        /// Кадастровый номер участка
        /// </summary>
        public string? PlotCadastralNumber { get; set; }

        /// <summary>
        /// Паспортная площадь дома
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal HousesPassportedFloorArea { get; set; }

        /// <summary>
        /// Паспортная площадь участка
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PlotPassportedFloorArea { get; set; }

        /// <summary>
        /// Площадь жилых помещений
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ResidentialPremisesPassportedArea { get; set; }

        /// <summary>
        /// Площадь нежилых помещений
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal NonResidentialPremisesPassportedArea { get; set; }

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
        /// Навигационное свойство для связи с Room
        /// </summary>
        public ICollection<Room> Rooms { get; set; }
    }
}
