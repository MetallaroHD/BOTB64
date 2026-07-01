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
        public required int PlayerId { get; init; }
        public required string PublicEndpoint { get; set; } // ip:port, for NAT punch handoff
        public bool IsHost { get; set; }
    }
}
