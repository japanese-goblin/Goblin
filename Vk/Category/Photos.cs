using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk.Category
{
    public class Photos
    {
        private readonly VkApi _api;

        public Photos(VkApi api)
        {
            _api = api;
        }

        public async Task<string> FastUploadPhoto(long peerId, byte[] image)
        {
            var server = await _api.Photos.GetMessagesUploadServer(peerId);
            var upload = await _api.Photos.UploadImage(server.UploadUrl, image);
            var save = await _api.Photos.SaveMessagesPhoto(upload);

            return $"photo{save.OwnerId}_{save.Id}_{save.AccessKey}";
        }

        public async Task<UploadServerInfo> GetMessagesUploadServer(long peerId)
        {
            var values = new Dictionary<string, string>
            {
                ["peer_id"] = peerId.ToString()
            };
            var res = await _api.CallApi<UploadServerInfo>("photos.getMessagesUploadServer", values);
            return res;
        }

        public async Task<UploadImageInfo> UploadImage(string url, byte[] data)
        {
            using(var client = new HttpClient())
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
            var values = new Dictionary<string, string>
            {
                ["photo"] = info.Photo,
                ["server"] = info.Server.ToString(),
                ["hash"] = info.Hash
            };

            var res = await _api.CallApi<Photo>("photos.saveMessagesPhoto", values);
            return res;
        }
    }
}