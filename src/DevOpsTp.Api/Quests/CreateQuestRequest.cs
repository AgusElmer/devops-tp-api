namespace DevOpsTp.Api.Quests;

public sealed record CreateQuestRequest(
    string? Title,
    string? Description,
    string? Rank,
    string? Type,
    int RewardGold,
    int RewardXp);
