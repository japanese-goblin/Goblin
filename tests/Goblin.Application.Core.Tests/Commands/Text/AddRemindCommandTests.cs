using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text
{
    public class AddRemindCommandTests : TestBase
    {
        [Theory]
        [InlineData("сегодня")]
        [InlineData("завтра")]
        [InlineData("21.01.2150")]
        public async Task ShouldReturnSuccessfulResult(string date)
        {
            var command = new AddRemindCommand(ApplicationContext);
            var text = $"{command.Aliases[0]} {date} 23:59 тест";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnFailedResult_Because_NotEnoughParameters()
        {
            var command = new AddRemindCommand(ApplicationContext);
            var text = $"{command.Aliases[0]} сегодня 23:59";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            
            result.Should().BeOfType<FailedResult>("Нужно указать три параметра");
            result.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnFailedResult_Because_DateIsOlderThanNow()
        {
            var command = new AddRemindCommand(ApplicationContext);
            var text = $"{command.Aliases[0]} 01.01.2010 23:59 тест";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);
        
            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            
            result.Should().BeOfType<FailedResult>("Дата меньше текущей");
            result.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task ShouldReturnFailedResult_Because_DateIsIncorrect()
        {
            var command = new AddRemindCommand(ApplicationContext);
            var text = $"{command.Aliases[0]} 45.01.2010 23:59 тест";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);
        
            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            
            result.Should().BeOfType<FailedResult>("Некорректная дата");
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task TaskShouldReturnFailedResult_Because_MaxRemindsCount()
        {
            var command = new AddRemindCommand(ApplicationContext);
            var text = $"{command.Aliases[0]} 01.01.2101 23:59 тест";
            var message = GenerateMessage(DefaultUserWithMaxReminds.Id, DefaultUserWithMaxReminds.Id, text, string.Empty);
        
            var result = await command.Execute<VkBotUser>(message, DefaultUserWithMaxReminds);
            
            result.Should().BeOfType<FailedResult>("У пользователя максимум напоминаний");
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}