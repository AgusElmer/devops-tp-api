using System.Net;
using System.Net.Http.Json;
using DevOpsTp.Api.Quests;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DevOpsTp.Tests;

public class QuestBaseTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public QuestBaseTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetQuests_ReturnsEmptyList_WhenNoQuestsWereCreated()
    {
        var quests = await _client.GetFromJsonAsync<Quest[]>("/api/quests");

        Assert.NotNull(quests);
        Assert.Empty(quests);
    }

    [Fact]
    public async Task CreateQuest_ReturnsCreatedQuest_WithAvailableStatus()
    {
        var request = NewQuestRequest(rank: "a", type: "Combat");

        var response = await _client.PostAsJsonAsync("/api/quests", request);
        var quest = await response.Content.ReadFromJsonAsync<Quest>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(quest);
        Assert.True(quest.Id > 0);
        Assert.Equal(request.Title, quest.Title);
        Assert.Equal(request.Description, quest.Description);
        Assert.Equal("A", quest.Rank);
        Assert.Equal("combat", quest.Type);
        Assert.Equal(QuestStatuses.Available, quest.Status);
        Assert.Equal(request.RewardGold, quest.RewardGold);
        Assert.Equal(request.RewardXp, quest.RewardXp);
        Assert.NotEqual(default, quest.CreatedAt);
        Assert.Null(quest.AcceptedAt);
        Assert.Null(quest.CompletedAt);
        Assert.Null(quest.AbandonedAt);
    }

    [Fact]
    public async Task GetQuestById_ReturnsCreatedQuest()
    {
        var created = await CreateQuest();

        var quest = await _client.GetFromJsonAsync<Quest>($"/api/quests/{created.Id}");

        Assert.NotNull(quest);
        Assert.Equal(created.Id, quest.Id);
        Assert.Equal(created.Title, quest.Title);
    }

    [Fact]
    public async Task GetQuestById_ReturnsNotFound_WhenQuestDoesNotExist()
    {
        var response = await _client.GetAsync("/api/quests/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateQuest_ReturnsBadRequest_WhenRequiredFieldsAreInvalid()
    {
        var request = new CreateQuestRequest(
            Title: "",
            Description: " ",
            Rank: "Z",
            Type: "unknown",
            RewardGold: -1,
            RewardXp: -10);

        var response = await _client.PostAsJsonAsync("/api/quests", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<Quest> CreateQuest()
    {
        var response = await _client.PostAsJsonAsync("/api/quests", NewQuestRequest());

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Quest>())!;
    }

    private static CreateQuestRequest NewQuestRequest(
        string title = "Clear the haunted observatory",
        string description = "Defeat the corrupted spirits before sunrise.",
        string rank = "B",
        string type = "combat",
        int rewardGold = 250,
        int rewardXp = 900)
    {
        return new CreateQuestRequest(title, description, rank, type, rewardGold, rewardXp);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
