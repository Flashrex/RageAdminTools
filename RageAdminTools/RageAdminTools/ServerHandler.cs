using GTANetworkAPI;
using System;

namespace RageAdminTools {
    class ServerHandler : Script {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart() {
            Console.WriteLine("[AdminTools] Erfolreich geladen.");
        }
    }
}
