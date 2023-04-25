using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace Toggle_Monsters;

public class ModEntry : Mod
{
	public bool spawnMonsters = true;

	public ModConfig config;

	public override void Entry(IModHelper helper)
	{
		config = helper.ReadConfig<ModConfig>();
		helper.get_Events().get_World().add_NpcListChanged((EventHandler<NpcListChangedEventArgs>)World_NpcListChanged);
		helper.get_Events().get_World().add_LocationListChanged((EventHandler<LocationListChangedEventArgs>)World_LocationListChanged);
		helper.get_Events().get_Input().add_ButtonPressed((EventHandler<ButtonPressedEventArgs>)Input_ButtonPressed);
	}

	private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		if (config.toggleKey.JustPressed())
		{
			spawnMonsters = !spawnMonsters;
			Game1.addHUDMessage(new HUDMessage("Set Monster spawns to " + spawnMonsters));
		}
	}

	private void World_LocationListChanged(object? sender, LocationListChangedEventArgs e)
	{
		if (!Context.get_IsWorldReady())
		{
			return;
		}
		foreach (GameLocation location in e.get_Added())
		{
			removeMonsters(location, ((IEnumerable<NPC>)location.characters).ToArray());
		}
	}

	private void World_NpcListChanged(object? sender, NpcListChangedEventArgs e)
	{
		removeMonsters(e.get_Location(), e.get_Added());
	}

	private void removeMonsters(GameLocation location, IEnumerable<NPC> npcs)
	{
		if (spawnMonsters)
		{
			return;
		}
		foreach (Monster monster in npcs.OfType<Monster>())
		{
			location.characters.Remove((NPC)(object)monster);
		}
	}
}
