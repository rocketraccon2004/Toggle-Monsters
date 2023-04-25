using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggle_Monsters
{
    public class ModEntry : Mod
    {
        private bool spawnMonsters = true;
        public ModConfig config;
        private bool hasSetDefaultValue = false;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += onGameLaunch;
            helper.Events.World.NpcListChanged += onNPCListChanged;
            helper.Events.World.LocationListChanged += onLocationListChanged;
            helper.Events.Input.ButtonPressed += onKeyPress;
            helper.Events.GameLoop.UpdateTicked += onUpdateTick;
        }

        private void onUpdateTick(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && !hasSetDefaultValue)
            {
                spawnMonsters = config.defaultValue;
                hasSetDefaultValue = true;
            }
        }

        private void onGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null) 
            {
                return;
            }

            configMenu.Register(
                ModManifest,
                () => config = new ModConfig(),
                () => Helper.WriteConfig(config)
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.defaultValue,
                value => config.defaultValue = value,
                () => "Default Value",
                () => "The Default Value"
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.toggleKey,
                value => config.toggleKey = value,
                () => "Toggle Keybind",
                () => "Keybind to Toggle Monsters"
            );
        }

        private void onKeyPress(object sender, ButtonPressedEventArgs e)
        {
            if (config.toggleKey.JustPressed())
            {
                spawnMonsters = !spawnMonsters;
                Game1.addHUDMessage(new HUDMessage("Set Monster spawns to " + spawnMonsters));
            }
        }

        private void onLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            foreach (GameLocation location in e.Added)
            {
                removeMonsters(location, location.characters.ToArray());
            }
        }

        private void onNPCListChanged(object sender, NpcListChangedEventArgs e)
        {
            removeMonsters(e.Location, e.Added);
        }

        private void removeMonsters(GameLocation location, IEnumerable<NPC> npcs)
        {
            if (spawnMonsters)
            {
                return;
            }
            foreach (Monster m in npcs.OfType<Monster>())
            {
                location.characters.Remove(m);
            }
        }
    }
}
