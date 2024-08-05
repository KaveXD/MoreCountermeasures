using HarmonyLib;
using System;
using UnityEngine;
using static ChaffCountermeasure;

namespace MoreCountermeasures
{
    [HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.TryFire))]
    class FlareFirePatch
    {
        static bool Prefix(ref int idx, FlareCountermeasure __instance)
        {
            // All of this code is so that the mode "Auto Double" fires the Countermeasures with every index, not just the default 0 and 1, because vtol for some reason only expects 2 flare countermeasures
            // Debug.Log(idx);
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());
            //Debug.Log("TypeF: " + __instance.GetType());

            //Debug.Log("TFcount: " + __instance.ejectTransforms.Length);
            int maxIdx = __instance.ejectTransforms.Length;
            bool flag = false;
            if (__instance.ConsumeCM(idx))
            {
                flag = true;
            }
            else
            {
                idx = (idx + 1) % maxIdx;
                if (__instance.ConsumeCM(idx))
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


                idx = (idx + 1) % maxIdx;
            }
            return false; //dont run original method, true would run the original method
        }
    }



    [HarmonyPatch(typeof(Countermeasure), nameof(Countermeasure.count), MethodType.Getter)]
    class getFlareCountPatch
    {
       static bool Prefix(FlareCountermeasure __instance, ref int __result) //__result is what is returned by harmony
        {        

            foreach (int i in __instance.counts) //this returns the sum off all countermeasures, not just the default left and right
            {
                __result += i; 
            }


            return false; //dont run original method, true would run the original method
        }
    }

    //[HarmonyPatch(typeof(FlareCountermeasure), nameof(FlareCountermeasure.SetCount))]
    //class setFLareCountPatch
    //{
    //    static bool Prefix(ref int c, FlareCountermeasure __instance)
    //    {
    //        Debug.Log("AAAA");
    //        Debug.Log("BBBB");
    //        Debug.Log("TESTFLARE");
    //        Debug.Log(__instance.GetType());
    //        int targetCount = c;
    //        if (__instance.GetType() == typeof(ChaffCountermeasure))
    //        {
    //            return true;
    //        }

    //        if(__instance.ejectTransforms.Length == 0 )
    //        {
    //            return false;
    //        }


    //        Type counterMeasureType = typeof(FlareCountermeasure);
    //        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
    //        int[] countsValue = (int[])countsField.GetValue(__instance);

    //        //this is so we dont get any funny values...

    //        int maxIndividual = (int)Math.Floor((decimal)__instance.maxCount / countsValue.Length);



    //        int leftToFill = targetCount - __instance.count;
    //        if (targetCount > __instance.maxCount)
    //        {
    //            targetCount = __instance.maxCount;
    //        }



    //        Debug.Log("count: " + __instance.count);
    //        Debug.Log("maxindividual: " + maxIndividual);
    //        Debug.Log("leftToFill: " + leftToFill);
    //        Debug.Log("MaxCount: " + __instance.maxCount);
    //        Debug.Log("Target: " + targetCount);

    //        while (leftToFill > 0)
    //        {
    //            Debug.Log("Loop");
    //            Debug.Log("LeftToFill: " + leftToFill);
    //            foreach (int num in countsValue)
    //            {
    //                Debug.Log("currentContent: " + num);
    //            }

    //            for (int i = 0; i < countsValue.Length; i++)
    //            {
    //                if (leftToFill <= 0)
    //                {
    //                    break;
    //                }
    //                if (countsValue[i] > maxIndividual)
    //                {
    //                    continue;
    //                }
    //                countsValue[i]++;
    //                leftToFill = targetCount - __instance.count;
    //            }
    //            if (leftToFill <= 0)
    //            {
    //                break;
    //            }
    //        }
    //        while (leftToFill < 0)
    //        {
    //            Debug.Log("Loop");
    //            Debug.Log("LeftToFill: " + leftToFill);
    //            foreach (int num in countsValue)
    //            {
    //                Debug.Log("currentContent: " + num);
    //            }


    //            for (int i = 0; i < countsValue.Length; i++)
    //            {
    //                if(leftToFill >= 0)
    //                {
    //                    break;
    //                }
    //                if (countsValue[i] <= 0)
    //                {
    //                    continue;
    //                }
    //                countsValue[i]--;
    //                leftToFill = targetCount - __instance.count;
    //            }
    //            if (leftToFill >= 0)
    //            {
    //                break;
    //            }
    //        }
    //        countsField.SetValue(__instance, countsValue);

    //        __instance.UpdateCountText();

    //        return false;
    //    }
    //}








    #region chaff


    [HarmonyPatch(typeof(ChaffCountermeasure), nameof(ChaffCountermeasure.TryFire))]
    class ChaffFirePatch
    {
        static bool Prefix(ref int idx, ChaffCountermeasure __instance)
        {
            // All of this code is so that the mode "Auto Double" fires the Countermeasures with every index, not just the default 0 and 1, because vtol for some reason only expects 2 flare countermeasures (it makes perfect sense)
            // Debug.Log(idx);

            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());
            //Debug.Log("TypeC: " + __instance.GetType());



            int maxIdx = __instance.launchers.Length;



            bool flag = false;
            if (__instance.ConsumeCM(idx))
            {
                flag = true;
            }
            else
            {
                idx = (idx + 1) % maxIdx;
                if (__instance.ConsumeCM(idx))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                __instance.launchers[idx].FireChaff(); ;
                __instance.InternalChaff();



                idx = (idx + 1) % maxIdx;
            }
            return false; //dont run original method, true would run the original method
        }
    }




    [HarmonyPatch(typeof(Countermeasure), nameof(Countermeasure.SetCount))]
    class setChaffCountPatch
    {
        static bool Prefix(ref int c, Countermeasure __instance)
        {
            int targetCount = c;
            ChaffLauncher[] launchers = __instance.GetComponentsInChildren<ChaffLauncher>();
            //Debug.Log("AAAA");
            //Debug.Log("BBBB");
            //Debug.Log("TESTCHAFF");
            //Debug.Log(__instance.GetType());


            if (__instance.GetType() == typeof(ChaffCountermeasure))
            {
                if (launchers.Length == 0)
                {
                    return false;
                }

                int maxIndividual = (int)Math.Floor((decimal)__instance.maxCount / __instance.counts.Length);
                int leftToFill = targetCount - __instance.count;
                if (targetCount > __instance.maxCount)
                {
                    targetCount = __instance.maxCount;
                }

                //Debug.Log("count: " + __instance.count);
                //Debug.Log("maxindividual: " + maxIndividual);
                //Debug.Log("leftToFill: " + leftToFill);
                //Debug.Log("MaxCount: " + __instance.maxCount);
                //Debug.Log("Target: " + targetCount);
                //Debug.Log("Type: " + __instance.GetType());
                while (leftToFill > 0)
                {
                    //Debug.Log("Loop");
                    //Debug.Log("LeftToFill: " + leftToFill);
                    //foreach (int num in __instance.counts)
                    //{
                    //    Debug.Log("currentContent: " + num);
                    //}

                    for (int i = 0; i < __instance.counts.Length; i++)
                    {
                        if (leftToFill <= 0)
                        {
                            break;
                        }
                        if (__instance.counts[i] > maxIndividual)
                        {
                            continue;
                        }
                        __instance.counts[i]++;
                        leftToFill = targetCount - __instance.count;
                    }
                    if (leftToFill <= 0)
                    {
                        break;
                    }
                }
                while (leftToFill < 0)
                {
                    for (int i = 0; i < __instance.counts.Length; i++)
                    {
                        if (leftToFill >= 0)
                        {
                            return false;
                        }
                        if (__instance.counts[i] <= 0)
                        {
                            continue;
                        }
                        __instance.counts[i]--;
                        leftToFill++;
                    }
                }

                __instance.UpdateCountText();
                return false;
            }
            else 
            {
                FlareCountermeasure __fakeInstance = __instance as FlareCountermeasure;
     
                if (__fakeInstance.ejectTransforms.Length == 0)
                {
                    return false;
                }



                //this is so we dont get any funny values...

                int maxIndividual = (int)Math.Floor((decimal)__instance.maxCount / __instance.counts.Length);



                int leftToFill = targetCount - __instance.count;
                if (targetCount > __instance.maxCount)
                {
                    targetCount = __instance.maxCount;
                }



                //Debug.Log("count: " + __instance.count);
                //Debug.Log("maxindividual: " + maxIndividual);
                //Debug.Log("leftToFill: " + leftToFill);
                //Debug.Log("MaxCount: " + __instance.maxCount);
                //Debug.Log("Target: " + targetCount);
                //Debug.Log("Type: " + __instance.GetType());


                while (leftToFill > 0)
                {
                    //Debug.Log("Loop");
                    //Debug.Log("LeftToFill: " + leftToFill);
                    //foreach (int num in __instance.counts)
                    //{
                    //    Debug.Log("currentContent: " + num);
                    //}

                    for (int i = 0; i < __instance.counts.Length; i++)
                    {
                        if (leftToFill <= 0)
                        {
                            break;
                        }
                        if (__instance.counts[i] > maxIndividual)
                        {
                            continue;
                        }
                        __instance.counts[i]++;
                        leftToFill = targetCount - __instance.count;
                    }
                    if (leftToFill <= 0)
                    {
                        break;
                    }
                }
                while (leftToFill < 0)
                {
                    //Debug.Log("Loop");
                    //Debug.Log("LeftToFill: " + leftToFill);
                    //foreach (int num in __instance.counts)
                    //{
                    //    Debug.Log("currentContent: " + num);
                    //}


                    for (int i = 0; i < __instance.counts.Length; i++)
                    {
                        if (leftToFill >= 0)
                        {
                            break;
                        }
                        if (__instance.counts[i] <= 0)
                        {
                            continue;
                        }
                        __instance.counts[i]--;
                        leftToFill = targetCount - __instance.count;
                    }
                    if (leftToFill >= 0)
                    {
                        break;
                    }
                }

                __instance.UpdateCountText();

                return false;
            }
        }
    }




    //[HarmonyPatch(typeof(ChaffCountermeasure), nameof(ChaffCountermeasure.count), MethodType.Getter)]
    //class getChaffCountPatch
    //{
    //    static bool Prefix(ChaffCountermeasure __instance, ref int __result) //__result is what is returned by harmony
    //    {


    //        foreach (int i in __instance.counts) //this returns the sum off all countermeasures, not just the default left and right
    //        {
    //            __result += i;
    //        }


    //        return false; //dont run original method, true would run the original method
    //    }
    //}














    #endregion


}
