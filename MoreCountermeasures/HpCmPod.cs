using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
public class HpCmPod : HPEquippable
{
    WeaponManager wm;
    CountermeasureManager cmm;
    [SerializeField] int maxCountermeasures;
    [SerializeField] Transform cmsTransform;

    int defaultMaxFlares;
    int defaultMaxChaff;
    bool hasDefaultValues = true;
    protected override void OnEquip()
    {
        base.OnEquip();
        Initialize();
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

        if (!cmsTransform)
        {
            Debug.Log("MoreCountermeasures:No Valid Transform");
            return;
        }


        cmm.chaffCMs[0].maxCount = maxCountermeasures;
        cmm.chaffCMs[0].count = maxCountermeasures-1;
        cmm.flareCMs[0].maxCount = maxCountermeasures;
        cmm.flareCMs[0].count = maxCountermeasures-1;



        FlareCountermeasure flares = cmm.flareCMs[0];

        Type counterMeasureType = typeof(FlareCountermeasure);

        FieldInfo countsField =  counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);

        int[] countsValue = (int[])countsField.GetValue(flares);


        countsField.SetValue(flares, countsValue.Append(160).ToArray());


        countsValue = (int[])countsField.GetValue(flares);

        flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform).ToArray();


    }
    public void Dequip(LoadoutConfigurator conf = null)
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

        foreach (var cms in cmm.chaffCMs)
        {
            Debug.Log("CMS:" + cms.ToString());
        }
        foreach (var cms in cmm.flareCMs)
        {
            Debug.Log("FLare:" + cms.ToString());
        }

        cmm.chaffCMs[0].count -= maxCountermeasures - 1;
        cmm.chaffCMs[0].maxCount -= maxCountermeasures;
        cmm.flareCMs[0].count -= maxCountermeasures - 1;
        cmm.flareCMs[0].maxCount -= maxCountermeasures;

        if (conf == null)
        {
            Debug.Log("MoreCountermeasures:No conf");
            return;
        }
    }
    //public void Update()
    //{
    //    CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();
    //    FlareCountermeasure flares = cmm.flareCMs[0];
    //    Type counterMeasureType = typeof(FlareCountermeasure);

    //    FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
    //    int[] countsValue = (int[])countsField.GetValue(flares);

    //    Debug.Log("CountsValueLen: " + countsValue.Length);
    //    foreach (var e in countsValue)
    //    {
    //        Debug.Log("CountsValue: " + e.ToString());
    //    }
    //}
}