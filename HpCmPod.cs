using System.Linq;
using UnityEngine;
using System;
public class HpCmPod : HPEquippable
{
    WeaponManager wm;
    [SerializeField] public int addedFlares;
    [SerializeField] public int addedChaff;
    [SerializeField] public Transform[] cmsTransform;
    private ChaffLauncher[] _launcher = Array.Empty<ChaffLauncher>();

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

                //adds another element to the array with the value of our countermeasures
                chaff.counts = chaff.counts.Append(addedChaff).ToArray();
            }

        }




        if (addedFlares > 0)
        {
            for (int i = 0; i < cmsTransform.Length; i++)
            {
                FlareCountermeasure flares = cmm.flareCMs[0];

                flares.counts = flares.counts.Append(0).ToArray();


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

       
                ChaffLauncher[] launchers = chaff.GetComponentsInChildren<ChaffLauncher>();

                int idxToRemove = Array.IndexOf(launchers, _launcher[i]);


                if (idxToRemove == -1)
                {
                    Debug.Log("Invalid Index when trying to remove BOL Pod");
                    return;
                }

                chaff.counts = chaff.counts.Where((val, idx) => idx != idxToRemove).ToArray();
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

                //countsValue = (int[])countsField.GetValue(flares);

                flares.ejectTransforms = flares.ejectTransforms.Append(cmsTransform[i]).ToArray();

                int idxToRemove = Array.IndexOf(flares.ejectTransforms, cmsTransform[i]);
                //Debug.Log("idxToRemove" +  idxToRemove);

                flares.ejectTransforms = flares.ejectTransforms.Where((val, idx) => idx != idxToRemove).ToArray();
                flares.counts= flares.counts.Where((val, idx) => idx != idxToRemove).ToArray();
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