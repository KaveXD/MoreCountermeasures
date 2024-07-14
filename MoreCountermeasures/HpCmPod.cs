using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
using System.Net.Sockets;
public class HpCmPod : HPEquippable
{
    WeaponManager wm;
    [SerializeField] public int addedFlares;
    [SerializeField] public int addedChaff;
    [SerializeField] public Transform[] cmsTransform;
    private ChaffLauncher[] _launcher = Array.Empty<ChaffLauncher>();

    private bool _changedCMS = false;
    protected override void OnEquip()
    {
        base.OnEquip();
        Initialize(); //run my custom method after the original methdo
    }
    public override void OnUnequip()
    {
        base.OnUnequip();

        Dequip();
    }
    public override void OnConfigAttach(LoadoutConfigurator configurator)
    {
        base.OnConfigAttach(configurator);
        Initialize(configurator);
    }
    public override void OnConfigDetach(LoadoutConfigurator configurator)
    {
        base.OnConfigDetach(configurator);
        Dequip(configurator);
    }
    public void Initialize(LoadoutConfigurator conf = null)
    {
        wm = conf ? conf.wm : weaponManager;


        if (!wm)
        {
            Debug.Log("MoreCountermeasures:No WeaponManager");
            return;
        }

        CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();

        if (!cmm)
        {
            Debug.Log("MoreCountermeasures:No CountermeasureManager");
            return;
        }

        if (cmsTransform.Length == 0)
        {
            Debug.Log("MoreCountermeasures:No Valid Transform");
            return;
        }

        if (addedChaff > 0)
        {
            for (int i = 0; i < cmsTransform.Length; i++)
            {
                //This code adds another launcher to the mix
                ChaffCountermeasure chaff = cmm.chaffCMs[0];
                ChaffLauncher existingLauncher = chaff.GetComponentInChildren<ChaffLauncher>();
                //clone the existing launchers so I dont have to add the gameobject in Unity every itme

                ChaffLauncher tempLauncher = Instantiate(existingLauncher, cmsTransform[i].position, cmsTransform[i].rotation, chaff.transform);

                _launcher = _launcher.Append(tempLauncher).ToArray();

                //this is just debugging so i can grab the values in dnSpy
                ChaffLauncher[] allLaunchers = chaff.GetComponentsInChildren<ChaffLauncher>();


                chaff.maxCount += addedChaff;
                //chaff.count += addedChaff;


                //gets the value of the internal counts[] array
                Type counterMeasureType = typeof(ChaffCountermeasure);
                FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
                int[] countsValue = (int[])countsField.GetValue(chaff);

                //adds another element to the array with the value of our countermeasures
                countsField.SetValue(chaff, countsValue.Append(addedChaff).ToArray());
            }

        }




        if (addedFlares > 0)
        {
            for (int i = 0; i < cmsTransform.Length; i++)
            {
                FlareCountermeasure flares = cmm.flareCMs[0];

                Type cmsFlareType = typeof(FlareCountermeasure);
                FieldInfo flareField = cmsFlareType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
                int[] flareValue = (int[])flareField.GetValue(flares);


                flareField.SetValue(flares, flareValue.Append(0).ToArray());

                flares.maxCount += addedFlares;

                //countsValue = (int[])countsField.GetValue(flares);

                flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform[i]).ToArray();
            }

        }
        _changedCMS = true;

    }
    public void Dequip(LoadoutConfigurator conf = null)
    {
        wm = conf ? conf.wm : weaponManager;

        if (_changedCMS == false)
        {
            Debug.Log("MoreCountermeasures:No changes to CMS count");
        }

        if (!wm)
        {
            Debug.Log("MoreCountermeasures:No WeaponManager");
            return;
        }

        CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();

        if (!cmm)
        {
            Debug.Log("MoreCountermeasures:No CountermeasureManager");
            return;
        }


        if (addedChaff > 0)
        {
            for (int i = 0; i < cmsTransform.Length; i++)
            {
                ChaffCountermeasure chaff = cmm.chaffCMs[0];

                Type counterMeasureType = typeof(FlareCountermeasure);
                FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
                int[] countsValue = (int[])countsField.GetValue(chaff);

                ChaffLauncher[] launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

                int idxToRemove = Array.IndexOf(launchers, _launcher[i]);


                if (idxToRemove == -1)
                {
                    Debug.Log("Invalid Index when trying to remove BOL Pod");
                    return;
                }

                countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
                countsValue = countsValue.Where((val, idx) => idx != idxToRemove).ToArray();
                countsField.SetValue(chaff, countsValue);
                chaff.maxCount -= addedChaff;
                _launcher[i].transform.parent = null;

                launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

            }


        }

        if (addedFlares > 0)
        {
            for (int i = 0; i < cmsTransform.Length; i++)
            {
                FlareCountermeasure flares = cmm.flareCMs[0];

                Type counterMeasureType = typeof(FlareCountermeasure);
                FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
                int[] countsValue = (int[])countsField.GetValue(flares);

                //countsValue = (int[])countsField.GetValue(flares);

                flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform[i]).ToArray();

                int idxToRemove = Array.IndexOf(flares.ejectTransforms, cmsTransform[i]);
                //Debug.Log("idxToRemove" +  idxToRemove);

                flares.ejectTransforms = flares.ejectTransforms.Where((val, idx) => idx != idxToRemove).ToArray();
                countsField.SetValue(flares, countsValue.Where((val, idx) => idx != idxToRemove).ToArray());
                flares.maxCount -= addedFlares;
            }

        }


        _changedCMS = false;

        if (conf == null)
        {
            Debug.Log("MoreCountermeasures:No conf");
            return;
        }
    }
}