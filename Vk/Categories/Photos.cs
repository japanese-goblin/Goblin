using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk
{
    public class Photos
    {
        public async Task<UploadServerInfo> GetMessagesUploadServer(long peerId)
        {
            var values = new Dictionary<string, string>()
            {
                ["peer_id"] = peerId.ToString()
            };
            var res = await VkApi.SendRequest("photos.getMessagesUploadServer", values);
            return JsonConvert.DeserializeObject<UploadServerInfo>(res);
        }

        public async Task<UploadImageInfo> UploadImage(string url, byte[] data)
        {
            using (var client = new HttpClient())
            {
                var requestContent = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(data);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                requestContent.Add(imageContent, "photo", "image.jpg");

                var response = await client.PostAsync(url, requestContent);

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var responseString = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                return JsonConvert.DeserializeObject<UploadImageInfo>(responseString);
            }
        }

        public async Task<Photo> SaveMessagesPhoto(UploadImageInfo info)
        {
            var values = new Dictionary<string, string>()
            {
                ["photo"] = info.Photo,
                ["server"] = info.Server.ToString(),
                ["hash"] = info.Hash
            };

            var res = await VkApi.SendRequest("photos.saveMessagesPhoto", values);
            return JsonConvert.DeserializeObject<Photo>(res.Substring(1, res.Length-2));
        }
    }
}