using StardewModdingAPI.Utilities;

namespace Toggle_Monsters
{
    public class ModConfig
    {
        public KeybindList toggleKey { get; set; } = KeybindList.Parse("OemTilde");
        public bool defaultValue = false;
    }
}
