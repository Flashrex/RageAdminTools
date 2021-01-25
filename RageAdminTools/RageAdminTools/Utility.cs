using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace RageAdminTools {
    class Utility : Script {

        public static List<Player> GetPlayerByName(string input) {

            List<Player> players = new List<Player>();
            foreach (Player player in NAPI.Pools.GetAllPlayers()) {
                if (player.Name.ToLower().Contains(input.ToLower())) players.Add(player);
            }

            return players;
        }

        public static bool WriteToTextFile(string path, string msg) {
            try {
                using (StreamWriter file =
                    new StreamWriter(@$"{path}positions.txt", true)) {
                    file.WriteLine(msg);
                    return true;
                }
            } catch(Exception e) {
                Console.WriteLine($"[Fehler] WriteToTextFile: {e.StackTrace}");
                Console.WriteLine($"[Fehler] WriteToTextFile: {e.Message}");
            }
            return false;
        }
    }
}
