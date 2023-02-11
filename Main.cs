using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using KitchenMods;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenIRegretIt
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.IRegretIt";
        public const string MOD_NAME = "I Regret It";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.3";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif
        internal const string ENABLED_ID = "enabled";
        internal const string AS_PARCEL_ID = "parcel";

        //public static AssetBundle Bundle;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            RegisterPreferences();
            SetupKLMenu();
        }

        private void RegisterPreferences()
        {
            PreferenceUtils.Register<KitchenLib.BoolPreference>(MOD_GUID, ENABLED_ID, "Enabled");
            PreferenceUtils.Get<KitchenLib.BoolPreference>(MOD_GUID, ENABLED_ID).Value = true;
            PreferenceUtils.Register<KitchenLib.BoolPreference>(MOD_GUID, AS_PARCEL_ID, "Enabled");
            PreferenceUtils.Get<KitchenLib.BoolPreference>(MOD_GUID, AS_PARCEL_ID).Value = Preferences.Get<bool>(Pref.ProvideStartingEnvelopesAsParcels);

            PreferenceUtils.Load();
        }

        private void SetupKLMenu()
        {
            //Setting Up For Pause Menu
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(IRegretItMenu<PauseMenuAction>), new IRegretItMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(IRegretItMenu<PauseMenuAction>), typeof(PauseMenuAction));
        }

        private void AddGameData()
        {
            // LogInfo("Attempting to register game data...");

            // AddGameDataObject<MyCustomGDO>();

            // LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            // LogInfo("Attempting to load asset bundle...");
            // Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            // LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            // AddGameData();

            // Perform actions when game data is built
            //Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            //{
            //};
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }

    internal class IRegretItMenu<T> : KLMenu<T>
    {
        Option<bool> EnabledOption;
        Option<bool> AsParcelOption;

        private static void OnPreferenceChanged(string key, bool value)
        {
            PreferenceUtils.Get<KitchenLib.BoolPreference>(Main.MOD_GUID, key).Value = value;
            PreferenceUtils.Save();
        }

        public IRegretItMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Offer Booking Desk");
            EnabledOption = new Option<bool>(
                new List<bool> { false, true },
                PreferenceUtils.Get<KitchenLib.BoolPreference>(Main.MOD_GUID, Main.ENABLED_ID).Value,
                new List<string> { "Never", "If Missing" });
            Add<bool>(EnabledOption).OnChanged += delegate (object _, bool b)
            {
                OnPreferenceChanged(Main.ENABLED_ID, b);
            };

            AddLabel("Spawn As");
            AsParcelOption = new Option<bool>(
                new List<bool> { false, true },
                PreferenceUtils.Get<KitchenLib.BoolPreference>(Main.MOD_GUID, Main.AS_PARCEL_ID).Value,
                new List<string> { "Letter", "Parcel" });
            Add<bool>(AsParcelOption).OnChanged += delegate (object _, bool b)
            {
                OnPreferenceChanged(Main.AS_PARCEL_ID, b);
            };

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }
    }
}
