using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
using System.Net.Sockets;
public class HpBolChaff : CWB_HPEquipExtension
{
    WeaponManager wm;
    [SerializeField] public int addedCountermeasures;
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


        //This code adds another launcher to the mix
        ChaffCountermeasure chaff = cmm.chaffCMs[0];
        ChaffLauncher existingLauncher = chaff.GetComponentInChildren<ChaffLauncher>();
        //clone the existing launchers so I dont have to add the gameobject in Unity every itme
        
        _launcher = Instantiate(existingLauncher, cmsTransform.position,cmsTransform.rotation, chaff.transform);

        //this is just debugging so i can grab the values in dnSpy
        ChaffLauncher[] allLaunchers = chaff.GetComponentsInChildren<ChaffLauncher>();


        chaff.maxCount += addedCountermeasures;
        chaff.count += addedCountermeasures;


        //gets the value of the internal counts[] array
        Type counterMeasureType = typeof(ChaffCountermeasure);
        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] countsValue = (int[])countsField.GetValue(chaff);

        //adds another element to the array with the value of our countermeasures
        countsField.SetValue(chaff, countsValue.Append(addedCountermeasures).ToArray());

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


        ChaffCountermeasure chaff = cmm.chaffCMs[0];

        Type counterMeasureType = typeof(FlareCountermeasure);
        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] countsValue = (int[])countsField.GetValue(chaff);


        //ChaffLauncher[] launchers = (ChaffLauncher[])launcherField.GetValue(chaff);
        ChaffLauncher[] launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

        //countsValue = (int[])countsField.GetValue(flares);

        //flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();

        int idxToRemove = Array.IndexOf(launchers, _launcher);


       if (idxToRemove == -1)
        {
            Debug.Log("Invalid Index when trying to remove BOL Pod");
            return;
        }
        //Debug.Log("idxToRemove" +  idxToRemove);

        countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
        countsValue = countsValue.Where((val, idx) => idx != idxToRemove).ToArray();
        countsField.SetValue(chaff, countsValue);
        chaff.maxCount -= addedCountermeasures;
        _launcher.transform.parent = null;

        launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

        _changedCMS = false;


        //FieldInfo launcherField = counterMeasureType.GetField("launchers", BindingFlags.NonPublic | BindingFlags.Instance);
        //launcherField.SetValue(chaff, launchers);

        if (conf == null)
        {
            Debug.Log("MoreCountermeasures:No conf");
            return;
        }
    }
}