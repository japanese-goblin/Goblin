namespace Goblin.BackgroundJobs;

internal static class Defaults
{
    public const int ChunkLimit = 100; // максимум 100 ID в сообщении
    public static readonly TimeSpan DelayBetweenSends = TimeSpan.FromSeconds(1.5);
}
