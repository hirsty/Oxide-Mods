using Oxide.Core;
using Oxide.Core.Plugins;
using System.Collections.Generic;
using System;
using UnityEngine;
using Oxide.Game.Rust.Libraries;


namespace Oxide.Plugins
{
    [Info("Timed Notifications", "Hirsty", "0.0.2", ResourceId = 1277)]
    [Description("Depending on Preset Times, this plugin will display a popup with a notification.")]
    class TimedNotifications : RustPlugin
    {
        public static string version = "0.0.2";
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
            public string CommandLine;
            public bool broadcast;
            public EventData()
            {

            }
            public EventData(DateTime Date, string info, string EventType = null)
            {
                this.EventDate = Date;
                this.broadcast = false;
                if(EventType == "notification")
                {
                    this.EventInfo = info;
                    this.CommandLine = "";
                } else if(EventType == "cmd")
                {
                    this.EventInfo = "";
                    this.CommandLine = info;
                }
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

        bool hasPermission(BasePlayer player, string permname)
        {
            if (player.net.connection.authLevel > 1)
                return true;
            return permission.UserHasPermission(player.userID.ToString(), permname);
        }
        private void LoadData()
        {
            storedData = Interface.GetMod().DataFileSystem.ReadObject<StoredData>("MyEvents");
            if (Convert.ToString(Config["Plugin", "Version"]) != version)
            {
                Puts("Uh oh! Not up to date! No Worries, lets update you!");
                switch(version)
                {
                    case "0.0.1":
                        Config["Plugin", "Version"] = version;
                        break;
                    case "0.0.2":
                        Config["Plugin", "Version"] = version;
                        Puts("Updating your Data File! Hang On!");
                        foreach (var storedEvent in storedData.Events)
                        {
                           
                            if (storedEvent.CommandLine == null)
                            {
                                storedEvent.CommandLine = "";
                            }
                        }
                        Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                        break;
                    default:
                        Config["Plugin", "Version"] = version;
                        break;
                }
                SaveConfig();

            }
            permission.RegisterPermission("canplanevent", this);
            // Load Imformation from a Config file
            if (!PopupNotifications)
            {
                Puts("PopupNotifications not found! Using text based notifications!");
                PopUpNotifier = false;
            } else
            {
                
                if (Convert.ToBoolean(Config["Plugin", "EnablePopUps"]).ToString() != null)
                {
                    PopUpNotifier = Convert.ToBoolean(Config["Plugin", "EnablePopUps"]);
                }
            }
            string[] seperator = new string[] { "," };
            // Build array of events
            int tick;
            if (Convert.ToInt16(Config["Plugin", "TimeCheck"]) > 0)
            {
                tick = Convert.ToInt16(Config["Plugin", "TimeCheck"]);
            } else
            {
                tick = 10;
            }
            string daystring = Convert.ToString(Config["Events", "AllowedDays"]);
            days = daystring.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            timer.Repeat(tick, 0, () => EventCheck());
        }

        [ChatCommand("notification")] // Whatever cammand you want the player to type
        private void notification(BasePlayer player, string command, string[] args)
        {
            if(!hasPermission(player, "canplanevent")) { SendChatMessage(player, "Timed Notifications", "You don't have access to this command"); return; }
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
                            int count = 0;
                            foreach (var storedEvent in storedData.Events)
                            {
                                if (!storedEvent.broadcast)
                                {
                                    count++;
                                    SendChatMessage(player, "", count + "- (D: " + storedEvent.EventDate.Day + "/" + storedEvent.EventDate.Month + "/" + storedEvent.EventDate.Year + " T: " + storedEvent.EventDate.Hour + ":" + storedEvent.EventDate.Minute + ") " + storedEvent.EventInfo);
                                }
                            }
                                break;
                        case "reset":
                            // Remove all notifications
                            storedData.Events.Clear();
                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            SendChatMessage(player, "Timed Notifications", "Events have been removed!");
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
                            }
                            catch
                            {
                                SendHelp(player);
                            }

                            string eventinfo = args[3];
                            var info = new EventData(setDate, eventinfo, "notification");
                            storedData.Events.Add(info);
                            SendChatMessage(player, "Timed Notifications", "Event Saved");
                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            break;
                        case "addcmd":
                            string[] datepart2 = args[1].Split(sep);
                            string[] timepart2 = args[2].Split(sep2);
                            try
                            {
                                setDate = new DateTime(datepart2[2].ToInt(), datepart2[1].ToInt(), datepart2[0].ToInt(), timepart2[0].ToInt(), timepart2[1].ToInt(), 0);
                            }
                            catch
                            {
                                SendHelp(player);
                            }

                            string cmdinfo = args[3];
                            var infocmd = new EventData(setDate, cmdinfo,"cmd");
                            storedData.Events.Add(infocmd);
                            SendChatMessage(player, "Timed Notifications", "Event Saved");
                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            break;
                        //                        case "remove":

                        // List Notifications
                        //                            int count = 0;
                        //                            foreach (var storedEvent in storedData.Events)
                        //                            {
                        //                                if (!storedEvent.broadcast)
                        //                                {
                        //                                    count++;
                        //                                    if(count.ToString() == args[1])
                        //                                    {
                        //                                        storedEvent.broadcast = false;
                        //                                  }
                        //                               }
                        //                            }
                        //                            Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                        //                            break;
                        default:
                            SendHelp(player);
                            break;
                    }
                    break;
                default:
                    SendHelp(player);
                    break;
            }
            // Function for the chat command
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
                        if (storedEvent.CommandLine == "")
                        {
                            SendNotification(storedEvent.EventInfo, Convert.ToInt16(Config["Plugin", "PopUpTime"]));
                        } else
                        {
                            /// WIP
                            var rust = new Oxide.Game.Rust.Libraries.Rust();
                            
                            string args = storedEvent.CommandLine.Remove(0,storedEvent.CommandLine.IndexOf(" ") +1);

                            string command = storedEvent.CommandLine.Substring(0, storedEvent.CommandLine.IndexOf(" "));
                            rust.RunServerCommand(command, args);
                        }
                        storedEvent.broadcast = true;
                        Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                    }

                    if (storedEvent.EventDate.Date <= DateTime.UtcNow.Date && !storedEvent.broadcast)
                    {
                        if (storedEvent.EventDate.Hour <= DateTime.UtcNow.Hour && !storedEvent.broadcast)
                        {
                            if (storedEvent.EventDate.Minute <= DateTime.UtcNow.Minute && !storedEvent.broadcast)
                            {
                                SendNotification(storedEvent.EventInfo, Convert.ToInt16(Config["Plugin", "PopUpTime"]));
                                storedEvent.broadcast = true;
                                Interface.GetMod().DataFileSystem.WriteObject("MyEvents", storedData);
                            }
                        }
                    }
                }
                // Check for Events on that day

            }
        }
        void SendHelp(BasePlayer player)
        {
            SendChatMessage(player, "Timed Notification", "/notification add <DD/MM/YY> <HH:MM> \"<MESSAGE>\" - To schedule a notification for the specified time");
            SendChatMessage(player, "Timed Notification", "/notification addcmd <DD/MM/YY> <HH:MM> \"<COMMAND>\" - To schedule a console command for the specified time");
            SendChatMessage(player, "Timed Notification", "/notification list - List Future Events");
            SendChatMessage(player, "Timed Notification", "/notification reset - Remove Current and Past Events");
        }
        //---------------------------->   Chat Sending   <----------------------------//
        void SendNotification(string message, int delay=5,BasePlayer player=null)
        {
            if (PopUpNotifier && PopupNotifications)
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
