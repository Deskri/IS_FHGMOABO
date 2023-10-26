using Xceed.Document.NET;
using Xceed.Words.NET;
using IS_FHGMOABO.DAL;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;

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

                    Paragraph signature = document.InsertParagraph();
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
        public static MemoryStream MeetingVotingRegister(List<Bulletin> bulletins)
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

                    document.PageLayout.Orientation = Orientation.Landscape;

                    Paragraph header = document.InsertParagraph();

                    int col = 5 + bulletins[0].Meeting.Questions.Count;
                    int row = 1 + bulletins.Count;

                    Table table = document.AddTable(row, col);

                    header.AppendLine($"РЕЕСТР ГОЛОСОВАНИЯ собственников помещений в многоквартирном доме, по адресу: {bulletins[0].Room.House.Type} {bulletins[0].Room.House.Street}, дом {bulletins[0].Room.House.Number}.").Bold();

                    table.Rows[0].Cells[0].Paragraphs.First().Append("№ квартиры/помещения");
                    table.Rows[0].Cells[1].Paragraphs.First().Append("ФИО (наименование юр.лица)");
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Площадь, принадлежащая собственнику на основании правоустанавливающего документа");
                    table.Rows[0].Cells[3].Paragraphs.First().Append("Сведения о документе, подтверждающем право собственности лица, участвующего в голосовании на помещение в МКД");

                    int questionСounter = 1;

                    foreach (var question in bulletins[0].Meeting.Questions)
                    {
                        table.Rows[0].Cells[3 + questionСounter].Paragraphs.First().Append($"Вопрос {question.Number}");
                        questionСounter++;
                    }
                    table.Rows[0].Cells[3 + questionСounter].Paragraphs.First().Append("Подпись");

                    for (var i = 0; i < bulletins.Count; i++)
                    {
                        table.Rows[i + 1].Cells[0].Paragraphs.First().Append($"{bulletins[i].Room.Number}");
                        if (bulletins[i].Property == null)
                        {
                            table.Rows[i + 1].Cells[1].Paragraphs.First().Append("Муниципалитет");
                        }
                        if (bulletins[i].Property.LegalPerson != null)
                        {
                            table.Rows[i + 1].Cells[1].Paragraphs.First().Append($"{bulletins[i].Property.LegalPerson.Name}");
                        }
                        if (bulletins[i].Property.NaturalPersons != null)
                        {
                            foreach (var person in bulletins[i].Property.NaturalPersons)
                            {
                                table.Rows[i + 1].Cells[1].Paragraphs.First().Append($"{person.LastName} {person.FirstName} {person.Patronymic} ");
                            }
                        }
                        table.Rows[i + 1].Cells[2].Paragraphs.First().Append($"{Math.Round(bulletins[i].Room.TotalArea * bulletins[i].Property.Share, 2)}");
                        table.Rows[i + 1].Cells[3].Paragraphs.First().Append($"{bulletins[i].Property.TypeOfStateRegistration} {bulletins[i].Property.StateRegistrationNumber}");
                    }

                    document.InsertTable(table);
                    document.Save();

                    return memoryStream;
                }
            }
        }
        public static MemoryStream MeetingBulletins(List<Bulletin> bulletins)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DocX document = DocX.Create(memoryStream))
                {
                    for (int i = 0; i < bulletins.Count; i++)
                    {
                        MemoryStream bulletin = new MemoryStream(Convert.FromBase64String(Convert.ToBase64String(MeetingBulletin(bulletins[i]).ToArray())));
                        using (DocX bulletinDocument = DocX.Load(bulletin))
                        {
                            document.InsertDocument(bulletinDocument);
                        }
                    }
                    document.RemoveParagraphAt(0);

                    document.Save();

                    return memoryStream;
                }
            }
        }
        public static MemoryStream MeetingBulletin(Bulletin bulletin)
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

                    switch (bulletin.Meeting.Format)
                    {
                        case "Заочное":
                            header.Append("БЮЛЛЕТЕНЬ ЗАОЧНОГО ГОЛОСОВАНИЯ\n").Bold();
                            break;
                        case "Очно-заочное":
                            header.Append("БЮЛЛЕТЕНЬ ОЧНО-ЗАОЧНОГО ГОЛОСОВАНИЯ\n").Bold();
                            break;
                    }
                    header.Append($"на общем собрании собственников в многоквартирном доме №{bulletin.Room.House.Number} по {bulletin.Room.House.Type} {bulletin.Room.House.Street} в {bulletin.Room.House.InhabitedLocality}\n").Bold();
                    header.Alignment = Alignment.center;

                    Table table = document.AddTable(1, 2);
                    table.Design = TableDesign.None;

                    table.Rows[0].Cells[0].Paragraphs.First().Append($"{bulletin.Room.House.InhabitedLocality}");
                    table.Rows[0].Cells[1].Paragraphs.First().Append($"{bulletin.Meeting.StartDate.Day}.{bulletin.Meeting.StartDate.Month}.{bulletin.Meeting.StartDate.Year}г.").Alignment = Alignment.right;

                    document.InsertTable(table);

                    Paragraph ownerInformation = document.InsertParagraph();

                    ownerInformation.Append("\nСведения о собственнике (представителя собственника):\n").Bold();
                    if (bulletin.Room.IsPrivatized == false)
                    {
                        ownerInformation.Append("Ф.И.О.:\n");
                        ownerInformation.Append("Муниципалитет.\n").UnderlineStyle(UnderlineStyle.singleLine);
                    }
                    else if (bulletin.Property.LegalPerson != null)
                    {
                        ownerInformation.Append("Ф.И.О.:\n");
                        ownerInformation.Append($"{bulletin.Property.LegalPerson.Name}.\n").UnderlineStyle(UnderlineStyle.singleLine);
                    }
                    else if (bulletin.Property.NaturalPersons != null)
                    {
                        ownerInformation.Append("Ф.И.О.:\n");
                        foreach (var person in bulletin.Property.NaturalPersons)
                        {
                            ownerInformation.Append($"{person.LastName} {person.FirstName} {person.Patronymic} ").UnderlineStyle(UnderlineStyle.singleLine);
                        }
                        ownerInformation.Append(".\n").UnderlineStyle(UnderlineStyle.singleLine);
                    }
                    else
                    {
                        ownerInformation.Append("_______________________________________________________________________________________________\n");
                    }
                    ownerInformation.Append("Сведения о документе на право собственности:\n").Bold();
                    ownerInformation.Append("Документ, подтверждающий право собственности на помещение:\n");
                    if (bulletin.Property != null && bulletin.Property.TypeOfStateRegistration != null && bulletin.Property.StateRegistrationNumber != null)
                    {
                        ownerInformation.Append($"{bulletin.Property.TypeOfStateRegistration} {bulletin.Property.StateRegistrationNumber} от {bulletin.Property.DateOfTaking.ToString("dd.MM.yyyy")}\n").UnderlineStyle(UnderlineStyle.singleLine);
                    }
                    else
                    {
                        ownerInformation.Append("_______________________________________________________________________________________________\n");
                    }
                    ownerInformation.Append("Кем выдано:\n");
                    if (bulletin.Property != null && bulletin.Property.TypeOfStateRegistration != null && bulletin.Property.StateRegistrationNumber != null)
                    {
                        ownerInformation.Append($"{bulletin.Property.ByWhomIssued}.\n").UnderlineStyle(UnderlineStyle.singleLine);
                    }
                    else
                    {
                        ownerInformation.Append("_______________________________________________________________________________________________\n");
                    }
                    ownerInformation.Append($"Сведения о помещении: квартира/помещение №{bulletin.Room.Number}, общая площадь {bulletin.Room.TotalArea}м2, доля собственника в жилом помещении: ");
                    if (bulletin.Property != null)
                    {
                        ownerInformation.Append($"{Math.Round(bulletin.Property.Share * 100, 2)}%");
                    }
                    else
                    {
                        ownerInformation.Append("___");
                    }

                    Paragraph agendaHeader = document.InsertParagraph();

                    agendaHeader.Append("\nПовестка дня:\n").Bold();
                    agendaHeader.Alignment = Alignment.center;

                    Paragraph agenda = document.InsertParagraph();
                    foreach (var question in bulletin.Meeting.Questions)
                    {
                        agenda.Append($"{question.Number}. {question.Agenda}\n");
                    }

                    Paragraph warning = document.InsertParagraph();

                    warning.Append("ВАЖНО! ").Bold();
                    warning.Append("При голосовании ставить только один из возможных вариантов ответа!\n").UnderlineStyle(UnderlineStyle.singleLine);
                    warning.Alignment = Alignment.center;

                    Paragraph votingResultsHeader = document.InsertParagraph();

                    votingResultsHeader.Append("РЕЗУЛЬТАТЫ ГОЛОСОВАНИЯ").Bold();
                    votingResultsHeader.Alignment = Alignment.center;

                    foreach (var question in bulletin.Meeting.Questions)
                    {
                        Paragraph votingResults = document.InsertParagraph();
                        Table tableVotingResult = document.AddTable(2, 3);

                        tableVotingResult.Rows[0].Cells[0].Paragraphs.First().Append("за").Alignment = Alignment.center;
                        tableVotingResult.Rows[0].Cells[1].Paragraphs.First().Append("против").Alignment = Alignment.center;
                        tableVotingResult.Rows[0].Cells[2].Paragraphs.First().Append("воздержался").Alignment = Alignment.center;

                        votingResults.Append($"\n{question.Number}. {question.Agenda}\n").Bold();
                        votingResults.Append($"Предложено: {question.Proposed}");
                        if(question.Attachment != null && question.AttachmentNumber != null)
                        {
                            votingResults.Append($" (Приложение {question.AttachmentNumber}).");
                        }
                        votingResults.Append("\n");
                        document.InsertTable(tableVotingResult);
                    }

                    Paragraph sign = document.InsertParagraph();

                    sign.Append("\nСобственник:_______________________________/____________________________________________________\n");
                    sign.Append("                                                 (подпись)                                                                             (Ф.И.О.)");

                    if (bulletin.Meeting.Questions != null)
                    {
                        foreach (var question in bulletin.Meeting.Questions)
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
