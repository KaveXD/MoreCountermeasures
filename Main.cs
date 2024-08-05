global using static MoreCountermeasures.Logger;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using System.Reflection;
using HarmonyLib;

namespace MoreCountermeasures
{
    [ItemId("kave.morecountermeasures")] // Harmony ID for your mod, make sure this is unique
    public class Main : VtolMod
    {
        public string ModFolder;

        private void Awake()
        {
            ModFolder = Assembly.GetExecutingAssembly().Location;
            Log($"Awake at {ModFolder}");
            
        }


        public override void UnLoad()
        {
            // Destroy any objects
        }
    }
}