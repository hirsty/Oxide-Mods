using Oxide.Core;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("Skeleton Plugin","Hirsty",1.0)]
    [Description("Just a skeleton Plugin for my reference")]
    class Skeleton : RustPlugin 
    {
        protected override void LoadDefaultConfig() => PrintWarning("Whoops! No config file, lets create a new one!");

      
    }
}
