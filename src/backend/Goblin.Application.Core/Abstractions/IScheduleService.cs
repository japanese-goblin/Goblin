using System;
using System.Threading.Tasks;

namespace Goblin.Application.Core.Abstractions;

public interface IScheduleService
{
    public Task<IResult> GetSchedule(int narfuGroup, DateTime date);
}