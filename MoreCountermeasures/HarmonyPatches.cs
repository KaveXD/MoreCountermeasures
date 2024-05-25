using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace MoreCountermeasures
{
    [HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.TryFire))]
    class FlareFirePatch
    {
        static bool Prefix(ref int idx, FlareCountermeasure __instance)
        {
            // All of this code is so that the mode "Auto Double" fires the Countermeasures with every index, not just the default 0 and 1, because vtol for some reason only expects 2 flare countermeasures
            // Debug.Log(idx);


            int maxIdx = __instance.ejectTransforms.Length;
            bool flag = false;
            if (__instance.ConsumeCM(idx))
            {
                flag = true;
            }
            else
            {
                idx = (idx + 1) % maxIdx;
                if(__instance.ConsumeCM(idx))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Vector3 position = __instance.ejectTransforms[idx].position;



                Vector3 velocity = __instance.rb.velocity + UnityEngine.Random.Range(__instance.ejectSpeed * 0.9f, __instance.ejectSpeed * 1.1f) * VectorUtils.WeightedDirectionDeviation(__instance.ejectTransforms[idx].forward, 3f);
                __instance.FireFlare(position, velocity, __instance.flareLife);

                //Debug.Log("Rotation" + __instance.ejectTransforms[idx].forward.ToString());
                //Debug.Log("Position" + position.ToString());
                //Debug.Log("Velocity" + velocity.ToString());


                idx = (idx+1) % maxIdx;
            }
            return false; //dont run original method, true would run the original method
        }
    }



    [HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.count), MethodType.Getter)]
    class getCountPatch
    {
       static bool Prefix(FlareCountermeasure __instance, ref int __result) //__result is what is returned by harmony
        {

            //Reflection, probably very slow but what can I do
            Type counterMeasureType = typeof(FlareCountermeasure);
            FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            int[] countsValue = (int[])countsField.GetValue(__instance);

            

            foreach (int i in countsValue) //this returns the sum off all countermeasures, not just the default left and right
            {
                __result += i; 
            }


            return false; //dont run original method, true would run the original method
        }
    }

}
