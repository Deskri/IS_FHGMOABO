using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    /// <summary>
    /// Документ подтверждающий право собственности
    /// </summary>
    [Display (Name = "Документ подтверждающий право собственности")]
    public class StateRegistration
    {
        /// <summary>
        /// Тип государственной регистрации
        /// </summary>
        [Display(Name = "Тип государственной регистрации")]
        public string? Type { get; set; }

        /// <summary>
        /// Номер государственной регистрации
        /// </summary>
        [Display(Name = "Номер государственной регистрации")]
        public string? Number { get; set; }

        /// <summary>
        /// Кем выдан документ
        /// </summary>
        [Display(Name = "Кем выдан документ")]
        public string? ByWhomIssued { get; set; }
    }
}
