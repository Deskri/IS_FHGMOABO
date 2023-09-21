using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    /// <summary>
    /// Модель дат собственности
    /// </summary>
    [Display(Name = "Даты")]
    public class PropertyDate
    {
        /// <summary>
        /// Дата вступления в собственность
        /// </summary>
        [Required(ErrorMessage = "Требуется внести дату вступления в собственность")]
        [Display(Name = "Дата вступления в собственность")]
        public DateTime DateOfTaking { get; set; }

        /// <summary>
        /// Дата окончания собственности
        /// </summary>
        [Display(Name = "Дата окончания собственности (необязательно)")]
        public DateTime? EndDate { get; set; }
    }
}
