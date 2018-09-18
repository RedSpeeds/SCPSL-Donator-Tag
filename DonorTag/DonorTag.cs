using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.IO;
using System;

namespace DonorTag
{
    [PluginDetails(
        author = "TheCreeperCow",
        name = "DonorTag",
        description = "Gives donors fancy tags",
        id = "com.thecreepercow.donortag",
        version = "4.0.4",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 3)]

    class DonorTagPlugin : Plugin
    {
        internal Tag[] donorTags = new Tag[0];

        public override void OnEnable()
        {
            donorTags = getDonorTags();
            this.Info("Donor Tags successfully loaded.");
		}

        public override void OnDisable()
        {
        }

        public Tag[] getDonorTags()
		{
			Tag[] tags = new Tag[0];
			if (this.GetConfigBool("donor_tags_use_config_mode"))
			{
				string[] donors = this.GetConfigList("donor_tags");
				tags = new Tag[donors.Length];
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
						tags[i] = new Tag("", donorParts[0], donorParts[1], donorParts[2], "");
					}
					else if (donorParts.Length == 4)
					{
						tags[i] = new Tag("", donorParts[0], donorParts[1], donorParts[2], donorParts[3]);
					}
					else
					{
						this.Warn("Invalid donor tag in configuration: " + donor);
						continue;
					}
				}
			}
			else
			{
				if (!File.Exists("DonorTags.csv"))
				{
					//File.Create("DonorTags.csv");
					File.AppendAllText("DonorTags.csv", "player_name,steamid,role_name,color,group" + Environment.NewLine);
					this.Debug("Created DonorTags.csv with header row: player_name,steamid,role_name,color,group");
				}

				using (var reader = new StreamReader("DonorTags.csv"))
				{
					List<String[]> rows = new List<String[]>();
					while (!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						rows.Add(line.Split(','));
					}

					List<Tag> tempList = new List<Tag>();
					for (int i = 0; i < rows.Count; i++)
					{
						if (i == 0)
						{
							this.Debug("Skipping header row: " + string.Join(",", rows[i]));
							continue;
						}

						String[] donorParts = rows[i];
						if (donorParts.Length == 4)
						{
							tempList.Add(new Tag(donorParts[0], donorParts[1], donorParts[2], donorParts[3], ""));
							this.Debug("Adding tag: " + tempList[tempList.Count - 1]);
						}
						else if (donorParts.Length == 5)
						{
							tempList.Add(new Tag(donorParts[0], donorParts[1], donorParts[2], donorParts[3], donorParts[4]));
							this.Debug("Adding tag: " + tempList[tempList.Count - 1]);
						}
						else
						{
							this.Warn("Invalid donor tag in configuration missing : " + string.Join(",", donorParts));
							continue;
						}
					}
					tags = tempList.ToArray();
				}
			}
            return tags;
        }
        
        public override void Register()
        {
            //this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.High);
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new JoinHandler(this), Priority.High);
			this.AddConfig(new Smod2.Config.ConfigSetting("donor_tags_use_config_mode", false, Smod2.Config.SettingType.BOOL, true, "If a donor tags configuration setting exceeds 256, and especially 512 characters it will glitch our your server."));
			this.AddConfig(new Smod2.Config.ConfigSetting("donor_tags", new string[] { }, Smod2.Config.SettingType.LIST, true, "Two-dimensional array of donor tags."));
		}
    }

    struct Tag
    {
        public string playerName, steamID, rankName, color, group;

        public Tag(string playerName, string steamID, string rankName, string color, string group)
        {
			this.playerName = playerName;
			this.steamID = steamID;
            this.rankName = rankName;
            this.color = color;
            this.group = group;
        }

        public override string ToString()
        {
            return playerName + "," + steamID + "," + rankName + "," + color + "," + group;
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
			
            Tag[] tags;
			if (this.plugin.donorTags.Length == 0)
			{
				this.plugin.Info("Donor Tags array is empty. Populate it with tags.");
				tags = this.plugin.getDonorTags();
				this.plugin.donorTags = tags;
			}
			else
			{
				this.plugin.Info("Using cached Donor Tags array for player.");
				tags = this.plugin.donorTags;
			}
            foreach (Tag tag in tags)
            {
				this.plugin.Info("Is this player a donor? " + ev.Player.SteamId + " == " + tag.steamID);
                if (ev.Player.SteamId == tag.steamID)
                {
                    ev.Player.SetRank(tag.color, tag.rankName, tag.group);
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
