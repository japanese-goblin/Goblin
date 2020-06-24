﻿using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text
{
    public class RemoveKeyboardCommandTests : TestBase
    {
        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new RemoveKeyboardCommand();
            var text = command.Aliases[0];
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);
            
            var result = await command.Execute<VkBotUser>(message, DefaultUser);

            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
            result.Keyboard.Should().NotBeNull();
            result.Keyboard.Buttons.Should().HaveCount(1);
        }
    }
}