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

        public async Task<(Guid lobbyId, string joinCode)> CreateLobby(int playerId, GameSizeType sizeType)
        {
            var resp = await _http.PostAsJsonAsync("/lobby/create", new { playerId, gameSizeType = sizeType });
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<CreateLobbyResponse>();
            return (result!.LobbyId, result.JoinCode);
        }

        public async Task<LobbyDto?> GetLobby(Guid lobbyId)
        {
            var resp = await _http.GetAsync($"/lobby/{lobbyId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<LobbyDto>();
        }

        public async Task<JoinLobbyResponse?> JoinLobby(int playerId, string joinCode)
        {
            var resp = await _http.PostAsJsonAsync("/lobby/join", new { playerId, joinCode });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<JoinLobbyResponse>();
        }
    }

    public record JoinLobbyResponse(Guid LobbyId, List<LobbyPlayerDto> Players, int HostPlayerId);
    public record CreateLobbyResponse(Guid LobbyId, string JoinCode);
    public record LobbyDto(List<LobbyPlayerDto> Players, int HostPlayerId, GameSizeType GameSizeType, bool IsFull);
    public record LobbyPlayerDto(int PlayerId, bool IsHost);
}