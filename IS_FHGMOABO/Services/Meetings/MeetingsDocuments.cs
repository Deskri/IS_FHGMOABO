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
                        ownerInformation.Append("100,00% ");
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
                        if (question.Attachment != null && question.AttachmentNumber != null)
                        {
                            votingResults.Append($" (Приложение {question.AttachmentNumber}).");
                        }
                        votingResults.Append("\n");
                        document.InsertTable(tableVotingResult);
                    }

                    Paragraph sign = document.InsertParagraph();

                    sign.Append("\nСобственник:_______________________________/___________________________________________________\n");
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
        public static MemoryStream MeetingBulletinWithoutAttachment(Bulletin bulletin)
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
                        ownerInformation.Append("100,00% ");
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
                        if (question.Attachment != null && question.AttachmentNumber != null)
                        {
                            votingResults.Append($" (Приложение {question.AttachmentNumber}).");
                        }
                        votingResults.Append("\n");
                        document.InsertTable(tableVotingResult);
                    }

                    Paragraph sign = document.InsertParagraph();

                    sign.Append("\nСобственник:_______________________________/___________________________________________________\n");
                    sign.Append("                                                 (подпись)                                                                             (Ф.И.О.)");

                    document.Save();

                    return memoryStream;
                }
            }
        }
        public static MemoryStream EmptyMeetingBulletin(Meeting meeting)
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

                    switch (meeting.Format)
                    {
                        case "Заочное":
                            header.Append("БЮЛЛЕТЕНЬ ЗАОЧНОГО ГОЛОСОВАНИЯ\n").Bold();
                            break;
                        case "Очно-заочное":
                            header.Append("БЮЛЛЕТЕНЬ ОЧНО-ЗАОЧНОГО ГОЛОСОВАНИЯ\n").Bold();
                            break;
                    }
                    header.Append($"на общем собрании собственников в многоквартирном доме №{meeting.House.Number} по {meeting.House.Type} {meeting.House.Street} в {meeting.House.InhabitedLocality}\n").Bold();
                    header.Alignment = Alignment.center;

                    Table table = document.AddTable(1, 2);
                    table.Design = TableDesign.None;

                    table.Rows[0].Cells[0].Paragraphs.First().Append($"{meeting.House.InhabitedLocality}");
                    table.Rows[0].Cells[1].Paragraphs.First().Append($"{meeting.StartDate.Day}.{meeting.StartDate.Month}.{meeting.StartDate.Year}г.").Alignment = Alignment.right;

                    document.InsertTable(table);

                    Paragraph ownerInformation = document.InsertParagraph();

                    ownerInformation.Append("\nСведения о собственнике (представителя собственника):\n").Bold();
                    ownerInformation.Append("_______________________________________________________________________________________________\n");
                    ownerInformation.Append("Сведения о документе на право собственности:\n").Bold();
                    ownerInformation.Append("Документ, подтверждающий право собственности на помещение:\n");
                    ownerInformation.Append("_______________________________________________________________________________________________\n");
                    ownerInformation.Append("Кем выдано:\n");
                    ownerInformation.Append("_______________________________________________________________________________________________\n");
                    ownerInformation.Append($"Сведения о помещении: квартира/помещение № ____, общая площадь ____ м2, доля собственника в жилом помещении: ___");

                    Paragraph agendaHeader = document.InsertParagraph();

                    agendaHeader.Append("\nПовестка дня:\n").Bold();
                    agendaHeader.Alignment = Alignment.center;

                    Paragraph agenda = document.InsertParagraph();
                    foreach (var question in meeting.Questions)
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

                    foreach (var question in meeting.Questions)
                    {
                        Paragraph votingResults = document.InsertParagraph();
                        Table tableVotingResult = document.AddTable(2, 3);

                        tableVotingResult.Rows[0].Cells[0].Paragraphs.First().Append("за").Alignment = Alignment.center;
                        tableVotingResult.Rows[0].Cells[1].Paragraphs.First().Append("против").Alignment = Alignment.center;
                        tableVotingResult.Rows[0].Cells[2].Paragraphs.First().Append("воздержался").Alignment = Alignment.center;

                        votingResults.Append($"\n{question.Number}. {question.Agenda}\n").Bold();
                        votingResults.Append($"Предложено: {question.Proposed}");
                        if (question.Attachment != null && question.AttachmentNumber != null)
                        {
                            votingResults.Append($" (Приложение {question.AttachmentNumber}).");
                        }
                        votingResults.Append("\n");
                        document.InsertTable(tableVotingResult);
                    }

                    Paragraph sign = document.InsertParagraph();

                    sign.Append("\nСобственник:_______________________________/___________________________________________________\n");
                    sign.Append("                                                 (подпись)                                                                             (Ф.И.О.)");

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
        public static MemoryStream MeetingBulletinsWithoutAttachment(List<Bulletin> bulletins)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DocX document = DocX.Create(memoryStream))
                {
                    for (int i = 0; i < bulletins.Count; i++)
                    {
                        MemoryStream bulletin = new MemoryStream(Convert.FromBase64String(Convert.ToBase64String(MeetingBulletinWithoutAttachment(bulletins[i]).ToArray())));
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
        public static MemoryStream MeetingProtocol(Meeting meeting)
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

                    header.Append("Протокол\n").Bold();
                    header.Append("общего собрания собственников помещений\n").Bold();
                    header.Append($"в многоквартирном доме № {meeting.House.Number} по адресу: {meeting.House.Type} {meeting.House.Street},\n").Bold();
                    switch (meeting.Format)
                    {
                        case "Очное":
                            header.Append("проводимого в форме очного голосования\n").Bold();
                            break;
                        case "Заочное":
                            header.Append("проводимого в форме заочного голосования\n").Bold();
                            break;
                        case "Очно-заочное":
                            header.Append("проводимого в форме очно-заочного голосования\n").Bold();
                            break;
                    }
                    header.Alignment = Alignment.center;

                    Table table = document.AddTable(1, 2);
                    table.Design = TableDesign.None;
                    table.Rows[0].Cells[0].Paragraphs.First().Append($"{meeting.House.InhabitedLocality}");
                    table.Rows[0].Cells[1].Paragraphs.First().Append($"{meeting.StartDate.AddDays(10).ToString("dd.MM.yyyy")}г.").Alignment = Alignment.right;
                    document.InsertTable(table);

                    Paragraph adress = document.InsertParagraph();
                    adress.Append($"\nАдрес: {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}\n").Bold().UnderlineStyle(UnderlineStyle.singleLine);

                    Paragraph areas = document.InsertParagraph();
                    areas.Append($"Общая площадь помещений дома: {meeting.ArchivalInformationOfMeeting.TotalAreaHouse} м².\n");
                    areas.Append($"Площадь помещений, находящихся в собственности граждан: {meeting.ArchivalInformationOfMeeting.ResidentialAreaInOwnership} м².\n");
                    areas.Append($"Площадь помещений, находящихся в муниципальной собственности: {meeting.ArchivalInformationOfMeeting.ResidentialAreaInNonOwnership} м².\n");
                    areas.Append($"Площадь нежилых помещений:  {meeting.ArchivalInformationOfMeeting.NonresidentialArea} м².\n");

                    Paragraph startDate = document.InsertParagraph();
                    startDate.Append($"Дата проведения общего собрания {meeting.StartDate.ToString("dd.MM.yyyy")}.\n");
                    switch (meeting.Format)
                    {
                        case "Очное":
                            startDate.Append("Общее собрание собственников помещений проводилось в очной форме.\n");
                            break;
                        case "Заочное":
                            startDate.Append("Общее собрание собственников помещений проводилось в заочной форме.\n");
                            break;
                        case "Очно-заочное":
                            startDate.Append("Общее собрание собственников помещений проводилось в очно-заочной форме.\n");
                            break;
                    }

                    Paragraph participated = document.InsertParagraph();
                    participated.Append($"В общем собрании количество собственников, которые приняли участие: {meeting.ArchivalInformationOfMeeting.OwnersParticipated}. ");
                    participated.Append($"Обладавшие площадью: {meeting.ArchivalInformationOfMeeting.ParticipatingArea} м². ");
                    participated.Append($"Голосов от общего числа голосов собственников помещений: {Math.Round(meeting.ArchivalInformationOfMeeting.ParticipatingArea / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100, 2)}%. ");
                    participated.Append($"Собрание проводилось по адресу: {meeting.House.InhabitedLocality}, {meeting.House.Type} {meeting.House.Street}, дом {meeting.House.Number}.\n");

                    Paragraph isTookPlace = document.InsertParagraph();
                    if (meeting.ArchivalInformationOfMeeting.ParticipatingArea / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100 >= 50)
                    {
                        isTookPlace.Append("В соответствии  со ст. 45 ЖК РФ общее собрание правомочно принимать решения по вопросам повестки дня, поставленным на голосование, кворум имеется.\n");
                    }
                    else
                    {
                        isTookPlace.Append("В соответствии  со ст. 45 ЖК РФ общее собрание не правомочно принимать решения по вопросам повестки дня, поставленным на голосование, кворум не имеется.\n");
                    }

                    Paragraph notification = document.InsertParagraph();
                    notification.Append("Уведомление о проведении общего собрания собственников доведены до сведения собственников помещений за 10 дней.\n");

                    Paragraph agenda = document.InsertParagraph();
                    agenda.Append("ПОВЕСТКА ДНЯ ОБЩЕГО СОБРАНИЯ\n").Bold().Alignment = Alignment.center;
                    if (meeting.Questions != null)
                    {
                        foreach (var question in meeting.Questions)
                        {
                            document.InsertParagraph().Append($"{question.Number}. {question.Agenda}").Alignment = Alignment.both;
                        }
                    }

                    if (meeting.ArchivalInformationOfMeeting.ParticipatingArea / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100 >= 50)
                    {
                        Paragraph results = document.InsertParagraph();
                        results.Append("\nРЕЗУЛЬТАТЫ ГОЛОСОВАНИЯ\n").Bold().Alignment = Alignment.center;
                        if (meeting.Questions != null)
                        {
                            foreach (var question in meeting.Questions)
                            {
                                bool quorum = (question.MeetingResult.AreaFor + question.MeetingResult.AreaAgainst + question.MeetingResult.AreaAbstained) / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100 > 50 ? true : false;

                                document.InsertParagraph().Append($"{question.Number}. {question.Agenda}").Bold().Alignment = Alignment.both;
                                document.InsertParagraph().Append($"Предложено: {question.Proposed}").Alignment = Alignment.both;
                                document.InsertParagraph().Append("").Alignment = Alignment.both;
                                Table result = document.AddTable(3, 3);
                                result.Rows[0].Cells[0].Paragraphs.First().Append("ЗА");
                                result.Rows[1].Cells[0].Paragraphs.First().Append("ПРОТИВ");
                                result.Rows[2].Cells[0].Paragraphs.First().Append("ВОЗДЕРЖАЛСЯ");
                                result.Rows[0].Cells[1].Paragraphs.First().Append($"{question.MeetingResult.AreaFor}").Alignment = Alignment.center;
                                result.Rows[1].Cells[1].Paragraphs.First().Append($"{question.MeetingResult.AreaAgainst}").Alignment = Alignment.center;
                                result.Rows[2].Cells[1].Paragraphs.First().Append($"{question.MeetingResult.AreaAbstained}").Alignment = Alignment.center;
                                if (question.DecisionType == 0)
                                {
                                    result.Rows[0].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaFor / (question.MeetingResult.AreaFor + question.MeetingResult.AreaAgainst + question.MeetingResult.AreaAbstained) * 100, 2)}%").Alignment = Alignment.center;
                                    result.Rows[1].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaAgainst / (question.MeetingResult.AreaFor + question.MeetingResult.AreaAgainst + question.MeetingResult.AreaAbstained) * 100, 2)}%").Alignment = Alignment.center;
                                    result.Rows[2].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaAbstained / (question.MeetingResult.AreaFor + question.MeetingResult.AreaAgainst + question.MeetingResult.AreaAbstained) * 100, 2)}%").Alignment = Alignment.center;
                                }
                                else
                                {
                                    result.Rows[0].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaFor / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100, 2)}%").Alignment = Alignment.center;
                                    result.Rows[1].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaAgainst / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100, 2)}%").Alignment = Alignment.center;
                                    result.Rows[2].Cells[2].Paragraphs.First().Append($"{Math.Round(question.MeetingResult.AreaAbstained / meeting.ArchivalInformationOfMeeting.TotalAreaHouse * 100, 2)}%").Alignment = Alignment.center;
                                }
                                document.InsertTable(result);

                                switch (question.DecisionType)
                                {
                                    case 0:
                                        if (question.MeetingResult.AreaFor > question.MeetingResult.AreaAgainst
                                            && question.MeetingResult.AreaFor > question.MeetingResult.AreaAbstained
                                            && quorum)
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append($"Решили: {question.Proposed}").Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        else
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня НЕ ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        break;
                                    case 1:
                                        if (question.MeetingResult.AreaFor / meeting.ArchivalInformationOfMeeting.TotalAreaHouse > (decimal)0.5
                                            && quorum)
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append($"Решили: {question.Proposed}").Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        else
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня НЕ ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        break;
                                    case 2:
                                        if (question.MeetingResult.AreaFor / meeting.ArchivalInformationOfMeeting.TotalAreaHouse > 2 / 3
                                            && quorum)
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append($"Решили: {question.Proposed}").Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        else
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня НЕ ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        break;
                                    case 3:
                                        if (question.MeetingResult.AreaFor / meeting.ArchivalInformationOfMeeting.TotalAreaHouse >= 1
                                            && quorum)
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append($"Решили: {question.Proposed}").Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        else
                                        {
                                            document.InsertParagraph().Append("Решение по вопросу повестки дня НЕ ПРИНЯТО.").Bold().Alignment = Alignment.both;
                                            document.InsertParagraph().Append("").Alignment = Alignment.both;
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    document.InsertParagraph().Append($"Председатель общего собрания:\n");
                    document.InsertParagraph().Append($"подпись: ______________ {meeting.Chairperson} дата:_____________\n").Alignment = Alignment.right;
                    document.InsertParagraph().Append($"Секретарь общего собрания:\n");
                    document.InsertParagraph().Append($"подпись: ______________ {meeting.Secretary} дата:_____________\n").Alignment = Alignment.right;
                    document.InsertParagraph().Append($"Члены счетной комиссии общего собрания:\n");
                    foreach (var member in meeting.CountingCommitteeMembers)
                    {
                        document.InsertParagraph().Append($"подпись: ______________ {member.FullName} дата:_____________\n").Alignment = Alignment.right;
                    }

                    document.Save();

                    return memoryStream;
                }
            }
        }
    }
}
