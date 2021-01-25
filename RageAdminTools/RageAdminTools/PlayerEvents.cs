using GTANetworkAPI;

namespace RageAdminTools {
    class PlayerEvents : Script{

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player) {
            player.SetData(EntityData.IsDebugActive, false);

            player.SendChatMessage("~b~[AdminTools] ~w~Nutze /debug um den Debug-Modus zu aktivieren.");
            player.SendChatMessage("~b~[AdminTools] ~w~Nutze /help um eine Übersicht über die Funktionen zu bekommen.");
        }
    }
}
