namespace IS_FHGMOABO.Models.PropertiesModels
{
    public class StateRegistration
    {
        /// <summary>
        /// Тип государственной регистрации
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Номер государственной регистрации
        /// </summary>
        public string? Number { get; set; }
        /// <summary>
        /// Кем выдан документ
        /// </summary>
        public string? ByWhomIssued { get; set; }
    }
}
