using BepInEx;
using HarmonyLib;

namespace AnimatedCybergrindTextures
{
    [BepInProcess("ULTRAKILL.exe")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class AnimatedCybergrindTextures : BaseUnityPlugin
    {
        private static Harmony _harmony;
        
        private void Awake()
        {
            AssetsManager.Instance.LoadAssets();
            AssetsManager.Instance.RegisterAssets();
            _harmony = new Harmony(PluginInfo.GUID);
            _harmony.PatchAll();
        }
    }
}