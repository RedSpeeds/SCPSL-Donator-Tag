using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.IO;
using System;
using Smod2.API;

namespace DonorTag
{
    [PluginDetails(
        author = "TheCreeperCow",
        name = "DonorTag",
        description = "Gives donors fancy tags",
        id = "com.thecreepercow.donortag",
        version = "4.1.8",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 17)]

    class DonorTagPlugin : Plugin
    {
        internal Dictionary<string, Tag> donorTags = new Dictionary<string, Tag>();

        public override void OnEnable()
        {
            this.donorTags = getDonorTags();
			this.Info("Loading tags into the server...");
			foreach (Tag tag in this.donorTags.Values)
			{
				this.Info(tag.ToString());
			}
			this.Info("Donor Tags successfully loaded.");
		}

        public override void OnDisable()
        {
        }

        public Dictionary<string, Tag> getDonorTags()
		{
			Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
			if (this.GetConfigBool("donor_tags_use_config_mode"))
			{
				string[] donors = this.GetConfigList("donor_tags");
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
						tags[donorParts[0]] = new Tag("", donorParts[0], donorParts[1], donorParts[2], "");
					}
					else if (donorParts.Length == 4)
					{
						tags[donorParts[0]] = new Tag("", donorParts[0], donorParts[1], donorParts[2], donorParts[3]);
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
							tags[donorParts[1]] = new Tag(donorParts[0], donorParts[1], donorParts[2], donorParts[3], "");
							this.Debug("Adding tag: " + tags[donorParts[1]]);
						}
						else if (donorParts.Length == 5)
						{
							tags[donorParts[1]] = new Tag(donorParts[0], donorParts[1], donorParts[2], donorParts[3], donorParts[4]);
							this.Debug("Adding tag: " + tags[donorParts[1]]);
						}
						else
						{
							this.Warn("Invalid donor tag in configuration missing : " + string.Join(",", donorParts));
							continue;
						}
					}
				}
			}
            return tags;
        }
        
        public override void Register()
        {
            this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.High);
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
			
			if (this.plugin.donorTags.Count == 0)
			{
				this.plugin.Debug("Donor Tags array is empty. Populate it with tags.");
				this.plugin.donorTags = this.plugin.getDonorTags();
			}
			else
			{
				this.plugin.Debug("Using cached Donor Tags array for player.");
			}

			if (this.plugin.donorTags.ContainsKey(ev.Player.SteamId))
			{
				Tag tag = this.plugin.donorTags[ev.Player.SteamId];
				ev.Player.SetRank(tag.color, tag.rankName, tag.group);
				this.plugin.Debug("Set tag for player: " + tag);
			}
        }
    }

    class RoundStartHandler : IEventHandlerRoundStart
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
			foreach (Player player in ev.Server.GetPlayers())
			{
				if (this.plugin.donorTags.ContainsKey(player.SteamId))
				{
					Tag tag = this.plugin.donorTags[player.SteamId];
					player.SetRank(tag.color, tag.rankName, tag.group);
					this.plugin.Debug("Set tag for player: " + tag);
				}
			}
        }
    }
}
