using Smod2.API;
using Smod2;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCPSLDonortag
{
    class JoinHandler : IEventPlayerJoin
    {
        private SCPSLDonortag plugin;
        public JoinHandler(Plugin plugin)
        {
            this.plugin =(SCPSLDonortag) plugin;
        }

        public void OnPlayerJoin(Player player)
        {
            

                if (player == null || player.SteamId == null)
                {
                    plugin.Error("ID or player is null");
                }
            String tag = plugin.getTag(player.SteamId);
            String color = plugin.getColor(player.SteamId);
            if (tag == null||color==null)
            {
                return;
            }
            player.SetRole(color, tag);
        }
    }
}