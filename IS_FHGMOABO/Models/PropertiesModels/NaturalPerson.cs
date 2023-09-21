using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    /// <summary>
    /// Модель собственника (физ. лицо)
    /// </summary>
    [Display(Name = "Собственник")]
    public class NaturalPerson
    {
        /// <summary>
        /// Имя
        /// </summary>
        [Display(Name = "Имя")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Display(Name = "Фамилия")]
        public string? LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [Display(Name = "Отчество (необязательно)")]
        public string? Patronymic { get; set; }

        /// <summary>
        /// День рождения
        /// </summary>
        [Display(Name = "Дата рождения (необязательно)")]
        public DateTime? DateOfBirth { get; set; }
    }
}
