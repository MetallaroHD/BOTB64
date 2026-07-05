using BOTB64.Server.Lobbies;
using BOTB64.Shared;

namespace BOTB64.Server.Endpoints
{
    public static class LobbyEndpoints
    {
        public static void MapLobbyEndpoints(this WebApplication app)
        {
            app.MapPost("/lobby/{id}/start", (Guid id, StartLobbyRequest req, LobbyManager lobbies) =>
            {
                var ok = lobbies.MarkStarted(id, req.PlayerId);
                return ok ? Results.Ok() : Results.BadRequest(new { error = "not host, or lobby not found" });
            });

            app.MapPost("/lobby/create", (CreateLobbyRequest req, LobbyManager lobbies, HttpContext ctx) =>
            {
                var endpoint = ctx.Connection.RemoteIpAddress + ":" + ctx.Connection.RemotePort;

                var lobby = lobbies.CreateCustomLobby(
                    req.PlayerId,
                    endpoint,
                    req.DisplayName,
                    req.GameSizeType);

                return Results.Ok(new CreateLobbyResponse(lobby.LobbyId, lobby.JoinCode!));
            });

            app.MapPost("/lobby/join", (JoinLobbyRequest req, LobbyManager lobbies, HttpContext ctx) =>
            {
                var endpoint = ctx.Connection.RemoteIpAddress + ":" + ctx.Connection.RemotePort;

                var (ok, lobby, error) = lobbies.JoinByCode(
                    req.JoinCode,
                    req.PlayerId,
                    endpoint,
                    req.DisplayName);

                return ok
                    ? Results.Ok(new JoinLobbyResponse(
                        lobby!.LobbyId,
                        lobby.JoinCode!,
                        lobby.Players.Select(p => new LobbyPlayerDto(p.PlayerId, p.DisplayName, p.IsHost)).ToList(),
                        lobby.Players.First(p => p.IsHost).PlayerId))
                    : Results.BadRequest(new { error });
            });

            app.MapGet("/lobby/{id}", (Guid id, LobbyManager lobbies) =>
            {
                var lobby = lobbies.FindByMatchID(id);
                if (lobby == null) return Results.NotFound();
                return Results.Ok(new LobbyDto(
                    lobby.JoinCode!,
                    lobby.Players.Select(p => new LobbyPlayerDto(p.PlayerId, p.DisplayName, p.IsHost)).ToList(),
                    lobby.HostPlayerID,
                    lobby.GameSizeType,
                    lobby.IsFull,
                    lobby.Started));
            });

            app.MapPost("/lobby/{id}/leave", (Guid id, LeaveLobbyRequest req, LobbyManager lobbies) =>
            {
                lobbies.Leave(id, req.PlayerId);
                return Results.Ok();
            });

            app.MapPost("/lobby/{id}/mode", (Guid id, SetModeRequest req, LobbyManager lobbies) =>
            {
                var ok = lobbies.SetGameSizeType(id, req.PlayerId, req.GameSizeType);
                return ok ? Results.Ok() : Results.BadRequest(new { error = "not host, or lobby not found" });
            });
        }
    }

    // DTOs
    public record SetModeRequest(int PlayerId, GameSizeType GameSizeType);

    public record CreateLobbyRequest(int PlayerId, string DisplayName, GameSizeType GameSizeType);

    public record JoinLobbyRequest(int PlayerId, string DisplayName, string JoinCode);

    public record LeaveLobbyRequest(int PlayerId);

    public record LobbyPlayerDto(int PlayerId, string DisplayName, bool IsHost);

    public record JoinLobbyResponse(Guid LobbyId, string JoinCode, List<LobbyPlayerDto> Players, int HostPlayerId);

    public record CreateLobbyResponse(Guid LobbyId, string JoinCode);

    public record LobbyDto(string JoinCode, List<LobbyPlayerDto> Players, int HostPlayerId, GameSizeType GameSizeType, bool IsFull, bool Started);

    public record StartLobbyRequest(int PlayerId);
}