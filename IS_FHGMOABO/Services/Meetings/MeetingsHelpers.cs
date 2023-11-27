using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.MeetingsModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Xceed.Words.NET;
using static IS_FHGMOABO.Models.MeetingsModels.DetailsMeetingModel;

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
        }

        /// <summary>
        /// Метод подсчета результатов по вопросу собрания
        /// </summary>
        /// <param name="results">Ответы на вопрос</param>
        /// <param name="question">Вопрос собрания</param>
        /// <param name="totalHouseArea">Общая площадь жилых и нежилых помещений (по паспорту МКД)</param>
        /// <returns>Модель отображения в представлении информации о вопросе</returns>
        public static QuestionResult ResponseСounter(List<VotingResult> results, Question question, decimal totalHouseArea)
        {
            var areaQuestion = results.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);

            bool quorum = areaQuestion / totalHouseArea * 100 > 50 ? true : false;

            IEnumerable<VotingResult> resultsFor;
            IEnumerable<VotingResult> resultsAgainst;
            IEnumerable<VotingResult> resultsAbstained;

            decimal areaFor = 0;
            decimal areaAgainst = 0;
            decimal areaAbstained = 0;

            decimal percentageFor = 0;
            decimal percentageAgainst = 0;
            decimal percentageAbstained = 0;

            bool isQuestionAccepted = false;

            switch (question.DecisionType)
            {
                case 0:
                    resultsFor = results.Where(x => x.Result == 1);
                    resultsAgainst = results.Where(x => x.Result == 0);
                    resultsAbstained = results.Where(x => x.Result == 2);

                    areaFor = resultsFor.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAgainst = resultsAgainst.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAbstained = resultsAbstained.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);

                    if (areaQuestion != 0)
                    {
                        if (areaFor != 0) percentageFor = areaFor / areaQuestion * 100;
                        if (areaAgainst != 0) percentageAgainst = areaAgainst / areaQuestion * 100;
                        if (areaAbstained != 0) percentageAbstained = areaAbstained / areaQuestion * 100;
                    }

                    if (percentageFor > percentageAgainst && percentageFor > percentageAbstained)
                    {
                        isQuestionAccepted = true;
                    }

                    break;
                case 1:

                    resultsFor = results.Where(x => x.Result == 1);
                    resultsAgainst = results.Where(x => x.Result == 0);
                    resultsAbstained = results.Where(x => x.Result == 2);

                    areaFor = resultsFor.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAgainst = resultsAgainst.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAbstained = resultsAbstained.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);

                    if (areaQuestion != 0)
                    {
                        if (areaFor != 0) percentageFor = areaFor / totalHouseArea * 100;
                        if (areaAgainst != 0) percentageAgainst = areaAgainst / totalHouseArea * 100;
                        if (areaAbstained != 0) percentageAbstained = areaAbstained / totalHouseArea * 100;
                    }

                    if (percentageFor > 50)
                    {
                        isQuestionAccepted = true;
                    }

                    break;
                case 2:

                    resultsFor = results.Where(x => x.Result == 1);
                    resultsAgainst = results.Where(x => x.Result == 0);
                    resultsAbstained = results.Where(x => x.Result == 2);

                    areaFor = resultsFor.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAgainst = resultsAgainst.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAbstained = resultsAbstained.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);

                    if (areaQuestion != 0)
                    {
                        if (areaFor != 0) percentageFor = areaFor / totalHouseArea * 100;
                        if (areaAgainst != 0) percentageAgainst = areaAgainst / totalHouseArea * 100;
                        if (areaAbstained != 0) percentageAbstained = areaAbstained / totalHouseArea * 100;
                    }

                    if (percentageFor > 2 / 3 * 100)
                    {
                        isQuestionAccepted = true;
                    }

                    break;
                case 3:

                    resultsFor = results.Where(x => x.Result == 1);
                    resultsAgainst = results.Where(x => x.Result == 0);
                    resultsAbstained = results.Where(x => x.Result == 2);

                    areaFor = resultsFor.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAgainst = resultsAgainst.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);
                    areaAbstained = resultsAbstained.Sum(x => x.Bulletin.Room.TotalArea * x.Bulletin.Property.Share);

                    if (areaQuestion != 0)
                    {
                        if (areaFor != 0) percentageFor = areaFor / totalHouseArea * 100;
                        if (areaAgainst != 0) percentageAgainst = areaAgainst / totalHouseArea * 100;
                        if (areaAbstained != 0) percentageAbstained = areaAbstained / totalHouseArea * 100;
                    }

                    if (percentageFor > 100)
                    {
                        isQuestionAccepted = true;
                    }

                    break;

            }

            return new QuestionResult()
            {
                Number = question.Number,
                Agenda = question.Agenda,
                IsThereQuorum = quorum,
                IsQuestionAccepted = isQuestionAccepted,
                DecisionType = question.DecisionType,
                For = new Responses()
                {
                    Area = areaFor,
                    Percentage = percentageFor,
                },
                Against = new Responses()
                {
                    Area = areaAgainst,
                    Percentage = percentageAgainst,
                },
                Abstained = new Responses()
                {
                    Area = areaAbstained,
                    Percentage = percentageAbstained,
                },
            };
        }
    }
}
