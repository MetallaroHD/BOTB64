using BOTB64.Shared;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BOTB64.Engine.Net
{
    public class LobbyApiClient
    {

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        static LobbyApiClient()
        {
            JsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private readonly HttpClient HTTP;

        public LobbyApiClient(string baseUrl)
        {
            HTTP = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<(Guid lobbyId, string joinCode)> CreateLobby(int playerId, string displayName, GameSizeType sizeType)
        {
            var resp = await HTTP.PostAsJsonAsync("/lobby/create", new
            {
                playerId,
                displayName,
                gameSizeType = sizeType,
                versionMajor = BOTBVersion.Major,
                versionMinor = BOTBVersion.Minor,
                versionPatch = BOTBVersion.Patch
            });
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(err?.Error ?? "Failed to create lobby");
            }
            var result = await resp.Content.ReadFromJsonAsync<CreateLobbyResponse>();
            return (result!.LobbyId, result.JoinCode);
        }

        public async Task<JoinLobbyResponse?> JoinLobby(int playerId, string name, string joinCode)
        {
            var resp = await HTTP.PostAsJsonAsync("/lobby/join",
                new { playerId, displayName = name, joinCode,
                    versionMajor = BOTBVersion.Major,
                    versionMinor = BOTBVersion.Minor,
                    versionPatch = BOTBVersion.Patch
                });

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(err?.Error ?? "Failed to create lobby");
            }

            return await resp.Content.ReadFromJsonAsync<JoinLobbyResponse>();
        }

        public async Task<LobbyDto?> GetLobby(Guid lobbyId)
        {
            var resp = await HTTP.GetAsync($"/lobby/{lobbyId}");
            if (!resp.IsSuccessStatusCode)
                return null;

            return await resp.Content.ReadFromJsonAsync<LobbyDto>(JsonOptions);
        }

        public async Task<bool> SetLobbyMode(Guid lobbyId, int playerId, GameSizeType sizeType)
        {
            var resp = await HTTP.PostAsJsonAsync($"/lobby/{lobbyId}/mode",
                new { playerId, gameSizeType = sizeType });

            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveLobby(Guid lobbyId, int playerId)
        {
            var resp = await HTTP.PostAsJsonAsync($"/lobby/{lobbyId}/leave",
                new { playerId });

            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> StartLobby(Guid lobbyId, int playerId)
        {
            var resp = await HTTP.PostAsJsonAsync($"/lobby/{lobbyId}/start", new { playerId });
            return resp.IsSuccessStatusCode;
        }
    }

    public record JoinLobbyResponse(Guid LobbyId, string JoinCode, List<LobbyPlayerDto> Players, int HostPlayerId);
    public record CreateLobbyResponse(Guid LobbyId, string JoinCode);
    public record LobbyDto(List<LobbyPlayerDto> Players, int HostPlayerId, GameSizeType GameSizeType, bool IsFull, bool Started);
    public record LobbyPlayerDto(int PlayerId, string DisplayName, bool IsHost);
    public record ErrorResponse(string Error);
}