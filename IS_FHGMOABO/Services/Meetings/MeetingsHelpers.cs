﻿using IS_FHGMOABO.DAL;
using IS_FHGMOABO.Models.MeetingsModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Xceed.Words.NET;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace IS_FHGMOABO.Services.Meetings
{
    public class MeetingsHelpers
    {
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

        public static List<House>? DedeserializeHouses(HttpContext httpContext)
        {
            var serializedModel = httpContext.Session.GetString("AddHouses");
            var deserializeModel = JsonConvert.DeserializeObject<List<House>>(serializedModel);

            return deserializeModel;
        }

        public static void SerializeHouses(AddMeetingModel model, HttpContext httpContext)
        {
            var serializedModel = JsonConvert.SerializeObject(model.Houses);
            httpContext.Session.SetString("AddHouses", serializedModel);
        }
    }
}
