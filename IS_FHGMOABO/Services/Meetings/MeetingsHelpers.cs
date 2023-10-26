using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.MeetingsModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xceed.Words.NET;

namespace IS_FHGMOABO.Services.Meetings
{
    public class MeetingsHelpers
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public MeetingsHelpers(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        /// <summary>
        /// Обработка файлов для сохранения их в модели и проверка правильности расширения
        /// </summary>
        /// <param name="model">Модель добавления собрания</param>
        /// <param name="ModelState">Состояние модели данных</param>
        public static void AddMeetingAttachment(AddMeetingModel model, ModelStateDictionary ModelState)
        {
            if (model.Meeting.Questions != null)
            {
                for (int i = 0; i < model.Meeting.Questions.Count; i++)
                {
                    if (model.Meeting.Questions[i].Attachment != null)
                    {
                        string fileExtension = Path.GetExtension(model.Meeting.Questions[i].Attachment.FileName).ToLower();
                        if (fileExtension != ".docx")
                        {
                            ModelState.AddModelError($"Meeting.Questions[{i}].Attachment", "Файл должен быть в расширении .docx");
                        }
                        else
                        {
                            using (DocX doc = DocX.Load(model.Meeting.Questions[i].Attachment.OpenReadStream()))
                            {
                                MemoryStream memoryStream = new MemoryStream();
                                doc.SaveAs(memoryStream);

                                model.Meeting.Questions[i].AttachmentString = Convert.ToBase64String(memoryStream.ToArray());
                            }

                            model.Meeting.Questions[i].AttachmentName = model.Meeting.Questions[i].Attachment.FileName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Десерелизация списка домов для случаев, при которых модель повторно возвращается
        /// </summary>
        /// <param name="httpContext">Текущий контекст для получения сессии</param>
        /// <returns>Список домов</returns>
        public static List<House>? DedeserializeHouses(HttpContext httpContext)
        {
            var serializedModel = httpContext.Session.GetString("AddHouses");
            var deserializeModel = JsonConvert.DeserializeObject<List<House>>(serializedModel);

            return deserializeModel;
        }

        /// <summary>
        /// Серелизация списка домов для случаев, при которых модель повторно возвращается
        /// </summary>
        /// <param name="model">Текущая модель добавления</param>
        /// <param name="httpContext">Текущий контекст для получения сессии</param>
        public static void SerializeHouses(AddMeetingModel model, HttpContext httpContext)
        {
            var serializedModel = JsonConvert.SerializeObject(model.Houses);
            httpContext.Session.SetString("AddHouses", serializedModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Модель добавления собрания</param>
        /// <param name="ModelState">Состояние модели данных</param>
        public static void ValidationAddMeeting(AddMeetingModel model, ModelStateDictionary ModelState)
        {
            if (model.Meeting.StartDate < DateTime.Now.AddDays(10))
            {
                ModelState.AddModelError("Meeting.StartDate", "Дата начала проведения должна быть больше 10 дней от текущего времени.");
            }

            if (model.Meeting.CountingCommitteeMembers == null || model.Meeting.CountingCommitteeMembers.Count == 0)
            {
                ModelState.AddModelError("Meeting.CountingCommitteeMembers", "Должен присутствовать хотя бы один член счетной комиссии.");
            }

            if (model.Meeting.Questions == null || model.Meeting.Questions.Count == 0)
            {
                ModelState.AddModelError("Meeting.Questions", "Должен присутствовать хотя бы один вопрос собрания.");
            }
            else
            {
                for (int i = 0; i < model.Meeting.Questions.Count; i++)
                {
                    if (model.Meeting.Questions[i].Percentage <= 0)
                    {
                        ModelState.AddModelError($"Meeting.Questions[{i}].Percentage", "Процент не должен быть меньше или равен 0.");
                    }

                    if (model.Meeting.Questions[i].Percentage > 100)
                    {
                        ModelState.AddModelError($"Meeting.Questions[{i}].Percentage", "Процент не должен быть больше 100.");
                    }
                }
            }
        }
    }
}
