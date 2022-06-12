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
        if(!req.UserId.HasValue)
        {
            BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToAll(req.Message, req.Attachments,
                                                                     req.SendStartKeyboard, req.ConsumerType));
            return SendOkAsync(ct);
        }

        BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToId(req.UserId.Value, req.Message, req.Attachments, req.ConsumerType));
        return SendOkAsync(ct);
    }
}

public class SendRequest
{
    public long? UserId { get; set; }

    public ConsumerType ConsumerType { get; set; }
    public string Message { get; set; }

    public ICollection<string> Attachments { get; set; }
    public bool SendStartKeyboard { get; set; }

    public SendRequest()
    {
        Attachments = Array.Empty<string>();
    }
}

public class SendRequestValidator : Validator<SendRequest>
{
    public SendRequestValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
    }
}