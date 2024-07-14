﻿using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
using System.Net.Sockets;
public class HpBolCms : CWB_HPEquipExtension
{
    WeaponManager wm;
    [SerializeField] public int addedFlares;
    [SerializeField] public int addedChaff;
    [SerializeField] public Transform cmsTransform;
    private ChaffLauncher _launcher;

    private bool _changedCMS = false;
    public override void OnEquip()
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
        wm = conf ? conf.wm : hpEquip.weaponManager;


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

        if (!cmsTransform)
        {
            Debug.Log("MoreCountermeasures:No Valid Transform");
            return;
        }

        if(addedChaff > 0)
        {
            //This code adds another launcher to the mix
            ChaffCountermeasure chaff = cmm.chaffCMs[0];
            ChaffLauncher existingLauncher = chaff.GetComponentInChildren<ChaffLauncher>();
            //clone the existing launchers so I dont have to add the gameobject in Unity every itme

            _launcher = Instantiate(existingLauncher, cmsTransform.position, cmsTransform.rotation, chaff.transform);

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




        if(addedFlares > 0)
        {
            FlareCountermeasure flares = cmm.flareCMs[0];

            Type cmsFlareType = typeof(FlareCountermeasure);
            FieldInfo flareField = cmsFlareType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            int[] flareValue = (int[])flareField.GetValue(flares);


            flareField.SetValue(flares, flareValue.Append(0).ToArray());

            flares.maxCount += addedFlares;

            //countsValue = (int[])countsField.GetValue(flares);

            flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();
        }


















        _changedCMS = true;

    }
    public void Dequip(LoadoutConfigurator conf = null)
    {
        wm = conf ? conf.wm : hpEquip.weaponManager;

        if(_changedCMS == false)
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


        if(addedChaff > 0)
        {
            ChaffCountermeasure chaff = cmm.chaffCMs[0];

            Type counterMeasureType = typeof(FlareCountermeasure);
            FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            int[] countsValue = (int[])countsField.GetValue(chaff);

            ChaffLauncher[] launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

            int idxToRemove = Array.IndexOf(launchers, _launcher);


            if (idxToRemove == -1)
            {
                Debug.Log("Invalid Index when trying to remove BOL Pod");
                return;
            }

            countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            countsValue = countsValue.Where((val, idx) => idx != idxToRemove).ToArray();
            countsField.SetValue(chaff, countsValue);
            chaff.maxCount -= addedChaff;
            _launcher.transform.parent = null;

            launchers = chaff.GetComponentsInChildren<ChaffLauncher>();
        }

        if(addedFlares > 0)
        {
            FlareCountermeasure flares = cmm.flareCMs[0];

            Type counterMeasureType = typeof(FlareCountermeasure);
            FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
            int[] countsValue = (int[])countsField.GetValue(flares);

            //countsValue = (int[])countsField.GetValue(flares);

            flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();

            int idxToRemove = Array.IndexOf(flares.ejectTransforms, cmsTransform);
            //Debug.Log("idxToRemove" +  idxToRemove);

            flares.ejectTransforms = flares.ejectTransforms.Where((val, idx) => idx != idxToRemove).ToArray();
            countsField.SetValue(flares, countsValue.Where((val, idx) => idx != idxToRemove).ToArray());
            flares.maxCount -= addedFlares;
        }




        _changedCMS = false;







        if (conf == null)
        {
            Debug.Log("MoreCountermeasures:No conf");
            return;
        }
    }
}