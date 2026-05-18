namespace DevOpsTp.Api.Quests;

public sealed class Quest
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Rank { get; init; }
    public required string Type { get; init; }
    public string Status { get; set; } = QuestStatuses.Available;
    public int RewardGold { get; init; }
    public int RewardXp { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? AbandonedAt { get; set; }
}

public static class QuestRanks
{
    public static readonly string[] All = ["E", "D", "C", "B", "A", "S"];
}

public static class QuestTypes
{
    public static readonly string[] All =
    [
        "combat",
        "gathering",
        "exploration",
        "delivery",
        "rescue",
        "crafting"
    ];
}

public static class QuestStatuses
{
    public const string Available = "available";
    public const string Accepted = "accepted";
    public const string Completed = "completed";
    public const string Abandoned = "abandoned";

    public static readonly string[] All = [Available, Accepted, Completed, Abandoned];
}

public sealed record QuestSummary(
    IReadOnlyDictionary<string, int> CountsByStatus,
    IReadOnlyDictionary<string, int> CountsByRank,
    int TotalCompletedQuests,
    int TotalGoldRewarded,
    int TotalXpRewarded);
