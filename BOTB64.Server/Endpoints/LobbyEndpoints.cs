using BOTB64.Server.Lobbies;
using BOTB64.Shared;

namespace BOTB64.Server.Endpoints
{
    public static class LobbyEndpoints
    {
        public static void MapLobbyEndpoints(this WebApplication app)
        {
            app.MapPost("/lobby/create", (CreateLobbyRequest req, LobbyManager lobbies, HttpContext ctx) =>
            {
                var endpoint = ctx.Connection.RemoteIpAddress + ":" + ctx.Connection.RemotePort;
                var lobby = lobbies.CreateCustomLobby(req.PlayerId, endpoint, req.GameSizeType);
                return Results.Ok(new { lobby.LobbyId, lobby.JoinCode });
            });

            app.MapPost("/lobby/join", (JoinLobbyRequest req, LobbyManager lobbies, HttpContext ctx) =>
            {
                var endpoint = ctx.Connection.RemoteIpAddress + ":" + ctx.Connection.RemotePort;
                var (ok, lobby, error) = lobbies.JoinByCode(req.JoinCode, req.PlayerId, endpoint);
                return ok
                    ? Results.Ok(new JoinLobbyResponse(lobby!.LobbyId, lobby.Players.Select(p => new LobbyPlayerDto(p.PlayerId, p.IsHost)).ToList(), lobby.Players.First(p => p.IsHost).PlayerId))
                    : Results.BadRequest(new { error });
            });

            app.MapGet("/lobby/{id}", (Guid id, LobbyManager lobbies) =>
            {
                var lobby = lobbies.FindByMatchID(id);
                return lobby != null ? Results.Ok(lobby) : Results.NotFound();
            });

            app.MapPost("/lobby/{id}/leave", (Guid id, LeaveLobbyRequest req, LobbyManager lobbies) =>
            {
                lobbies.Leave(id, req.PlayerId);
                return Results.Ok();
            });
        }
    }

    public record LobbyPlayerDto(int PlayerId, bool IsHost);
    public record JoinLobbyResponse(Guid LobbyId, List<LobbyPlayerDto> Players, int HostPlayerId);
    public record CreateLobbyRequest(int PlayerId, GameSizeType GameSizeType);
    public record JoinLobbyRequest(int PlayerId, string JoinCode);
    public record LeaveLobbyRequest(int PlayerId);
}