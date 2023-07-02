using Kitchen;
using KitchenMods;
using PreferenceSystem;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenIRegretIt
{
    public class Main : IModInitializer
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.IRegretIt";
        public const string MOD_NAME = "I Regret It";
        public const string MOD_VERSION = "0.1.1";

        internal const string ENABLED_ID = "enabled";
        internal const string AS_PARCEL_ID = "parcel";
        internal static PreferenceSystemManager PrefManager;

        public Main()
        {

        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Offer Booking Desk")
                .AddOption<bool>(
                    ENABLED_ID,
                    true,
                    new bool[] { false, true },
                    new string[] { "Never", "If Missing" })
                .AddLabel("Spawn As")
                .AddOption<bool>(
                    AS_PARCEL_ID,
                    Preferences.Get<bool>(Pref.ProvideStartingEnvelopesAsParcels),
                    new bool[] { false, true },
                    new string[] { "Letter", "Parcel" })
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        public void PreInject() { }

        public void PostInject() { }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
