using Harmony;
using System.Reflection;
using UnityEngine;

namespace MoreCountermeasures
{
    public class Main : VTOLMOD
    {
        public static Main instance;

        public override void ModLoaded()
        {
            var harmonyInstance = HarmonyInstance.Create("kave.morecountermeasures");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            base.ModLoaded();
            Debug.Log("MoreCountermeasuresLoaded");

            instance = this;
        }
    }
}