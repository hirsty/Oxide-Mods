using Oxide.Core.Plugins;
using System;

namespace Oxide.Plugins
{
    [Info("Emote", "Hirsty", "1.0.2", ResourceId = 1353)]
    [Description("This will allow players to express their feelings!")]
    class Emote : RustPlugin
    {
        public static string version = "1.0.2";
        public string template = "";
        public string EnableEmotes = "false";
        #region Config Data
        protected override void LoadDefaultConfig()
        {

            PrintWarning("Whoops! No config file, lets create a new one!"); // Runs when no configuration file has been found
            Config.Clear();
            Config["Plugin", "Version"] = version;
            Config["Config", "Text"] = "<color=#f0f0f0><i><b>{Player}</b> {Message}</i></color>";
            Config["Config", "EnableEmotes"] = "false";
            Config["Emotes", ":)"] = "smiles";
            Config["Emotes", ":-)"] = "smiles";
            Config["Emotes", ":=)"] = "smiles";
            Config["Emotes", ":("] = "sulks";
            Config["Emotes", ":-("] = "sulks";
            Config["Emotes", ":=("] = "sulks";
            Config["Emotes", ":D"] = "grins";
            Config["Emotes", ":-D"] = "grins";
            Config["Emotes", ":=D"] = "grins";
            Config["Emotes", ":d"] = "grins";
            Config["Emotes", ":-d"] = "grins";
            Config["Emotes", ":=d"] = "grins";
            Config["Emotes", ":|"] = "is speechless";
            Config["Emotes", ":-|"] = "is speechless";
            Config["Emotes", ":=|"] = "is speechless";
            Config["Emotes", ":p"] = "sticks out a tongue";
            Config["Emotes", ":-p"] = "sticks out a tongue";
            Config["Emotes", ":=p"] = "sticks out a tongue";
            Config["Emotes", ":P"] = "sticks out a tongue";
            Config["Emotes", ":-P"] = "sticks out a tongue";
            Config["Emotes", ":=P"] = "sticks out a tongue";
            Config["Emotes", ":$"] = "blushes";
            Config["Emotes", ":-$"] = "blushes";
            Config["Emotes", ":=$"] = "blushes";
            Config["Emotes", "]:)"] = "gives an evil grin";
            Config["Emotes", ">:)"] = "gives an evil grin";
            Config["Emotes", ":*"] = "blows a kiss";
            Config["Emotes", ":-*"] = "blows a kiss";
            Config["Emotes", ":=*"] = "blows a kiss";
            Config["Emotes", ":@"] = "looks angry";
            Config["Emotes", ":-@"] = "looks angry";
            Config["Emotes", ":=@"] = "looks angry";
            Config["Emotes", "x("] = "looks angry";
            Config["Emotes", "x-("] = "looks angry";
            Config["Emotes", "x=("] = "looks angry";
            Config["Emotes", "X("] = "looks angry";
            Config["Emotes", "X-("] = "looks angry";
            Config["Emotes", "X=("] = "looks angry";
            Config["Emotes", "o/"] = "waves";
            Config["Emotes", "\\o"] = "waves back";
            SaveConfig();
        }
        private void Loaded() => LoadConfigData(); // What to do when plugin loaded

        private void LoadConfigData()
        {
            if (Config["Plugin", "Version"].ToString() != version)
            {
                Puts("Uh oh! Not up to date! No Worries, lets update you!");
                switch (version)
                {
                    case "1.0.1":
                        Config["Config", "EnableEmotes"] = "false";
                        Config["Emotes", ":)"] = "smiles";
                        Config["Emotes", ":-)"] = "smiles";
                        Config["Emotes", ":=)"] = "smiles";
                        Config["Emotes", ":("] = "sulks";
                        Config["Emotes", ":-("] = "sulks";
                        Config["Emotes", ":=("] = "sulks";
                        Config["Emotes", ":D"] = "grins";
                        Config["Emotes", ":-D"] = "grins";
                        Config["Emotes", ":=D"] = "grins";
                        Config["Emotes", ":d"] = "grins";
                        Config["Emotes", ":-d"] = "grins";
                        Config["Emotes", ":=d"] = "grins";
                        Config["Emotes", ":|"] = "is speechless";
                        Config["Emotes", ":-|"] = "is speechless";
                        Config["Emotes", ":=|"] = "is speechless";
                        Config["Emotes", ":p"] = "sticks out a tongue";
                        Config["Emotes", ":-p"] = "sticks out a tongue";
                        Config["Emotes", ":=p"] = "sticks out a tongue";
                        Config["Emotes", ":P"] = "sticks out a tongue";
                        Config["Emotes", ":-P"] = "sticks out a tongue";
                        Config["Emotes", ":=P"] = "sticks out a tongue";
                        Config["Emotes", ":$"] = "blushes";
                        Config["Emotes", ":-$"] = "blushes";
                        Config["Emotes", ":=$"] = "blushes";
                        Config["Emotes", "]:)"] = "gives an evil grin";
                        Config["Emotes", ">:)"] = "gives an evil grin";
                        Config["Emotes", ":*"] = "blows a kiss";
                        Config["Emotes", ":-*"] = "blows a kiss";
                        Config["Emotes", ":=*"] = "blows a kiss";
                        Config["Emotes", ":@"] = "looks angry";
                        Config["Emotes", ":-@"] = "looks angry";
                        Config["Emotes", ":=@"] = "looks angry";
                        Config["Emotes", "x("] = "looks angry";
                        Config["Emotes", "x-("] = "looks angry";
                        Config["Emotes", "x=("] = "looks angry";
                        Config["Emotes", "X("] = "looks angry";
                        Config["Emotes", "X-("] = "looks angry";
                        Config["Emotes", "X=("] = "looks angry";
                        Config["Emotes", "o/"] = "waves";
                        Config["Emotes", "\\o"] = "waves back";
                        break;

                }
                Config["Plugin", "Version"] = version;
                Config.Save();
            }
            template = Config["Config", "Text"].ToString();
            EnableEmotes = Config["Config", "EnableEmotes"].ToString();

        }
        #endregion
        #region Hooks
        [HookMethod("CheckForEmotes")]
        public string EmoteCheck(BasePlayer player, string checkmsg)
        {
            if (Config["Emotes", checkmsg] != null && EnableEmotes == "true")
            {
                string emote = Config["Emotes", checkmsg].ToString();

                string build = template;
                build = build.Replace("{Player}", player.displayName);
                build = build.Replace("{Message}", checkmsg);
                return build;
            }
            else
            {
                return checkmsg;
            }
        }
        #endregion
        #region Chat Commands
        [ChatCommand("me")] // Whatever cammand you want the player to type
        private void TheFunction(BasePlayer player, string command, string[] args)
        {
            string full = string.Join(" ", args);
            SendChatMessage(player, full);
        }
        #endregion
        object OnPlayerChat(ConsoleSystem.Arg arg)
        {
            
            //string message = arg.GetString(0, "text");
            string message = arg.GetString(0);
           
            if (Config["Emotes", message] != null && EnableEmotes == "true")
            {
                
                BasePlayer player = (BasePlayer)arg.connection.player;
                SendChatMessage(player, Config["Emotes", message].ToString());
                return false;
               
            }
            return null;
        }

        void SendChatMessage(BasePlayer player, string msg)
        {
            string build = template;
            build = build.Replace("{Player}", player.displayName);
            build = build.Replace("{Message}", msg);
            ConsoleSystem.Broadcast("chat.add", player.userID, build, 1.0);
            PrintToConsole(player, build); 
        }
    }
}
