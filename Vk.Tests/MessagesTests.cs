using System;
using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Vk.Tests
{
    public class MessagesTests : TestBase
    {
        [Fact(DisplayName = "messages.send correct")]
        public async void Send__Correct()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/messages.send.json"));

                var result = await GetApi("messages.send").Messages.Send(1, "test");

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .Times(1);

                Assert.True(result.MessageId >= 0);
                Assert.True(result.PeerId >= 0);
            }
        }

        [Fact(DisplayName = "messages.send throws error")]
        public async void Send__Incorrect()
        {
            var api = new VkApi("token");
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await api.Messages.Send(new long[] { }, "asd"));
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await api.Messages.Send(1, ""));
        }

        [Fact(DisplayName = "messages.delete correct")]
        public async void Delete_Correct()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/messages.delete.json"));

                var api = new VkApi("token");
                await api.Messages.Delete(1);

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .Times(1);
            }
        }

        [Fact(DisplayName = "messages.delete throws error")]
        public async void Delete_Incorrect()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi("messages.delete").Messages.Delete(new long[] { }));
        }
    }
}