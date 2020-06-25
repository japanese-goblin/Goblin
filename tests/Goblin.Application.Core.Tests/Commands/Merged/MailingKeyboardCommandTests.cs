﻿using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged
{
    public class MailingKeyboardCommandTests : TestBase
    {
        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new MailingKeyboardCommand();
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
            result.Keyboard.Should().NotBeNull();
        }
    }
}