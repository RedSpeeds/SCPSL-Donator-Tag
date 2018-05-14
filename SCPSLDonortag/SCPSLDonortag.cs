using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using System;
using System.Collections.Generic;

namespace SCPSLDonortag
{
    [PluginDetails(
         author = "TheCreeperCow",
         name = "DonorTag",
         description = "Gives donors fancy tags",
        id = "com.thecreepercow.donortag",
        version = "0.1",
        SmodMajor =2,
        SmodMinor =0,
        SmodRevision =13)]

    class SCPSLDonortag : Plugin
    {
        private List<String> donorList;
        private String color;
        private String tag;
        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
            donorList = new List<string>(GetConfigList("Donors_SteamID"));
            color = GetConfigString("Donor_Color");
            tag = GetConfigString("Donor_Tag");
        }
        public String getColor()
        {
            return color;
        }
        public List<String> getSteamIDs()
        {
            return donorList;
        }
        public String getTag()
        {
            return tag;
        }
        public override void Register()
        {
            this.AddEventHandler(typeof(IEventPlayerJoin), new JoinHandler(this), Priority.High);
            this.AddConfig(new Smod2.Config.ConfigSetting("Donors_SteamID", "", Smod2.Config.SettingType.LIST,true,"SteamIDS of your donors"));
            this.AddConfig(new Smod2.Config.ConfigSetting("Donor_Tag", "Donator", Smod2.Config.SettingType.STRING, true, "The Donor tag text"));
            this.AddConfig(new Smod2.Config.ConfigSetting("Donor_Color", "green", Smod2.Config.SettingType.STRING, true, "The Donor tag color"));
        }
    }
}
