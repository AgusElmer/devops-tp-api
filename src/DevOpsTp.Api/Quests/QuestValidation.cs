namespace DevOpsTp.Api.Quests;

public static class QuestValidation
{
    public static string[] ValidateCreate(CreateQuestRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors.Add("title is required");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add("description is required");
        }

        if (!QuestRanks.All.Contains(request.Rank?.Trim().ToUpperInvariant()))
        {
            errors.Add("rank must be one of: E, D, C, B, A, S");
        }

        if (!QuestTypes.All.Contains(request.Type?.Trim().ToLowerInvariant()))
        {
            errors.Add("type must be one of: combat, gathering, exploration, delivery, rescue, crafting");
        }

        if (request.RewardGold < 0)
        {
            errors.Add("rewardGold cannot be negative");
        }

        if (request.RewardXp < 0)
        {
            errors.Add("rewardXp cannot be negative");
        }

        return errors.ToArray();
    }
}
