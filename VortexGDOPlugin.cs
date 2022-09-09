using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using PotionCraft.ManagersSystem;
using PotionCraft.ManagersSystem.Potion;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.VortexMapItem;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapObject;
using PotionCraft.ObjectBased.UIElements.FloatingText;
using PotionCraft.Settings;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Bindings;

namespace VortexGDO
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class VortexGDOPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private static ConfigEntry<float> _cfgThreshold;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Log = Logger;

            _cfgThreshold = Config.Bind<float>("General", "Threshold", 1f, "If the potion indicator is touching a vortex, the distance between them must be less than this value to unlock the vortex");
            float value = Mathf.Clamp(_cfgThreshold.Value, 0f, 2.5f);
            if (value !=_cfgThreshold.Value)
            {
                _cfgThreshold.Value = value;
            }
        }

        private void Update()
        {
            var vortex = Managers.RecipeMap.currentVortexMapItem;
            if ((vortex == null) || (!vortex.isVortexLocked))
            {
                // nothing to do if we aren't touching a vortex, or we are but its already unlocked
                return;
            }

            var indicator = Managers.RecipeMap.recipeMapObject.indicatorContainer;
            if (indicator == null)
            {
                // if the indicator does not exist, we can do nothing
                return;
            }

            // get distance between the vortex and the indicator
            var vPos = vortex.transform.localPosition;
            var iPos = indicator.transform.localPosition;
            var mag = (vPos - iPos).magnitude;
            if (mag <= _cfgThreshold.Value)
            {
                // if the distance is less than a set threshold, unlock the vortex
                PrintRecipeMapMessage("Vortex unlocked!", Vector2.zero);
                Log.LogDebug($"Vortex unlocked, magnitude {mag:N4}");
                vortex.SetLockedState(false);
            }
        }

        [Command("VortexGDO-SetThreshold", "Set distance threshold for vortex unlocking", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        public static void Cmd_SetThreshold(float newThresh)
        {
            newThresh = Mathf.Clamp(newThresh, 0f, 2.5f);
            Log.LogDebug($"Old threshold: {_cfgThreshold.Value:N4} | New threshold: {newThresh}");
            _cfgThreshold.Value = newThresh;
        }

        [Command("VortexGDO-UnlockAll", "Unlocks all vortexes on the current map", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        public static void Cmd_UnlockAll()
        {
            var currentMap = Managers.RecipeMap.currentMap;
            if (currentMap == null)
            {
                Log.LogInfo("There is no current map");
                return;
            }

            if ((VortexMapItem.vortexesCollection == null) || (VortexMapItem.vortexesCollection.itemsPerMap == null))
            {
                Log.LogInfo("There are no vortexes we can access");
                return;
            }
            var mapVortexes = VortexMapItem.vortexesCollection.itemsPerMap;

            if (!mapVortexes.ContainsKey(currentMap) || (mapVortexes[currentMap] == null) || (mapVortexes[currentMap].Count == 0))
            {
                Log.LogInfo($"Map '{currentMap.potionBase.GetName()}' contains no vortexes");
                return;
            }

            int total = 0;
            int unlocked = 0;
            foreach (var mapItem in mapVortexes[currentMap])
            {
                if (mapItem is not VortexMapItem vortex)
                {
                    continue;
                }

                if (vortex.isVortexLocked)
                {
                    vortex.SetLockedState(false);
                    unlocked++;
                }
                total++;
            }
            Log.LogInfo($"For map '{currentMap.potionBase.GetName()}', found {total} vortexes, {unlocked} were locked");
        }

        public static void PrintRecipeMapMessage(string message, Vector2 position)
        {
            RecipeMapObject recipeMapObject = Managers.RecipeMap.recipeMapObject;
            var prefab = Settings<PotionManagerSettings>.Asset.collectedFloatingTextPrefab;
            Vector2 msgPos = recipeMapObject.transmitterWindow.ViewRect.center + position;
            CollectedFloatingText.SpawnNewText(prefab, msgPos, new CollectedFloatingText.FloatingTextContent(message, CollectedFloatingText.FloatingTextContent.Type.Text, 0f), Managers.Game.Cam.transform, false, false);
        }
    }
}
