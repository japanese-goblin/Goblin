using System;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace Goblin.Application.Vk
{
    public static class AttachmentsParser
    {
        public static MediaAttachment[] StringsToAttachments(string[] attachmentsNames)
        {
            var attachTypes = new Dictionary<string, Type>
            {
                ["photo"] = typeof(Photo),
                ["poll"] = typeof(Poll),
                ["video"] = typeof(Video),
                ["wall"] = typeof(Wall),
                ["doc"] = typeof(Document),
                ["audio"] = typeof(Audio),
                ["market"] = typeof(Market)
            };
            var attachmentList = new List<MediaAttachment>();

            foreach(var attachType in attachTypes)
            {
                var selected = attachmentsNames
                               .Where(x => x.StartsWith(attachType.Key))
                               .Select(x =>
                               {
                                   var attach = Activator.CreateInstance(attachType.Value) as MediaAttachment;
                                   var data = x.Replace(attachType.Key, string.Empty)
                                               .Split('_');

                                   attach.OwnerId = long.Parse(data[0]);
                                   attach.Id = long.Parse(data[1]);
                                   return attach;
                               }).ToArray();
                if(!selected.Any())
                {
                    continue;
                }

                attachmentList.AddRange(selected);
            }

            return attachmentList.ToArray();
        }
    }
}