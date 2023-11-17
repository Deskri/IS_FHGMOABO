using IS_FHGMOABO.DAL;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using Xceed.Words.NET;

namespace IS_FHGMOABO.Models.MeetingsModels
{
    public class AddMeetingModel
    {
        /// <summary>
        /// Модель создания собрания
        /// </summary>
        public AddMeeting Meeting { get; set; }
        /// <summary>
        /// Лист домов для выбора
        /// </summary>
        public List<House>? Houses { get; set; }

    }

    public class AddMeeting
    {
        /// <summary>
        /// Идентификатор дома
        /// </summary>
        [Required(ErrorMessage = "Требуется внести дом")]
        [Display(Name = "Дом")]
        public int HouseId { get; set; }
        /// <summary>
        /// Дата начала проведения собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести дату начала проведения собрания")]
        [Display(Name = "Дата начала проведения собрания")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Формат проведения собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести формат проведения собрания")]
        [Display(Name = "Формат проведения собрания")]
        public FormatMeeting Format { get; set; }
        /// <summary>
        /// Секретарь
        /// </summary>
        [Display(Name = "Секретарь")]
        public AddSecretary Secretary { get; set; }
        /// <summary>
        /// Председатель
        /// </summary>
        [Display(Name = "Председатель")]
        public AddChairperson Chairperson { get; set; }
        /// <summary>
        /// Лист членов счетной комиссии
        /// </summary>
        [Display(Name = "Члены счетной комиссии")]
        public List<AddCountingCommitteeMember> CountingCommitteeMembers { get; set; }
        /// <summary>
        /// Лист вопросов собрания
        /// </summary>
        [Display(Name = "Вопросы голосования")]
        public List<AddQuestion> Questions { get; set; }
        /// <summary>
        /// Тип перечисления формата собрания
        /// </summary>
        public enum FormatMeeting
        {
            [Description("Очное")]
            InPerson,
            [Description("Заочное")]
            Absentee,
            [Description("Очно-заочное")]
            InPersonAndAbsentee,
        }
        /// <summary>
        /// Метод получения строки описания формата собрания из типа перечисления
        /// </summary>
        /// <param name="value">Значение типа перечисления</param>
        /// <returns>Строка описания</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>(false);

            return attribute != null ? attribute.Description : value.ToString();
        }
    }
    public class AddSecretary
    {
        /// <summary>
        /// Фамилия секретаря собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести фамилию секретаря собрания")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя секретаря собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести имя секретаря собрания")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество секретаря собрания
        /// </summary>
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }
    }
    public class AddChairperson
    {
        /// <summary>
        /// Фамилия председателя собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести фамилию председателя собрания")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя председателя собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести имя председателя собрания")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество председателя собрания
        /// </summary>
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }
    }
    public class AddCountingCommitteeMember
    {
        /// <summary>
        /// Фамилия члена счетной комиссии собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести фамилию члена счетной комиссии собрания")]
        [Display(Name = "Фамилия члена счетной комиссии собрания")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя члена счетной комиссии собрания
        /// </summary>
        [Required(ErrorMessage = "Требуется внести имя члена счетной комиссии собрания")]
        [Display(Name = "Имя члена счетной комиссии собрания")]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество члена счетной комиссии собрания
        /// </summary>
        [Display(Name = "Отчество члена счетной комиссии собрания")]
        public string? Patronymic { get; set; }
    }
    public class AddQuestion
    {
        /// <summary>
        /// Повестка дня
        /// </summary>
        [Required(ErrorMessage = "Требуется внести повестку дня")]
        [Display(Name = "Повестка дня")]
        public string Agenda { get; set; }

        /// <summary>
        /// Предложено
        /// </summary>
        [Required(ErrorMessage = "Требуется внести предложено")]
        [Display(Name = "Предложено")]
        public string Proposed { get; set; }

        /// <summary>
        /// Тип вопроса для принятия решения
        /// </summary>
        [Required(ErrorMessage = "Требуется внести тип вопроса для принятия решения")]
        [Display(Name = "Тип вопроса для принятия решения")]
        public short DecisionType { get; set; }

        /// <summary>
        /// Приложение
        /// </summary>
        [Display(Name = "Приложение")]
        public IFormFile? Attachment { get; set; }

        /// <summary>
        /// Название файла приложения при сохранения в модель
        /// </summary>
        public string? AttachmentName { get; set; }

        /// <summary>
        /// Файл приложения конвертированный в строку
        /// </summary>
        public string? AttachmentString { get; set; }
    }
}
