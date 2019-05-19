using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Vk.Models;
using Xunit;

namespace Vk.Tests
{
    public class PhotosTests : TestBase
    {
        private const int PeerId = 1;

        [Fact]
        public async void GetMessagesUploadServer_CorrectData()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/photos/getMessagesUploadServer.json"));

                var result = await GetApi().Photos.GetMessagesUploadServer(1);

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .WithRequestBody("peer_id=1")
                        .Times(1);

                Assert.Equal(-64, result.AlbumId);
                Assert.NotEmpty(result.UploadUrl);
                Assert.Contains($"peer_id={PeerId}", result.UploadUrl);
            }
        }

        [Fact]
        public async void SaveMessagesPhoto_CorrectData()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/photos/saveMessagesPhoto.json"));

                var result = await GetApi().Photos.SaveMessagesPhoto(new UploadImageInfo()); //TODO:

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .Times(1);

                Assert.NotEmpty(result);
                var photo = result[0];
                Assert.NotNull(photo);
            }
        }
    }
}