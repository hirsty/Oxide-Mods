using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Libraries;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Data;
using UnityEngine;
using Rust;



namespace Oxide.Plugins
{
    [Info("Timed Notifications", "Hirsty", 1.0)]
    [Description("Depending on Preset Times, this plugin will display a popup with a notification.")]
    class TimedNotifications : RustPlugin
    {
        public static string version = "1.0.0";
        public bool PopUpNotifier = true;
        public string[] days;
        public string TZ;
        public int hour;
        public int min;

        [PluginReference]
        Plugin PopupNotifications;
        class EventData
        {
            public EventData()
            {
               
            }
        }
        class StoredData
        {
            public HashSet<PlayerInfo> Players = new HashSet<PlayerInfo>();

            public StoredData()
            {
            }
        }

        class PlayerInfo
        {
            public string UserId;
            public string Name;

            public PlayerInfo()
            {
            }

            public PlayerInfo(BasePlayer player)
            {
                UserId = player.userID.ToString();
                Name = player.displayName;
                
            }
        }
        EventData eventData;
        protected override void LoadDefaultConfig() {
            PrintWarning("Whoops! No config file, lets create a new one!"); // Runs when no configuration file has been found
            Config.Clear();
            Config["Plugin","Version"] = version;
            //Config["Events", "TimeZone"] = "GMT";
            Config["Events", "AllowedDays"] = "Mon,Tue,Wed,Thu,Fri,Sat,Sun";
            Config["Plugin", "PopUpTime"] = "20";
            Config["Plugin", "EnablePopUps"]=true;
            SaveConfig();
        }
        private void Loaded() => LoadData(); // What to do when plugin loaded

        private void LoadData()
        {
            // Load Imformation from a Config file
            if (!PopupNotifications)
            {
                Puts("PopupNotifications not found! Using text based notifications!");
                PopUpNotifier = false;
            } else
            {
                if(Config["Plugin", "EnablePopups"] != null)
                {
                    PopUpNotifier = (bool)Config["Plugin", "EnablePopups"];
                }
            }
            string[] seperator = new string[] { "," };
            // Build array of events
            string daystring = (string)Config["Events", "AllowedDays"];
            days = daystring.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            //Puts(days.GetValue(0).ToString());
            //PopupNotifications.Call("CreatePopupNotification", "Test message");
            eventData = Interface.GetMod().DataFileSystem.ReadObject<EventData>("MyEvents");
           
        }

        [ChatCommand("notification")] // Whatever cammand you want the player to type
        private void notification(BasePlayer player, string command, string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    SendHelp(player);
                    break;
                case 1:
                    switch (args[1])
                    {
                        case "list":
                            // List Notifications
                            break;
                        case "reset":
                            // Remove all notifications
                            break;
                        default:
                            SendHelp(player);
                            break;
                    }
                    break;
                case 2:
                    switch (args[1])
                    {
                        case "add":
                            // Add a Notification
                            break;
                        case "remove":
                            // Remove a Notification]
                            break;
                        default:
                            SendHelp(player);
                            break;
                    }
                    break;
            }
            // Function for the chat command
        }
        void OnServerInitialized()
        {
            // Called when server is booted up
        }

        void OnTick()
        {
            if (!days.Contains(DateTime.Now.ToUniversalTime().ToString("ddd")))
            {
                return;
            } else {

            }
            // Do something  on server "Tick" - defined by tickrate
        }

        void OnPluginUnloaded()
        {
            // Called when plugin is unloaded - clean up here
        }
        void SendHelp(BasePlayer player)
        {
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
            SendChatMessage(player, "TimedNotify", "Help Text");
        }

        //---------------------------->   Chat Sending   <----------------------------//

        void SendNotification(string message, int delay)
        {
            if (PopUpNotifier)
            {
                PopupNotifications.Call("CreatePopupNotification", message,null, delay);
            }
            else
            {
                BroadcastChat("Notification", message);
            }
        }
        void BroadcastChat(string prefix, string msg)
        {
            PrintToChat("<color=orange>" + prefix + "</color>: " + msg);
        }

        void SendChatMessage(BasePlayer player, string prefix, string msg)
        {
            SendReply(player, "<color=orange>" + prefix + "</color>: " + msg);
        }

        //---------------------------------------------------------------------------//
    }
}
