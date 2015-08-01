using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Libraries;
using System.Collections.Generic;
using System.Globalization;
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
        public DateTime setDate;
        public int min;
        char sep = '/';
        char sep2 = ':';

[PluginReference]
        Plugin PopupNotifications;
        
        class StoredData
        {
            public HashSet<EventData> Events = new HashSet<EventData>();

            public StoredData()
            {
            }
        }
        class EventData
        {
            public DateTime EventDate;
            public string EventInfo;
            public bool broadcast;
            public EventData()
            {

            }
            public EventData(DateTime Date, string info)
            {
               this.EventDate = Date;
               this.EventInfo = info;
                this.broadcast = false;
            }
        }
        StoredData storedData;
        protected override void LoadDefaultConfig() {
            PrintWarning("Whoops! No config file, lets create a new one!"); // Runs when no configuration file has been found
            Config.Clear();
            Config["Plugin","Version"] = version;
            //Config["Events", "TimeZone"] = "GMT";
            Config["Events", "AllowedDays"] = "Mon,Tue,Wed,Thu,Fri,Sat,Sun";
            Config["Plugin", "PopUpTime"] = 20;
            Config["Plugin", "TimeCheck"] = 10;
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


            //PopupNotifications.Call("CreatePopupNotification", "Test message",null,10);
            string[] seperator = new string[] { "," };
            // Build array of events
            int tick;
            if (Config["Plugin", "TimeCheck"] != null)
            {
                tick = (int)Config["Plugin", "TimeCheck"];
            } else
            {
                tick = 10;
            }
            string daystring = (string)Config["Events", "AllowedDays"];
            days = daystring.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            //Puts(days.GetValue(0).ToString());
            //PopupNotifications.Call("CreatePopupNotification", "Test message");
            storedData = Interface.GetMod().DataFileSystem.ReadObject <StoredData>("MyEvents");
            timer.Repeat(tick, 0, () => EventCheck());

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
                    switch (args[0])
                    {
                        case "list":
                            // List Notifications
                            break;
                        case "reset":
                            // Remove all notifications
                            storedData.Events.Clear();
                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            SendChatMessage(player, "TimeNotify:", "Events have been removed!");
                            break;
                        default:
                            SendHelp(player);
                            break;
                    }
                    break;
                case 2:
                case 3:
                case 4:
                    switch (args[0])
                    {
                        case "add":
                            string[] datepart = args[1].Split(sep);
                            string[] timepart = args[2].Split(sep2);
                            try
                            {
                                setDate = new DateTime(datepart[2].ToInt(), datepart[1].ToInt(), datepart[0].ToInt(), timepart[0].ToInt(), timepart[1].ToInt(), 0);
                            } catch
                            {
                                SendHelp(player);
                            }
                            
                            string eventinfo = args[3];
                            var info = new EventData(setDate,eventinfo);
                            storedData.Events.Add(info);
                            SendChatMessage(player, "TimedNotify", "Event Saved");
                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
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

        void EventCheck()
        {
            if (!days.Contains(DateTime.Now.ToUniversalTime().ToString("ddd")))
            {
                return;
            } else {
                foreach(var storedEvent in storedData.Events)
                {
                    if(storedEvent.EventDate.Date <= DateTime.UtcNow.Date && !storedEvent.broadcast)
                    {
                        SendNotification(storedEvent.EventInfo, (int)Config["Plugin", "PopUpTime"]);
                        storedEvent.broadcast = true;
                        Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                    }

                    if (storedEvent.EventDate.Date <= DateTime.UtcNow.Date && !storedEvent.broadcast)
                    {
                        if (storedEvent.EventDate.Hour <= DateTime.UtcNow.Hour && !storedEvent.broadcast)
                        {
                            if (storedEvent.EventDate.Minute <= DateTime.UtcNow.Minute && !storedEvent.broadcast)
                            {
                                SendNotification(storedEvent.EventInfo, (int)Config["Plugin", "PopUpTime"]);
                                storedEvent.broadcast = true;
                                Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            }
                        }
                    }
                }
                // Check for Events on that day

            }
        }

        void OnPluginUnloaded()
        {
            // Called when plugin is unloaded - clean up here
        }
        void SendHelp(BasePlayer player)
        {
            SendChatMessage(player, "TimedNotify", "/notification add <DD/MM/YY> <HH:MM> \"<MESSAGE>\". To schedule a notification for the specified time");
        }

        //---------------------------->   Chat Sending   <----------------------------//

        void SendNotification(string message, int delay=5,BasePlayer player=null)
        {
            if (PopUpNotifier)
            {
                
                PopupNotifications.Call("CreatePopupNotification", "<color=orange>Notification:</color> " + message, player, delay);
               
            }
            else
            {
                if (player != null)
                {
                    SendChatMessage(player, "Notification", message);
                } else
                {
                    BroadcastChat("Notification", message);
                }
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
