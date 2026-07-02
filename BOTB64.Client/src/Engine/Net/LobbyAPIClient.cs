using System.Net.Http.Json;
using BOTB64.Shared;

namespace BOTB64.Engine.Net
{
    public class LobbyApiClient
    {
        private readonly HttpClient _http;

        public LobbyApiClient(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<(Guid lobbyId, string joinCode)> CreateLobby(int playerId, string name, GameSizeType sizeType)
        {
            var resp = await _http.PostAsJsonAsync("/lobby/create",
                new { playerId, displayName = name, gameSizeType = sizeType });

            resp.EnsureSuccessStatusCode();

            var result = await resp.Content.ReadFromJsonAsync<CreateLobbyResponse>();
            return (result!.LobbyId, result.JoinCode);
        }

        public async Task<JoinLobbyResponse?> JoinLobby(int playerId, string name, string joinCode)
        {
            var resp = await _http.PostAsJsonAsync("/lobby/join",
                new { playerId, displayName = name, joinCode });

            if (!resp.IsSuccessStatusCode) return null;

            return await resp.Content.ReadFromJsonAsync<JoinLobbyResponse>();
        }

        public async Task<LobbyDto?> GetLobby(Guid lobbyId)
        {
            var resp = await _http.GetAsync($"/lobby/{lobbyId}");
            if (!resp.IsSuccessStatusCode) return null;

            return await resp.Content.ReadFromJsonAsync<LobbyDto>();
        }

        public async Task<bool> SetLobbyMode(Guid lobbyId, int playerId, GameSizeType sizeType)
        {
            var resp = await _http.PostAsJsonAsync($"/lobby/{lobbyId}/mode",
                new { playerId, gameSizeType = sizeType });

            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveLobby(Guid lobbyId, int playerId)
        {
            var resp = await _http.PostAsJsonAsync($"/lobby/{lobbyId}/leave",
                new { playerId });

            return resp.IsSuccessStatusCode;
        }
    }

    public record JoinLobbyResponse(Guid LobbyId, string JoinCode, List<LobbyPlayerDto> Players, int HostPlayerId);
    public record CreateLobbyResponse(Guid LobbyId, string JoinCode);
    public record LobbyDto(string JoinCode, List<LobbyPlayerDto> Players, int HostPlayerId, GameSizeType GameSizeType, bool IsFull);
    public record LobbyPlayerDto(int PlayerId, string DisplayName, bool IsHost);
}