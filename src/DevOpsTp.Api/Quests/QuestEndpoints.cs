namespace DevOpsTp.Api.Quests;

public static class QuestEndpoints
{
    public static RouteGroupBuilder MapQuestEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/quests")
            .WithTags("Quests");

        group.MapGet("/", (QuestStore store) =>
        {
            return Results.Ok(store.GetAll());
        })
        .WithName("GetQuests");

        group.MapGet("/{id:int}", (int id, QuestStore store) =>
        {
            var quest = store.GetById(id);
            return quest is null ? Results.NotFound() : Results.Ok(quest);
        })
        .WithName("GetQuestById");

        group.MapGet("/summary", (QuestStore store) =>
        {
            return Results.Ok(store.GetSummary());
        })
        .WithName("GetQuestSummary");

        group.MapPost("/", (CreateQuestRequest request, QuestStore store) =>
        {
            var errors = QuestValidation.ValidateCreate(request);
            if (errors.Length > 0)
            {
                return Results.BadRequest(new { errors });
            }

            var quest = store.Create(request);
            return Results.Created($"/api/quests/{quest.Id}", quest);
        })
        .WithName("CreateQuest");

        group.MapPatch("/{id:int}/accept", (int id, QuestStore store) =>
        {
            var existingQuest = store.GetById(id);
            if (existingQuest is null)
            {
                return Results.NotFound();
            }

            var quest = store.Accept(id);
            return quest is null
                ? Results.Conflict(new { error = "only available quests can be accepted" })
                : Results.Ok(quest);
        })
        .WithName("AcceptQuest");

        group.MapPatch("/{id:int}/complete", (int id, QuestStore store) =>
        {
            var existingQuest = store.GetById(id);
            if (existingQuest is null)
            {
                return Results.NotFound();
            }

            var quest = store.Complete(id);
            return quest is null
                ? Results.Conflict(new { error = "only accepted quests can be completed" })
                : Results.Ok(quest);
        })
        .WithName("CompleteQuest");

        group.MapPatch("/{id:int}/abandon", (int id, QuestStore store) =>
        {
            var existingQuest = store.GetById(id);
            if (existingQuest is null)
            {
                return Results.NotFound();
            }

            var quest = store.Abandon(id);
            return quest is null
                ? Results.Conflict(new { error = "only accepted quests can be abandoned" })
                : Results.Ok(quest);
        })
        .WithName("AbandonQuest");

        return group;
    }
}
