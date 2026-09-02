using Goblin.Domain;

namespace Goblin.Application.Core;

public record FlowExecutionResult(FlowType FlowType, string? FlowState, bool IsSuccessful, string Message, CoreKeyboard? Keyboard);
