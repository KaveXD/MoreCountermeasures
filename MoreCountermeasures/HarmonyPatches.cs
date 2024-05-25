using Harmony;
using System;
using System.Reflection;
using System.Xml.Schema;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace MoreCountermeasures
{
    [HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.TryFire))]
    class FlareFirePatch
    {
        static bool Prefix(ref int idx, FlareCountermeasure __instance)
        {
            Debug.Log(idx);
            //WeaponManager wm = VTOLAPI.GetPlayersVehicleGameObject().GetComponent<WeaponManager>();
            //CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();
            //FlareCountermeasure flare = cmm.flareCMs[0];
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

                Debug.Log("Rotation" + __instance.ejectTransforms[idx].forward.ToString());
                Debug.Log("Position" + position.ToString());
                Debug.Log("Velocity" + velocity.ToString());


                idx = (idx+1) % maxIdx;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.count), MethodType.Getter)]
    class getCountPatch
    {
       static bool Prefix(FlareCountermeasure __instance, ref int __result)
        {

            Type counterMeasureType = typeof(FlareCountermeasure);
            FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            int[] countsValue = (int[])countsField.GetValue(__instance);

            Debug.Log("LINE");
            

            foreach (int i in countsValue)
            {
                Debug.Log("CmsCount: " + i);
                __result += i;
            }

            Debug.Log("LINE");

            return false;
        }
    }

}
