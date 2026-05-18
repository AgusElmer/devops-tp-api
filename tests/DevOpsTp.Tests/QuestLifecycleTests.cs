using System.Net;
using System.Net.Http.Json;
using DevOpsTp.Api.Quests;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevOpsTp.Tests;

public class QuestLifecycleTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public QuestLifecycleTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AcceptQuest_ChangesAvailableQuestToAccepted()
    {
        var created = await CreateQuest();

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/accept", null);
        var quest = await response.Content.ReadFromJsonAsync<Quest>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(quest);
        Assert.Equal(QuestStatuses.Accepted, quest.Status);
        Assert.NotNull(quest.AcceptedAt);
        Assert.Null(quest.CompletedAt);
        Assert.Null(quest.AbandonedAt);
    }

    [Fact]
    public async Task CompleteQuest_ChangesAcceptedQuestToCompleted()
    {
        var created = await CreateQuest();
        await _client.PatchAsync($"/api/quests/{created.Id}/accept", null);

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/complete", null);
        var quest = await response.Content.ReadFromJsonAsync<Quest>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(quest);
        Assert.Equal(QuestStatuses.Completed, quest.Status);
        Assert.NotNull(quest.AcceptedAt);
        Assert.NotNull(quest.CompletedAt);
        Assert.Null(quest.AbandonedAt);
    }

    [Fact]
    public async Task AbandonQuest_ChangesAcceptedQuestToAbandoned()
    {
        var created = await CreateQuest();
        await _client.PatchAsync($"/api/quests/{created.Id}/accept", null);

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/abandon", null);
        var quest = await response.Content.ReadFromJsonAsync<Quest>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(quest);
        Assert.Equal(QuestStatuses.Abandoned, quest.Status);
        Assert.NotNull(quest.AcceptedAt);
        Assert.Null(quest.CompletedAt);
        Assert.NotNull(quest.AbandonedAt);
    }

    [Theory]
    [InlineData("complete")]
    [InlineData("abandon")]
    public async Task AvailableQuest_CannotBeCompletedOrAbandoned(string action)
    {
        var created = await CreateQuest();

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/{action}", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("accept")]
    [InlineData("complete")]
    [InlineData("abandon")]
    public async Task CompletedQuest_CannotChangeStateAgain(string action)
    {
        var created = await CreateQuest();
        await _client.PatchAsync($"/api/quests/{created.Id}/accept", null);
        await _client.PatchAsync($"/api/quests/{created.Id}/complete", null);

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/{action}", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("accept")]
    [InlineData("complete")]
    [InlineData("abandon")]
    public async Task AbandonedQuest_CannotChangeStateAgain(string action)
    {
        var created = await CreateQuest();
        await _client.PatchAsync($"/api/quests/{created.Id}/accept", null);
        await _client.PatchAsync($"/api/quests/{created.Id}/abandon", null);

        var response = await _client.PatchAsync($"/api/quests/{created.Id}/{action}", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task QuestSummary_ReturnsCountsAndCompletedRewards()
    {
        var completed = await CreateQuest(rank: "S", rewardGold: 500, rewardXp: 1200);
        var accepted = await CreateQuest(rank: "A", rewardGold: 200, rewardXp: 300);
        var abandoned = await CreateQuest(rank: "E", rewardGold: 50, rewardXp: 25);
        await CreateQuest(rank: "S", rewardGold: 999, rewardXp: 999);

        await _client.PatchAsync($"/api/quests/{completed.Id}/accept", null);
        await _client.PatchAsync($"/api/quests/{completed.Id}/complete", null);
        await _client.PatchAsync($"/api/quests/{accepted.Id}/accept", null);
        await _client.PatchAsync($"/api/quests/{abandoned.Id}/accept", null);
        await _client.PatchAsync($"/api/quests/{abandoned.Id}/abandon", null);

        var summary = await _client.GetFromJsonAsync<QuestSummary>("/api/quests/summary");

        Assert.NotNull(summary);
        Assert.Equal(1, summary.CountsByStatus[QuestStatuses.Available]);
        Assert.Equal(1, summary.CountsByStatus[QuestStatuses.Accepted]);
        Assert.Equal(1, summary.CountsByStatus[QuestStatuses.Completed]);
        Assert.Equal(1, summary.CountsByStatus[QuestStatuses.Abandoned]);
        Assert.Equal(2, summary.CountsByRank["S"]);
        Assert.Equal(1, summary.CountsByRank["A"]);
        Assert.Equal(1, summary.CountsByRank["E"]);
        Assert.Equal(1, summary.TotalCompletedQuests);
        Assert.Equal(500, summary.TotalGoldRewarded);
        Assert.Equal(1200, summary.TotalXpRewarded);
    }

    private async Task<Quest> CreateQuest(
        string rank = "B",
        int rewardGold = 250,
        int rewardXp = 900)
    {
        var request = new CreateQuestRequest(
            "Recover the lost deployment artifact",
            "Find the missing artifact and return it to the release guild.",
            rank,
            "exploration",
            rewardGold,
            rewardXp);

        var response = await _client.PostAsJsonAsync("/api/quests", request);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Quest>())!;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
