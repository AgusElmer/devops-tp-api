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

        return group;
    }
}
