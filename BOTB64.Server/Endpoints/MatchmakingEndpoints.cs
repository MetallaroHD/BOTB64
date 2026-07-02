using BOTB64.Server.Matchmaking;
using BOTB64.Shared;

namespace BOTB64.Server.Endpoints
{
    public static class MatchmakingEndpoints
    {
        public static void MapMatchmakingEndpoints(this WebApplication app)
        {
            app.MapPost("/matchmaking/queue", (QueueRequest req, MatchmakingQueue queue, HttpContext ctx) =>
            {
                var endpoint = ctx.Connection.RemoteIpAddress + ":" + ctx.Connection.RemotePort;
                var lobbyId = queue.Enqueue(req.PlayerId, endpoint, req.GameSizeType);
                return Results.Ok(new { lobbyId }); // null = still waiting, client keeps polling
            });
        }
    }

    public record QueueRequest(int PlayerId, GameSizeType GameSizeType);
}