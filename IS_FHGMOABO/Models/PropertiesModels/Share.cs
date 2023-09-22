using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.PropertiesModels
{
    /// <summary>
    /// Доля собственности
    /// </summary>
    [Display(Name = "Доля")]
    public class Share
    {
        /// <summary>
        /// Делимое значение доли
        /// </summary>
        public int Dividend { get; set; }

        /// <summary>
        /// Делитель доли
        /// </summary>
        public int Divisor { get; set; }
    }
}
