using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
public class HpBolFlare : CWB_HPEquipExtension
{
    WeaponManager wm;
    [SerializeField] public int addedCountermeasures;
    [SerializeField] public Transform cmsTransform;

    private bool _changedCMS = false;
    public override void OnEquip()
    {
        base.OnEquip();
        Initialize(); //run my custom method after the original methdo
    }
    public override void OnUnequip()
    {
        base.OnUnequip();

    }
    public override void OnConfigAttach(LoadoutConfigurator configurator)
    {
        base.OnConfigAttach(configurator);
        Initialize(configurator);
    }
    public override void OnConfigDetach(LoadoutConfigurator configurator)
    {
        base.OnConfigDetach(configurator);
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



        FlareCountermeasure flares = cmm.flareCMs[0];

        Type counterMeasureType = typeof(FlareCountermeasure);
        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] countsValue = (int[])countsField.GetValue(flares);


        countsField.SetValue(flares, countsValue.Append(addedCountermeasures).ToArray());

        flares.maxCount += addedCountermeasures;

        //countsValue = (int[])countsField.GetValue(flares);

        flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();
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


        FlareCountermeasure flares = cmm.flareCMs[0];

        Type counterMeasureType = typeof(FlareCountermeasure);
        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] countsValue = (int[])countsField.GetValue(flares);

        //countsValue = (int[])countsField.GetValue(flares);

        flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();

        int idxToRemove = Array.IndexOf(flares.ejectTransforms, cmsTransform);
        //Debug.Log("idxToRemove" +  idxToRemove);

        flares.ejectTransforms = flares.ejectTransforms.Where((val,idx) => idx != idxToRemove).ToArray();
        countsField.SetValue(flares, countsValue.Where((val, idx) => idx != idxToRemove).ToArray());
        flares.maxCount -= addedCountermeasures;


        if (conf == null)
        {
            Debug.Log("MoreCountermeasures:No conf");
            return;
        }
    }
}