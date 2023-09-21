using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    /// <summary>
    /// Модель собственника (юр. лицо)
    /// </summary>
    [Display(Name = "Собственник")]
    public class LegalPerson
    {
        /// <summary>
        /// Наименование организации
        /// </summary>
        [Display(Name = "Наименование организации")]
        public string? Name { get; set; }
    }
}
