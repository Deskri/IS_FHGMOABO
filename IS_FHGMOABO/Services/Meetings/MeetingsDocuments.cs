using Xceed.Document.NET;
using Xceed.Words.NET;
using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.Services.Meetings
{
    public class MeetingsDocuments
    {
        public static MemoryStream MeetingNotification(Meeting meeting)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DocX document = DocX.Create(memoryStream))
                {
                    document.SetDefaultFont(new Font("Times New Roman"), 11);

                    document.MarginLeft = 36;
                    document.MarginRight = 36;
                    document.MarginTop = 36;
                    document.MarginBottom = 36;

                    Paragraph header = document.InsertParagraph();
                    Paragraph welcome = document.InsertParagraph();
                    Paragraph date = document.InsertParagraph();
                    Paragraph mainInfo = document.InsertParagraph();
                    Paragraph agenda = document.InsertParagraph();
                    Paragraph signature = document.InsertParagraph();

                    header.AppendLine("УВЕДОМЛЕНИЕ").Bold();
                    header.AppendLine("О ПРОВЕДЕНИИ ОЧЕРЕДНОГО");
                    header.AppendLine("ОБЩЕГО СОБРАНИЯ СОБСТВЕННИКОВ ПОМЕЩЕНИЙ");
                    switch (meeting.Format)
                    {
                        case "Очное":
                            header.AppendLine("В ФОРМЕ ОЧНОГО ГОЛОСОВАНИЯ");
                            break;
                        case "Заочное":
                            header.AppendLine("В ФОРМЕ ЗАОЧНОГО ГОЛОСОВАНИЯ");
                            break;
                        case "Очно-заочное":
                            header.AppendLine("В ФОРМЕ ОЧНО-ЗАОЧНОГО ГОЛОСОВАНИЯ");
                            break;
                    }
                    header.AppendLine("В МНОГОКВАРТИРНОМ ДОМЕ ПО АДРЕСУ:");
                    if (meeting.House != null)
                        header.AppendLine($"{meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}").Bold();
                    header.Alignment = Alignment.center;

                    welcome.AppendLine("Уважаемые собственники помещений!").Bold();
                    welcome.Alignment = Alignment.center;

                    date.AppendLine($"{meeting.StartDate.Day}.{meeting.StartDate.Month}.{meeting.StartDate.Year}г.").Bold();
                    date.Alignment = Alignment.right;

                    switch (meeting.Format)
                    {
                        case "Очное":
                            mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме очного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.");
                            mainInfo.AppendLine($"\tНачало вышеуказанного общего собрания: ___ часов ___ минут «___» _________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. Просим собственников принять личное участие.");
                            break;
                        case "Заочное":
                            mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме заочного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.");
                            mainInfo.AppendLine($"\tОкончание вышеуказанного общего собрания и приема решений (бюллетеней) собственников помещений: ____ часов ___ минут  «___» ___________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.");
                            mainInfo.AppendLine($"\tБюллетени для заполнения будут разосланы в почтовые ящики.");
                            mainInfo.AppendLine($"\tСдать заполненные бюллетени можно в ________________________________.");
                            break;
                        case "Очно-заочное":
                            mainInfo.AppendLine($"\tПросим Вас принять участие в очередном общем собрании собственников помещений в форме очно-заочного голосования по вопросам, представленным ниже в повестке дня, в соответствии с Жилищным кодексом РФ по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.");
                            mainInfo.AppendLine($"\tНачало вышеуказанного общего собрания: ___ часов ___ минут «___» _________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}. Просим собственников принять личное участие.");
                            mainInfo.AppendLine($"\tОкончание вышеуказанного общего собрания и приема решений (бюллетеней) собственников помещений: ____ часов ___ минут  «___» ___________ ____ года по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.");
                            mainInfo.AppendLine($"\tБюллетени для заполнения будут разосланы в почтовые ящики.");
                            mainInfo.AppendLine($"\tСдать заполненные бюллетени можно в ________________________________.");
                            break;
                    }
                    mainInfo.AppendLine($"\tРешения, принятые общим собранием, и итоги голосования будут объявлены в течение десяти календарных дней с даты окончания вышеуказанного собрания (в соответствии с частью 3 статьи 46 Жилищного кодекса Российской Федерации).");
                    mainInfo.Alignment = Alignment.both;

                    agenda.AppendLine("\tПовестка дня:").Bold();
                    if (meeting.Questions != null)
                    {
                        foreach (var question in meeting.Questions)
                        {
                            document.InsertParagraph().Append($"\t{question.Number}. {question.Agenda}").Bold().Alignment = Alignment.both;
                            document.InsertParagraph().Append($"\tПредложено: {question.Proposed}").Alignment = Alignment.both;
                        }
                    }
                    agenda.Alignment = Alignment.both;

                    signature.AppendLine("\tИнициатор _____________________________________/__________________________");

                    if (meeting.Questions != null)
                    {
                        foreach (var question in meeting.Questions)
                        {
                            if (question.Attachment != null)
                            {
                                byte[] byteArray = Convert.FromBase64String(question.Attachment);
                                MemoryStream attachment = new MemoryStream(byteArray);

                                document.InsertSectionPageBreak();
                                using (DocX attachmentDocument = DocX.Load(attachment))
                                {
                                    document.InsertDocument(attachmentDocument);
                                }
                            }
                        }
                    }

                    document.Save();

                    return memoryStream;
                }
            }
        }
    }
}
