//using System.Linq;
//using UnityEngine;
//using System.Reflection;
//using System;
//public class HpBolChaff : CWB_HPEquipExtension
//{
//    WeaponManager wm;
//    [SerializeField] public int addedCountermeasures;

//    private bool _changedCMS = false;
//    private int _addedIndex;
//    public override void OnEquip()
//    {
//        base.OnEquip();
//        Initialize();
//    }
//    public override void OnUnequip()
//    {
//        base.OnUnequip();
//    }
//    public override void OnConfigAttach(LoadoutConfigurator configurator)
//    {
//        base.OnConfigAttach(configurator);
//        Initialize(configurator);
//    }
//    public override void OnConfigDetach(LoadoutConfigurator configurator)
//    {
//        base.OnConfigDetach(configurator);
//    }
//    public void Initialize(LoadoutConfigurator conf = null)
//    {
//        wm = conf ? conf.wm : hpEquip.weaponManager;


//        if (!wm)
//        {
//            Debug.Log("MoreCountermeasures:No WeaponManager");
//            return;
//        }

//        CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();

//        if (!cmm)
//        {
//            Debug.Log("MoreCountermeasures:No CountermeasureManager");
//            return;
//        }



//        ChaffCountermeasure chaffInstance = cmm.chaffCMs[0];

//        Type counterMeasureType = typeof(ChaffCountermeasure);
//        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
//        int[] countsValue = (int[])countsField.GetValue(chaffInstance);

//        _addedIndex = countsValue.Length; // this is so we can later remove the right index. It doesnt matter right now, as the counts[idx] are gonna all gonna be equal, (because vtolvr logic), but if i eventually override everything, this will be important

//        countsField.SetValue(chaffInstance, countsValue.Append(addedCountermeasures).ToArray());
    


//        chaffInstance.maxCount += addedCountermeasures;

//        //countsValue = (int[])countsField.GetValue(flares);


//        _changedCMS = true;

        

//    }
//    public void Dequip(LoadoutConfigurator conf = null)
//    {
//        wm = conf ? conf.wm : hpEquip.weaponManager;

//        if(_changedCMS == false)
//        {
//            Debug.Log("MoreCountermeasures:No changes to CMS count");
//        }

//        if (!wm)
//        {
//            Debug.Log("MoreCountermeasures:No WeaponManager");
//            return;
//        }

//        CountermeasureManager cmm = wm.GetComponentInChildren<CountermeasureManager>();

//        if (!cmm)
//        {
//            Debug.Log("MoreCountermeasures:No CountermeasureManager");
//            return;
//        }


//        ChaffCountermeasure chaffInstance = cmm.chaffCMs[0];

//        Type counterMeasureType = typeof(FlareCountermeasure);
//        FieldInfo countsField = counterMeasureType.GetField("counts", BindingFlags.NonPublic | BindingFlags.Instance);
//        int[] countsValue = (int[])countsField.GetValue(chaffInstance);


//        countsField.SetValue(chaffInstance, countsValue.Where((val, idx) => idx != _addedIndex).ToArray());
//        chaffInstance.maxCount -= addedCountermeasures;


//        if (conf == null)
//        {
//            Debug.Log("MoreCountermeasures:No conf");
//            return;
//        }
//    }
//}