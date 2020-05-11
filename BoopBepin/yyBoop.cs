using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using HarmonyLib;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoopBepin
{
	[BepInPlugin(nameof(BoopBepin), nameof(BoopBepin), VERSION)]
	public class BoopBepin : BaseUnityPlugin
	{
		public const string VERSION = "1.1.0";
		
		public static Camera cam;
		private static GameObject boop;
		private static BreastCollider bc;
		
		private static bool isStudio;

		private static ConfigEntry<bool> runInStudio { get; set; }
		private static ConfigEntry<bool> runInGame { get; set; }
		public static ConfigEntry<KeyboardShortcut> Toggle { get; private set; }
		
		private void Awake()
		{
			runInStudio = Config.Bind("General", "Run in studio", false);
			runInGame = Config.Bind("General", "Run in game", true);
			Toggle = Config.Bind("General", "Toggle", new KeyboardShortcut(KeyCode.M));
			
			if (Application.productName.Contains("PlayHomeStudio"))
				isStudio = true;
			
			runInStudio.SettingChanged += delegate
			{
				if (!isStudio)
					return;

				if (runInStudio.Value)
					CreateBoop();
				else
					DestroyBoop();
			};
			
			runInGame.SettingChanged += delegate
			{
				if (isStudio)
					return;

				if (runInGame.Value)
					CreateBoop();
				else
					DestroyBoop();
			};

			HarmonyWrapper.PatchAll(typeof(BoopBepin));
		}

		private static void DestroyBoop()
		{
			if (boop != null)
			{
				Destroy(boop);
				boop = null;
			}

			if (bc != null)
			{
				Destroy(bc);
				bc = null;
			}
		}
		
		private static void CreateBoop()
		{
			DestroyBoop();

			if (isStudio && !runInStudio.Value || !isStudio && !runInGame.Value)
				return;
			
			var scene = SceneManager.GetActiveScene();
			
			if (!isStudio && scene.name != "EditScene" && scene.name != "ADVScene" && scene.name != "SelectScene" && scene.name != "H")
				return;

			if (isStudio && scene.name != "Studio")
				return;
			
			cam = Camera.main;
			
			boop = new GameObject("BoopBepin");
			bc = boop.AddComponent<BreastCollider>();
		}
		
		private void OnEnable() => SceneManager.sceneLoaded += LevelChange;
		private void OnDisable() => SceneManager.sceneLoaded -= LevelChange;
		private static void LevelChange(UnityEngine.SceneManagement.Scene s, LoadSceneMode m) => CreateBoop();
		
		[HarmonyPostfix, HarmonyPatch(typeof(Female), "Apply")]
		public static void Female_Apply_Patch(Female __instance)
		{
			if (bc == null)
				return;

			bc.AddHuman(__instance);
		}
		
		[HarmonyPostfix, HarmonyPatch(typeof(Male), "Apply")]
		public static void Male_Apply_Patch(Male __instance)
		{
			if (bc == null)
				return;

			bc.AddHuman(__instance);
		}
	}
}
