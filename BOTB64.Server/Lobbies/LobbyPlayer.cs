using BOTB64.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Server.Lobbies
{
    public class LobbyPlayer
    {
        public int PlayerId { get; set; }
        public string DisplayName { get; set; } = "";
        public string PublicEndpoint { get; set; } = "";
        public bool IsHost { get; set; }
    }
}
