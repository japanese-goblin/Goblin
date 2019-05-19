using System;
using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Vk.Tests
{
    public class MessagesTests : TestBase
    {
        [Fact]
        public async void Send_CorrectData_SendMessage()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/messages/send.json"));

                var result = await GetApi().Messages.Send(1, "test");

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .WithQueryParamValue("message", "test")
                        .WithQueryParamValue("peer_ids", 1)
                        .Times(1);

                Assert.True(result.MessageId >= 0);
                Assert.True(result.PeerId >= 0);
            }
        }
        
        [Fact]
        public async void Send_EmptyIds_ThrowsError()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi().Messages.Send(new long[] { }, "asd"));
        }

        [Fact]
        public async void Send_EmptyTextAndAttachs_ThrowsError()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi().Messages.Send(1, ""));
        }
        
        [Fact]
        public async void Delete_CorrectId_DeleteMessage()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("data/messages/delete.json"));
                
                await GetApi().Messages.Delete(1);

                httpTest.ShouldHaveCalled($"{VkApi.EndPoint}*")
                        .WithVerb(HttpMethod.Post)
                        .WithQueryParamValue("message_ids", 1)
                        .Times(1);
            }
        }
        
        [Fact]
        public async void Delete_EmptyIdList_ThrowsError()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi().Messages.Delete(new long[] { }));
        }
    }
}