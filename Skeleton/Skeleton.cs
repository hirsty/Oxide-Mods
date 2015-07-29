using UnityEngine;
using Rust;
using Oxide.Core.Plugins;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Data;
using Oxide.Core;



namespace Oxide.Plugins
{
    [Info("Skeleton Plugin", "Hirsty", 1.0)]
    [Description("Just a skeleton Plugin for my reference")]
    class Skeleton : RustPlugin
    {
        protected override void LoadDefaultConfig() => PrintWarning("Whoops! No config file, lets create a new one!"); // Runs when no configuration file has been found
        private void Loaded() => LoadConfigData(); // What to do when plugin loaded

        private void LoadConfigData()
        {
            // Load Imformation from a Config file
        }

        [ChatCommand("skeleton")] // Whatever cammand you want the player to type
        private void TheFunction(BasePlayer player, string command, string[] args)
        {
            // Function for the chat command
        }

        void OnServerInitialized()
        {
            // Called when server is booted up
        }

        void OnTick()
        {
            // Do something  on server "Tick" - defined by tickrate
        }

        void OnPluginUnloaded()
        {
            // Called when plugin is unloaded - clean up here
        }
    }
}
