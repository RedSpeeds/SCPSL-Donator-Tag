using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;

namespace DonorTag
{
    [PluginDetails(
        author = "TheCreeperCow",
        name = "DonorTag",
        description = "Gives donors fancy tags",
        id = "com.thecreepercow.donortag",
        version = "3.0.1",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 3)]

    class DonorTagPlugin : Plugin
    {
        //internal Tag[] donorTags = new Tag[] { };

        public override void OnEnable()
        {
            //donorTags = getDonorTags();
            this.Info("Donor Tags successfully loaded.");
		}

        public override void OnDisable()
        {
        }

        public Tag[] getDonorTags()
        {
            string[] donors = this.GetConfigList("donor_tags");
            Tag[] tags = new Tag[donors.Length];
            for (int i = 0; i < donors.Length; i++)
            {
                string donor = donors[i];
                string[] donorParts = donor.Split(';');
                if (donorParts.Length < 3)
                {
                    this.Warn("Invalid donor tag in configuration: " + donor);
                    continue;
                }
                else if (donorParts.Length == 3)
                {
                    tags[i] = new Tag(donorParts[0], donorParts[1], donorParts[2], "");
                }
                else if (donorParts.Length == 4)
                {
                    tags[i] = new Tag(donorParts[0], donorParts[1], donorParts[2], donorParts[3]);
                }
                else
                {
                    this.Warn("Invalid donor tag in configuration: " + donor);
                    continue;
                }
            }
            return tags;
        }
        
        public override void Register()
        {
            //this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.High);
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new JoinHandler(this), Priority.High);
			this.AddConfig(new Smod2.Config.ConfigSetting("donor_tags", new string[] { }, Smod2.Config.SettingType.LIST, true, "Two-dimensional array of donor tags."));
		}
    }

    struct Tag
    {
        public string SteamID, name, color, group;

        public Tag(string SteamID, string name, string color, string group)
        {
            this.SteamID = SteamID;
            this.name = name;
            this.color = color;
            this.group = group;
        }

        public override string ToString()
        {
            return "SteamID:" + SteamID + ";Name:" + name + ";Color:" + color + ";Group:" + group;
        }
    }

    class JoinHandler : IEventHandlerPlayerJoin
    {
        private DonorTagPlugin plugin;

        public JoinHandler(Plugin plugin)
        {
            this.plugin = (DonorTagPlugin) plugin;
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (ev.Player == null || ev.Player.SteamId == null)
            {
                plugin.Error("Player is null or the PlayerJoinEvent failed to pass the player's SteamID.");
                return;
            }

            Tag[] tags = this.plugin.getDonorTags();
            foreach (Tag tag in tags)
            {
                if (ev.Player.SteamId == tag.SteamID)
                {
                    ev.Player.SetRank(tag.color, tag.name, tag.group);
                    break;
                }
            }
        }
    }

    /*class RoundStartHandler : IEventHandlerRoundStart
    {
        private DonorTagPlugin plugin;

        public RoundStartHandler(Plugin plugin)
        {
            this.plugin = (DonorTagPlugin)plugin;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
			this.plugin.Info("Refreshing donor tags from configuration...");
            this.plugin.donorTags = this.plugin.getDonorTags();
			string output = "";
			foreach (Tag tag in this.plugin.donorTags)
			{
				if (output.Length == 0)
				{
					output = tag.ToString();
				} else
				{
					output += ',' + tag.ToString();
				}
			}
			this.plugin.Info("Tags loaded into the server: " + output);
        }
    }*/
}
