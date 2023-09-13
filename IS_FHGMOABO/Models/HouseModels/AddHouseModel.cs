using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.HouseModels
{
    public class AddHouseModel
    {
        /// <summary>
        /// Номер дома
        /// </summary>
        [Required(ErrorMessage = "Требуется внести номер дома")]
        [Display(Name = "Номер дома")]
        public string Number { get; set; }

        /// <summary>
        /// Тип улицы
        /// </summary>
        [Required(ErrorMessage = "Требуется внести тип улицы")]
        [Display(Name = "Тип улицы")]
        public Type type { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        [Required(ErrorMessage = "Требуется внести улицу")]
        [Display(Name = "Улица")]
        public string Street { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        [Display(Name = "Район (необязательно)")]
        public string? Region { get; set; }

        /// <summary>
        /// Населеный пункт
        /// </summary>
        [Required(ErrorMessage = "Требуется внести населеный пункт")]
        [Display(Name = "Населеный пункт")]
        public string InhabitedLocality { get; set; }

        /// <summary>
        /// Округ
        /// </summary>
        [Display(Name = "Округ (необязательно)")]
        public string? District { get; set; }

        /// <summary>
        /// Субъект
        /// </summary>
        [Display(Name = "Субъект")]
        public string Subject { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        [Required(ErrorMessage = "Требуется внести страну")]
        [Display(Name = "Страна")]
        public string Country { get; set; }

        /// <summary>
        /// Кадастровый номер дома
        /// </summary
        [Display(Name = "Кадастровый номер дома (необязательно)")]
        public string? HouseCadastralNumber { get; set; }

        /// <summary>
        /// Кадастровый номер участка
        /// </summary>
        [Display(Name = "Кадастровый номер участка (необязательно)")]
        public string? PlotCadastralNumber { get; set; }

        /// <summary>
        /// Паспортная площадь дома
        /// </summary>
        [Required(ErrorMessage = "Требуется внести паспортную площадь дома")]
        [Display(Name = "Паспортная площадь дома")]
        public decimal HousesPassportedFloorArea { get; set; }

        /// <summary>
        /// Паспортная площадь участка
        /// </summary>
        [Display(Name = "Паспортная площадь участка (необязательно)")]
        public decimal? PlotPassportedFloorArea { get; set; }

        /// <summary>
        /// Площадь жилых помещений
        /// </summary>
        [Required(ErrorMessage = "Требуется внести площадь жилых помещений")]
        [Display(Name = "Площадь жилых помещений")]
        public decimal ResidentialPremisesPassportedArea { get; set; }

        /// <summary>
        /// Площадь нежилых помещений
        /// </summary>
        [Required(ErrorMessage = "Требуется внести площадь нежилых помещений")]
        [Display(Name = "Площадь нежилых помещений")]
        public decimal NonResidentialPremisesPassportedArea { get; set; }

        public enum Type
        {
            улица,
            бульвар,
            дорога,
            магистраль,
            набережная,
            переулок,
            площадь,
            проспект,
            тупик,
            шоссе,
        }
    }
}
