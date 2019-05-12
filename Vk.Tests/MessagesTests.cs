using System;
using Xunit;

namespace Vk.Tests
{
    public class MessagesTests : TestBase
    {
        [Fact(DisplayName = "messages.send correct")]
        public async void Send__Correct()
        {
            var result = await GetApi("messages.send").Messages.Send(1, "test");
            Assert.True(result.MessageId >= 0);
            Assert.True(result.PeerId >= 0);
        }

        [Fact(DisplayName = "messages.send throws error")]
        public async void Send__Incorrect()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi("messages.send").Messages.Send(new long[] { }, ""));
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi("messages.send").Messages.Send(1, ""));
        }

        [Fact(DisplayName = "messages.delete correct")]
        public async void Delete_Correct()
        {
            await GetApi("messages.delete").Messages.Delete(1);
        }

        [Fact(DisplayName = "messages.delete throws error")]
        public async void Delete_Incorrect()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await GetApi("messages.delete").Messages.Delete(new long[] {}));
        }
    }
}