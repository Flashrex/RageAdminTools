using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RageAdminTools {
    class Commands : Script{

        public static Dictionary<string, string> Teleportation = new Dictionary<string, string>() {
            { "gotocoords", "Teleportiere dich zu Koordinaten." },
            { "goto", "Teleportiere dich zu Spieler." },
            { "gethere", "Teleportiere Spieler zu dir." },
        };

        public static Dictionary<string, string> Debug = new Dictionary<string, string>() {
            { "savepos", "Speichere deine aktuelle Position in einer .txt-Datei." },
            { "freecam", "Aktiviert/Deaktiviert den Freecam-Modus." },
            { "tv", "Spectate einen Spieler." }
        };

        public static Dictionary<string, string> Vehicle = new Dictionary<string, string>() {
            { "veh", "Spawne ein Fahrzeug." },
            { "fixveh", "Repariere dein Fahrzeug." }
        };

        public static Dictionary<string, string> Stats = new Dictionary<string, string>() {
            { "heal", "Heale dich oder einen Spieler." },
            { "sethp", "Setze deine HP oder die eines anderen Spielers." },
            { "setarmor", "Setze deine Armor oder die eines anderen Spielers." }
        };

        public static Dictionary<string, string> Misc = new Dictionary<string, string>() {
            { "debug", "Aktiviert/Deaktiviert den Debug Modus." },
            { "aw", "Sende eine Nachricht an einen Spieler." },
            { "weapon", "Gib dir oder einem anderen Spieler eine Waffe" }
        };

        public static Dictionary<string, Dictionary<string, string>> Categorys = new Dictionary<string, Dictionary<string, string>>() {
            { "Teleportation", Teleportation },
            { "Debug", Debug },
            { "Vehicle", Vehicle },
            { "Stats", Stats },
            { "Misc", Misc }
        };

        [Command("help", GreedyArg = true)]
        public void CMD_Help(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive)) return;

            if (input.Length == 0) {
                string msg = "~b~[AdminTools] ~w~Kategorien: ";
                foreach (KeyValuePair<string, Dictionary<string, string>> pair in Categorys) {
                    msg += pair.Key + " ";
                }
                player.SendChatMessage("~b~[AdminTools] ~w~Wähle eine Kategorie mit /help [Kategorie]");
                player.SendChatMessage(msg);
                return;
            }
            string[] args = input.Split(" ");

            if (!Categorys.TryGetValue(args[0], out Dictionary<string, string> dic)) {
                player.SendChatMessage("~b~[AdminTools] ~w~Ungültige Kategorie: " + args[0]);
                return;
            }
            foreach (KeyValuePair<string, string> pair in dic) {
                player.SendChatMessage($"~b~[AdminTools] ~w~/{pair.Key} - {pair.Value}");
            }
        }

        [Command("debug")]
        public void CMD_Debug(Player player) {
            if (!player.HasData(EntityData.IsDebugActive)) return;

            bool IsDebugActive = player.GetData<bool>(EntityData.IsDebugActive);

            IsDebugActive = !IsDebugActive;
            player.SendNotification("Debug: " + (IsDebugActive ? "~g~ Aktiviert" : "~r~ Deaktiviert"));
            player.SetData(EntityData.IsDebugActive, IsDebugActive);
        }

        #region Teleportation
        [Command("goto", GreedyArg = true)]
        public void CMD_Goto(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /goto [SpielerName]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            Player target = players.First();
            if (target.Id == player.Id) return;

            if (player.IsInVehicle) player.Vehicle.Position = target.Position.Around(2);
            else player.Position = target.Position.Around(2);
        }

        [Command("gethere", GreedyArg = true)]
        public void CMD_Gethere(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /gethere [SpielerName]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            Player target = players.First();
            if (target.Id == player.Id) return;

            if (target.IsInVehicle) target.Vehicle.Position = player.Position.Around(2);
            else target.Position = player.Position.Around(2);
        }

        [Command("gotocoords", GreedyArg = true)]
        public void CMD_GotoCoords(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 3) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /gotocoords [x] [y] [z]");
                return;
            }

            double x, y, z;
            try {
                x = Convert.ToDouble(args[0]);
                y = Convert.ToDouble(args[1]);
                z = Convert.ToDouble(args[2]);

            } catch {
                player.SendChatMessage("~b~[AdminTools] ~w~Eingabe war in einem ungültigen Format!");
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /gotocoords [x] [y] [z]");
                return;
            }

            if (player.IsInVehicle) player.Vehicle.Position = new Vector3(x, y, z);
            else player.Position = new Vector3(x, y, z);

            player.SendNotification($"Teleportiert: {args[0]}, {args[1]}, {args[2]}");
        }
        #endregion

        #region DebugTools
        [Command("freecam")]
        public void CMD_FreeCam(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            if (input.Length != 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /freecam");
                return;
            }

            player.TriggerEvent(ClientEvents.Event_Freecam);
            player.SetSharedData(EntityData.IsInvisible, true);
        }

        [Command("savepos", GreedyArg = true, Alias = "position, pos, getpos")]
        public void CMD_SavePos(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (input.Length == 0 || args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /savepos [positionsName]");
                return;
            }

            string msg, position;
            if (player.IsInVehicle) {
                position = $"{player.Vehicle.Position.X.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Vehicle.Position.Y.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Vehicle.Position.Z.ToString(new CultureInfo("en-US"))} | "
                              + $"{ player.Vehicle.Rotation.X.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Vehicle.Rotation.Y.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Vehicle.Rotation.Z.ToString(new CultureInfo("en-US"))}";
                msg = $"[InVehiclePosition] {args[0]} : {position}";
            } else {
                position = $"{player.Position.X.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Position.Y.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Position.Z.ToString(new CultureInfo("en-US"))} | "
                              + $"{ player.Rotation.X.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Rotation.Y.ToString(new CultureInfo("en-US"))}, "
                              + $"{ player.Rotation.Z.ToString(new CultureInfo("en-US"))}";

                msg = $"[OnFootPosition] {args[0]} : {position}";
            }

            if (Utility.WriteToTextFile("./rageadmintools/", msg)) {
                player.SendChatMessage(msg);
            }

        }

        [Command("tv", GreedyArg = true, Alias = "spec")]
        public void CMD_TV(Player player, string input) {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");
            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /tv [Spielername]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            Player target = players.First();
            if (target.Id == player.Id) return;
            player.SetData(EntityData.SpecatingPosition, player.Position);

            player.Position = target.Position.Add(new Vector3(0, 0, 50));
            player.TriggerEvent(ClientEvents.Event_Spectate, target.Id);
            player.SetSharedData(EntityData.IsInvisible, true);
        }
        #endregion

        #region Vehicle
        [Command("veh", GreedyArg = true, Alias = "spawncar")]
        public void CMD_Vehicle(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /vehicle [vehicleName]");
                return;
            }

            Vehicle veh;
            if (player.HasData(EntityData.DebugVehicle)) {
                veh = player.GetData<Vehicle>(EntityData.DebugVehicle);
                veh.Delete();
            }

            VehicleHash hash = NAPI.Util.VehicleNameToModel(args[0]);
            if (hash == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Ungültiger Fahrzeugname!");
                return;
            }

            veh = NAPI.Vehicle.CreateVehicle(hash, player.Position.Around(5.0f), player.Rotation, 0, 0, "ATools");
            veh.CustomPrimaryColor = new Color(0, 53, 227);
            veh.CustomSecondaryColor = new Color(0, 53, 227);
            player.SetIntoVehicle(veh, 0);
            player.SetData(EntityData.DebugVehicle, veh);
            player.SendNotification("Fahrzeug gespawnt: ~b~" + veh.DisplayName);
        }

        [Command("fixveh", Alias = "afixveh, repair, repairveh")]
        public void CMD_FixVeh(Player player) {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            if (!player.IsInVehicle) {
                player.SendChatMessage("~b~[AdminTools] ~w~Du befindest dich in keinem Fahrzeug!");
                return;
            }

            player.Vehicle.Repair();
            player.SendNotification("Fahrzeug repariert");
        }
        #endregion

        #region stats
        [Command("heal")]
        public void CMD_Heal(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            if (input.Length == 0) {
                player.Health = 100;
                player.Armor = 100;
                player.SendNotification("Heal");
                return;
            }

            string[] args = input.Split(" ");

            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /heal [Spielername]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            Player target = players.First();
            target.Health = 100;
            target.Armor = 100;
            player.SendNotification($"~b~{target.Name} ~w~geheilt.");
        }

        [Command("sethp", GreedyArg = true)]
        public void CMD_SetHp(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 2) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /sethp [Spielername] [Lebenspunkte]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            int health = int.Parse(args[1]);
            if (health < 0 || health > 100) {
                player.SendChatMessage($"~b~[AdminTools] ~w~Lebenspunkte müssen zwischen 0 und 100 sein.");
                return;
            }

            Player target = players.First();
            target.Health = health;
            player.SendNotification($"{target.Name}: ~b~{health} ~w~HP");
        }

        [Command("setarmor", GreedyArg = true)]
        public void CMD_SetArmor(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 2) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /setarmor [Spielername] [Armorpunkte]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            int armor = int.Parse(args[1]);
            if (armor < 0 || armor > 100) {
                player.SendChatMessage($"~b~[AdminTools] ~w~Armorpunkte müssen zwischen 0 und 100 sein.");
                return;
            }

            Player target = players.First();
            target.Armor = armor;
            player.SendNotification($"{target.Name}: ~b~{armor} ~w~Armor");
        }
        #endregion

        #region Misc
        [Command("weapon", GreedyArg = true, Alias = "giveweapon")]
        public void CMD_Weapon(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /weapon [Waffenname]");
                return;
            }

            WeaponHash hash = NAPI.Util.WeaponNameToModel(args[0]);
            if (hash == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Ungültiger Waffenname!");
                return;
            }

            player.GiveWeapon(hash, 5000);
            player.SendNotification($"Waffe erhalten: ~b~{input}");
        }

        [Command("aw", GreedyArg = true)]
        public void CMD_Aw(Player player, string input = "") {
            if (!player.HasData(EntityData.IsDebugActive) || !player.GetData<bool>(EntityData.IsDebugActive)) return;

            string[] args = input.Split(" ");

            if (args.Length < 2) {
                player.SendChatMessage("~b~[AdminTools] ~w~Nutze: /aw [Spielername] [Nachricht]");
                return;
            }

            List<Player> players = Utility.GetPlayerByName(args[0]);
            if (players.Count == 0) {
                player.SendChatMessage("~b~[AdminTools] ~w~Es konnte kein Spieler mit diesem Namen gefunden werden.");
                return;

            } else if (players.Count > 1) {
                player.SendChatMessage("~b~[AdminTools] ~w~Der Name war nicht eindeutig.");
                string names = "";
                foreach (Player client in players) {
                    names += client + " ";
                }
                player.SendChatMessage($"~b~[AdminTools] ~w~Gefundene Spieler: {names}");
                return;
            }

            string msg = "";
            for (int i = 1; i < args.Length; i++) {
                msg += args[i] + " ";
            }

            players.First().SendChatMessage($"~b~[AdminTools] ~w~{player.Name} flüstert: {msg}");
        }
        #endregion

        [RemoteEvent("AdminTools:StopSpectate")]
        public void OnStopSpectate(Player player) {
            if(player.HasData(EntityData.SpecatingPosition)) {
                player.Position = player.GetData<Vector3>(EntityData.SpecatingPosition);
                player.ResetData(EntityData.SpecatingPosition);
            }

            if (player.HasSharedData(EntityData.IsInvisible)) player.ResetSharedData(EntityData.IsInvisible);
        }
    }
}
