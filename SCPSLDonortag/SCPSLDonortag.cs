using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using System;
using System.Collections.Generic;

namespace SCPSLDonortag
{
    [PluginDetails(
         author = "TheCreeperCow",
         name = "DonorTag",
         description = "Gives donors fancy tags",
        id = "com.thecreepercow.donortag",
        version = "2.1",
        SmodMajor =3,
        SmodMinor =3,
        SmodRevision =0)]

    class SCPSLDonortag : Plugin
    {
        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {

        }

        public String getTag(String SteamID)
        {
            Dictionary<String, String> SteamIDS = ConfigManager.Manager.Config.GetDictValue("Donor_SteamIDs");
            if (!SteamIDS.ContainsKey(SteamID))
            {
                return null;
            }
            String tagID = SteamIDS[SteamID];
            Dictionary<String, String> tags = ConfigManager.Manager.Config.GetDictValue("Donor_Tags");
            String tag = tags[tagID].Split('|')[0];
            return tag;
        }
        public String getColor(String SteamID)
        {
            Dictionary<String, String> SteamIDS = ConfigManager.Manager.Config.GetDictValue("Donor_SteamIDs");
            if (!SteamIDS.ContainsKey(SteamID))
            {
                return null;
            }
            String tagID = SteamIDS[SteamID];
            Dictionary<String, String> tags = ConfigManager.Manager.Config.GetDictValue("Donor_Tags");
            String color = tags[tagID].Split('|')[1].ToLower();
            return color;
        }
        public override void Register()
        {
            String[] defaultvalid = new string[0];
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new JoinHandler(this), Priority.High);
        }
    }
}
