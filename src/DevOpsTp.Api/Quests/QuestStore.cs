namespace DevOpsTp.Api.Quests;

public sealed class QuestStore
{
    private readonly List<Quest> _quests = [];
    private readonly object _sync = new();
    private int _nextId = 1;

    public IReadOnlyCollection<Quest> GetAll()
    {
        lock (_sync)
        {
            return _quests
                .OrderBy(quest => quest.Id)
                .Select(Clone)
                .ToList();
        }
    }

    public Quest? GetById(int id)
    {
        lock (_sync)
        {
            var quest = _quests.FirstOrDefault(item => item.Id == id);
            return quest is null ? null : Clone(quest);
        }
    }

    public Quest Create(CreateQuestRequest request)
    {
        var quest = new Quest
        {
            Id = GetNextId(),
            Title = request.Title!.Trim(),
            Description = request.Description!.Trim(),
            Rank = request.Rank!.Trim().ToUpperInvariant(),
            Type = request.Type!.Trim().ToLowerInvariant(),
            Status = QuestStatuses.Available,
            RewardGold = request.RewardGold,
            RewardXp = request.RewardXp,
            CreatedAt = DateTimeOffset.UtcNow
        };

        lock (_sync)
        {
            _quests.Add(quest);
        }

        return Clone(quest);
    }

    public Quest? Accept(int id)
    {
        lock (_sync)
        {
            var quest = _quests.FirstOrDefault(item => item.Id == id);
            if (quest is null || quest.Status != QuestStatuses.Available)
            {
                return null;
            }

            quest.Status = QuestStatuses.Accepted;
            quest.AcceptedAt = DateTimeOffset.UtcNow;
            return Clone(quest);
        }
    }

    public Quest? Complete(int id)
    {
        lock (_sync)
        {
            var quest = _quests.FirstOrDefault(item => item.Id == id);
            if (quest is null || quest.Status != QuestStatuses.Accepted)
            {
                return null;
            }

            quest.Status = QuestStatuses.Completed;
            quest.CompletedAt = DateTimeOffset.UtcNow;
            return Clone(quest);
        }
    }

    public Quest? Abandon(int id)
    {
        lock (_sync)
        {
            var quest = _quests.FirstOrDefault(item => item.Id == id);
            if (quest is null || quest.Status != QuestStatuses.Accepted)
            {
                return null;
            }

            quest.Status = QuestStatuses.Abandoned;
            quest.AbandonedAt = DateTimeOffset.UtcNow;
            return Clone(quest);
        }
    }

    public QuestSummary GetSummary()
    {
        lock (_sync)
        {
            var countsByStatus = QuestStatuses.All.ToDictionary(
                status => status,
                status => _quests.Count(quest => quest.Status == status));

            var countsByRank = QuestRanks.All.ToDictionary(
                rank => rank,
                rank => _quests.Count(quest => quest.Rank == rank));

            var completedQuests = _quests
                .Where(quest => quest.Status == QuestStatuses.Completed)
                .ToList();

            return new QuestSummary(
                countsByStatus,
                countsByRank,
                completedQuests.Count,
                completedQuests.Sum(quest => quest.RewardGold),
                completedQuests.Sum(quest => quest.RewardXp));
        }
    }

    private int GetNextId()
    {
        lock (_sync)
        {
            return _nextId++;
        }
    }

    private static Quest Clone(Quest quest)
    {
        return new Quest
        {
            Id = quest.Id,
            Title = quest.Title,
            Description = quest.Description,
            Rank = quest.Rank,
            Type = quest.Type,
            Status = quest.Status,
            RewardGold = quest.RewardGold,
            RewardXp = quest.RewardXp,
            CreatedAt = quest.CreatedAt,
            AcceptedAt = quest.AcceptedAt,
            CompletedAt = quest.CompletedAt,
            AbandonedAt = quest.AbandonedAt
        };
    }
}
