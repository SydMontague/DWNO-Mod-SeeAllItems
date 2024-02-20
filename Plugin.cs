using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.ComponentModel;

namespace SeeAllItems;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Digimon World Next Order.exe")]
public class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; }

    public ConfigEntry<float> itemRenderDistance { get; private set; }
    public ConfigEntry<float> materialRenderDistance { get; private set; }
    public ConfigEntry<float> itemRadarDistance { get; private set; }

    public override void Load()
    {
        Instance = this;

        itemRenderDistance = Config.Bind("General", "ItemRenderDistance", 200.0f, "Distance at which items become visible. Vanilla is 20.");
        materialRenderDistance = Config.Bind("General", "MaterialRenderDistance", 200.0f, "Distance at which material spots become visible. Vanilla is 20.");
        itemRadarDistance = Config.Bind("General", "ItemRadarDistance", 20.0f, "Distance at which your Digimon will notify you about items nearby. Vanilla is 20.");

        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Patches));
    }
}
