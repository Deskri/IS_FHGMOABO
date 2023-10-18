using IS_FHGMOABO.DAL;
using IS_FHGMOABO.DBConection;
using IS_FHGMOABO.Models.MeetingsModels;
using IS_FHGMOABO.Services.Meetings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace IS_FHGMOABO.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public MeetingsController(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _applicationDBContext.Meetings
                                                   .Include(x => x.House)
                                                   .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddMeetingModel()
            {
                Houses = await _applicationDBContext.Houses.ToListAsync(),
            };

            MeetingsHelpers.SerializeHouses(model, HttpContext);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddMeetingModel model)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            MeetingsHelpers.ValidationAddMeeting(model, ModelState);

            if (ModelState.IsValid)
            {
                var meeting = new Meeting()
                {
                    HouseId = model.Meeting.HouseId,
                    StartDate = model.Meeting.StartDate,
                    Format = AddMeeting.GetEnumDescription(model.Meeting.Format),
                    Status = "Создано",
                    Secretary = $"{model.Meeting.Secretary.LastName} {model.Meeting.Secretary.FirstName} {model.Meeting.Secretary.Patronymic}",
                    Chairperson = $"{model.Meeting.Chairperson.LastName} {model.Meeting.Chairperson.FirstName} {model.Meeting.Chairperson.Patronymic}",
                };

                meeting.CountingCommitteeMembers = new List<CountingCommitteeMember>();

                foreach (var countingCommitteeMember in model.Meeting.CountingCommitteeMembers)
                {
                    meeting.CountingCommitteeMembers.Add(new CountingCommitteeMember()
                    {
                        FullName = $"{countingCommitteeMember.LastName} {countingCommitteeMember.FirstName} {countingCommitteeMember.Patronymic}",
                    });
                }

                meeting.Questions = new List<Question>();

                int attachmentNumber = 1;

                for (int i = 0; i < model.Meeting.Questions.Count; i++)
                {
                    if (model.Meeting.Questions[i].AttachmentString == null)
                    {
                        meeting.Questions.Add(new Question()
                        {
                            Number = i + 1,
                            Agenda = model.Meeting.Questions[i].Agenda,
                            Proposed = model.Meeting.Questions[i].Proposed,
                            Percentage = model.Meeting.Questions[i].Percentage,
                        });
                    }
                    else
                    {
                        meeting.Questions.Add(new Question()
                        {
                            Number = i + 1,
                            Agenda = model.Meeting.Questions[i].Agenda,
                            Proposed = model.Meeting.Questions[i].Proposed,
                            Percentage = model.Meeting.Questions[i].Percentage,
                            Attachment = model.Meeting.Questions[i].AttachmentString,
                            AttachmentNumber = attachmentNumber,
                        });

                        attachmentNumber++;
                    }
                }

                await _applicationDBContext.AddAsync(meeting);
                await _applicationDBContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCountingCommitteeMember(AddMeetingModel model)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            if (model.Meeting.CountingCommitteeMembers == null)
            {
                model.Meeting.CountingCommitteeMembers = new List<AddCountingCommitteeMember>();
            }

            model.Meeting.CountingCommitteeMembers.Add(new AddCountingCommitteeMember());

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteCountingCommitteeMember(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.CountingCommitteeMembers.RemoveAt(index);

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddQuestion(AddMeetingModel model)
        {
            if (model.Meeting.Questions == null)
            {
                model.Meeting.Questions = new List<AddQuestion>();
            }

            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions.Add(new AddQuestion());

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteQuestion(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions.RemoveAt(index);

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveAttachment(AddMeetingModel model, int index)
        {
            MeetingsHelpers.AddMeetingAttachment(model, ModelState);

            model.Meeting.Questions[index].AttachmentString = null;
            model.Meeting.Questions[index].AttachmentName = null;

            model.Houses = MeetingsHelpers.DedeserializeHouses(HttpContext);

            return View("Add", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _applicationDBContext.Meetings
                                                   .Where(x => x.Id == id)
                                                   .Include(x => x.Questions)
                                                   .Include(x => x.CountingCommitteeMembers)
                                                   .Include(x => x.House)
                                                   .FirstOrDefaultAsync();

            return View("Details", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var question = await _applicationDBContext.Questions
                                                      .Include(x => x.Meeting)
                                                      .Where(x => x.Id == id)
                                                      .FirstOrDefaultAsync();

            byte[] byteArray = Convert.FromBase64String(question.Attachment);
            MemoryStream memoryStream = new MemoryStream(byteArray);

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Приложение №{question.AttachmentNumber} ОCС от {question.Meeting.StartDate.ToString("dd.MM.yyyy")}.docx");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateNotification(int id)
        {
            var meeting = await _applicationDBContext.Meetings
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.House)
                                                     .Include(x => x.Questions)
                                                     .FirstOrDefaultAsync();

            /*            using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (DocX document = DocX.Create(memoryStream))
                            {
                                document.SetDefaultFont(new Font("Times New Roman"), 11);

                                document.MarginLeft = 36;
                                document.MarginRight = 36;
                                document.MarginTop = 36;
                                document.MarginBottom = 36;

                                Paragraph header = document.InsertParagraph();
                                header.AppendLine("УВЕДОМЛЕНИЕ").Bold();
                                header.AppendLine("О ПРОВЕДЕНИИ ОЧЕРЕДНОГО");
                                header.AppendLine("ОБЩЕГО СОБРАНИЯ СОБСТВЕННИКОВ ПОМЕЩЕНИЙ");
                                if (meeting.Format == "Очное")
                                {
                                    header.AppendLine("В ФОРМЕ ОЧНОГО ГОЛОСОВАНИЯ");
                                }
                                else if (meeting.Format == "Заочное")
                                {
                                    header.AppendLine("В ФОРМЕ ЗАОЧНОГО ГОЛОСОВАНИЯ");
                                }
                                else if (meeting.Format == "Очно-заочное")
                                {
                                    header.AppendLine("В ФОРМЕ ОЧНО-ЗАОЧНОГО ГОЛОСОВАНИЯ");
                                }
                                header.AppendLine("В МНОГОКВАРТИРНОМ ДОМЕ ПО АДРЕСУ:");
                                header.AppendLine($"{meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}").Bold();
                                header.Alignment = Alignment.center;

                                Paragraph welcome = document.InsertParagraph();
                                welcome.AppendLine("Уважаемые собственники помещений!").Bold();
                                welcome.Alignment = Alignment.center;

                                Paragraph date = document.InsertParagraph();
                                date.AppendLine($"{meeting.StartDate.Day}.{meeting.StartDate.Month}.{meeting.StartDate.Year}г.").Bold();
                                date.Alignment = Alignment.right;

                                Paragraph mainInfo = document.InsertParagraph();
                                if (meeting.Format == "Очное")
                                {
                                    mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме очного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. ");
                                }
                                else if (meeting.Format == "Заочное")
                                {
                                    mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме заочного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. ");
                                }
                                else if (meeting.Format == "Очно-заочное")
                                {
                                    mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме очно-заочного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. ");
                                }
                                if (meeting.Format == "Очное" || meeting.Format == "Очно-заочное")
                                {
                                    mainInfo.AppendLine($"\tНачало вышеуказанного общего собрания: ___ часов ___ минут «___» _________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. Просим собственников принять личное участие. ");
                                }
                                if (meeting.Format == "Заочное" || meeting.Format == "Очно-заочное")
                                {
                                    mainInfo.AppendLine($"\tОкончание вышеуказанного общего собрания и приема решений (бюллетеней) собственников помещений: ____ часов ___ минут  «___» ___________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. ");
                                    mainInfo.AppendLine($"\tБюллетени для заполнения будут разосланы в почтовые ящики. ");
                                    mainInfo.AppendLine($"\tСдать заполненные бюллетени можно в ________________________________. ");
                                }
                                mainInfo.AppendLine($"\tРешения, принятые общим собранием, и итоги голосования будут объявлены в течение десяти календарных дней с даты окончания вышеуказанного собрания (в соответствии с частью 3 статьи 46 Жилищного кодекса Российской Федерации). ");
                                mainInfo.Alignment = Alignment.both;

                                Paragraph agenda = document.InsertParagraph();
                                agenda.AppendLine("\tПовестка дня: ").Bold();
                                foreach (var question in meeting.Questions)
                                {
                                    agenda.AppendLine($"\t{question.Number}. {question.Agenda} ").Bold();
                                    agenda.AppendLine($"\t{question.Proposed} ");
                                }
                                agenda.Alignment = Alignment.both;

                                Paragraph signature = document.InsertParagraph();
                                signature.AppendLine("\tИнициатор _____________________________________/__________________________");

                                document.Save();
                            }*/

            var memoryStream = MeetingsDocuments.MeetingNotification(meeting);

/*            memoryStream.Position = 0;*/

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Уведомление о проведении ОСС.docx");
        }
    }
}