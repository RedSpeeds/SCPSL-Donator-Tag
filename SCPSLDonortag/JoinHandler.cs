using Smod2.API;
using Smod2;
using Smod2.Events;
using Smod2.EventHandlers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCPSLDonortag
{
    class JoinHandler : IEventHandlerPlayerJoin
    {
        private SCPSLDonortag plugin;
        public JoinHandler(Plugin plugin)
        {
            this.plugin =(SCPSLDonortag) plugin;
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (ev.Player == null || ev.Player.SteamId == null)
            {
                plugin.Error("ID or player is null");
            }
            String tag = plugin.getTag(ev.Player.SteamId);
            String color = plugin.getColor(ev.Player.SteamId);
            if (tag == null||color==null)
            {
                return;
            }
            ev.Player.SetRank(color, tag);
        }
    }
}