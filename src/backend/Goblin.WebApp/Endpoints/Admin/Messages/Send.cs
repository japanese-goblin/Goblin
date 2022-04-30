using FastEndpoints;
using FluentValidation;
using Goblin.BackgroundJobs.Jobs;
using Goblin.Domain;
using Hangfire;

namespace Goblin.WebApp.Endpoints.Admin.Messages;

public class Send : Endpoint<SendRequest>
{

    public override void Configure()
    {
        Post("admin/messages/send");
    }

    public override Task HandleAsync(SendRequest req, CancellationToken ct)
    {
        //TODO: send to selected users
        BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToAll(req.Message, req.Attachments,
                                                                 req.SendStartKeyboard, req.ConsumerType));
        return SendOkAsync(ct);
        // if (req.ConsumerType == ConsumerType.AllInOne)
        // {
        //     var groups = _context.BotUsers.AsEnumerable()
        //                         .Select(x => new { x.Id, x.ConsumerType })
        //                         .ToArray();
        //     foreach (var groupedUsers in groups)
        //     {
        //         var sender = _senders.First(x => x.ConsumerType == groupedUsers.ConsumerType);
        //         await sender.SendToMany(groups.Select(x => x.Id), req.Message);
        //     }
        // }
        // else
        // {
        //     var sender = _senders.First(x => x.ConsumerType == req.ConsumerType);
        //     var users = req.UserIds.Any() ?
        //         req.UserIds :
        //         _context.BotUsers.Where(x => x.ConsumerType == req.ConsumerType)
        //                 .Select(x => x.Id)
        //                 .ToArray();
        //     
        //     await sender.SendToMany(users, req.Message);
        // }
    }
}

public class SendRequest
{
    public long[] UserIds { get; set; }

    public ConsumerType ConsumerType { get; set; }
    public string Message { get; set; }

    public string[] Attachments { get; set; }
    public bool SendStartKeyboard { get; set; }

    public SendRequest()
    {
        UserIds = Array.Empty<long>();
    }
}

public class SendRequestValidator : Validator<SendRequest>
{
    public SendRequestValidator()
    {
        RuleFor(x => x.UserIds).NotNull()
                               .NotEmpty()
                               .When(x => x.ConsumerType != ConsumerType.AllInOne);
        RuleFor(x => x.ConsumerType).NotEmpty();
        RuleFor(x => x.Message).NotEmpty();
    }
}