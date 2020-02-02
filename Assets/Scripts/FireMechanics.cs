using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using Random = UnityEngine.Random;


public enum EFireLevels
{
    Fire_Small = 0,
    Fire_Medium = 1,
    Fire_Large = 2,
}
public class FireMechanics : MonoBehaviour
{
    private EFireLevels FireLevel;

    private float spawnDist = 0.8f;
    private float MinDistToSpawn = 0.6f;

    private GameObject FirePrefab;

    private float TimeToWait = 3.0f;
    private float EndTime;

    private bool bLatch = false;
    private bool bLevelIncreaseCoolDown = false;
    private bool bLevelDecreaseCoolDown = false;
    private bool bIsIncreasingLevel = false;

    private ParticleSystem FirePartSystem;
    private ParticleSystem FirePartChild;

    private List<Collider> Boundries;

    [SerializeField] private float MedFireUpgradeWaitTime = 1.0f;
    [SerializeField] private float LargeFireUpgradeWaitTime = 1.5f;

    private float checkFireUpgradeWait = 0.25f; //The time (in seconds) to wait before checking again if the fire needs to be upgraded


    private float LevelDownCooldownTime = 0.1f;

    void Awake()
    {
        FirePrefab = Resources.Load<GameObject>("FireObject");
        FireLevel = EFireLevels.Fire_Small;

        Boundries = new List<Collider>();
        var BoundaryObjects = GameObject.FindGameObjectsWithTag(GameplayStatics.FIRE_BOUNDARY_TAG)
            .Where(obj => obj.GetComponent<Collider>());

        foreach (var boundary in BoundaryObjects)
        {
            Boundries.Add(boundary.GetComponent<Collider>());
        }

       
        var FireChild = GetComponentsInChildren<ParticleSystem>();

        foreach (var child in FireChild)
        {
            if (child.CompareTag("FireParticleParent")) { FirePartSystem = child; }
            if (child.CompareTag("FireParticleChild")) { FirePartChild = child; ; }
            
        }

        UpdateFireParticles();

        StartCoroutine(TrySpawnFire());
        StartCoroutine(TryUpgradeFire());
    }


    private IEnumerator TrySpawnFire()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeToWait);
            SpawnFire();
        }
    }
    private void SpawnFire()
    {

        Vector3 SpawnPointL = new Vector3(-spawnDist, 0 , 0) + gameObject.transform.position;
        Vector3 SpawnPointR = new Vector3(spawnDist, 0 , 0) + gameObject.transform.position;
        var FireObjects = GameObject.FindObjectsOfType<FireMechanics>();

        foreach (var fire in FireObjects)
        {
            if(Vector3.Distance(fire.gameObject.transform.position, SpawnPointL) < MinDistToSpawn) { SpawnPointL = Vector3.zero;}
            if(Vector3.Distance(fire.gameObject.transform.position, SpawnPointR) < MinDistToSpawn) { SpawnPointR = Vector3.zero;}
        }

        foreach (var boundary in Boundries)
        {
            if(Vector3.Distance( boundary.ClosestPoint(SpawnPointL), SpawnPointL) < boundary.bounds.size.x/2) { SpawnPointL = Vector3.zero; }
            if(Vector3.Distance(boundary.ClosestPoint(SpawnPointR), SpawnPointR) < boundary.bounds.size.x / 2) { SpawnPointR = Vector3.zero; }
        }
        
        if(!SpawnPointL.Equals(Vector3.zero)){Instantiate(FirePrefab, SpawnPointL, Quaternion.identity);}
        if(!SpawnPointR.Equals(Vector3.zero)){Instantiate(FirePrefab, SpawnPointR, Quaternion.identity);}
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameplayStatics.EXTINGUISH_COLLIDER_TAG) && !bLevelDecreaseCoolDown)
        {
            if ((int) FireLevel > 0)
            {
                DecreaseLevel();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    ///<summary>Used for instantiating the Fire's level</summary>
    public void SetLevel(EFireLevels newLevel)
    {
        FireLevel = (int)FireLevel > (int)newLevel? FireLevel : newLevel;

        UpdateFireParticles();
    }

    private void DecreaseLevel()
    {
        FireLevel = (EFireLevels) ((int) FireLevel - 1);
        //FireLevel = 0;
        bLevelDecreaseCoolDown = true;

        UpdateFireParticles();
        StartCoroutine(ResetDecreaseWaitLatch(LevelDownCooldownTime));
    }

    private void IncreaseLevel()
    {
        if(FireLevel!= EFireLevels.Fire_Large) { bIsIncreasingLevel = true; }

        FireLevel = (int)FireLevel + 1 <= (int)EFireLevels.Fire_Large ? (EFireLevels)((int)FireLevel + 1) : EFireLevels.Fire_Large;
        bLevelIncreaseCoolDown = true;

        UpdateFireParticles();

        StartCoroutine(ResetIncreaseWaitLatch(FireLevel == EFireLevels.Fire_Small ? MedFireUpgradeWaitTime : LargeFireUpgradeWaitTime));
    }

    private IEnumerator TryUpgradeFire()
    {
        yield return new WaitForSeconds(MedFireUpgradeWaitTime);
        while (true)
        {
            //I want to try and always upgrade the fire if it's possible, since it might fully upgrade, get extinguished to small and then be allowed to grow again, this is a way to ensure it's always trying to get larger
            if (!bLevelDecreaseCoolDown && FireLevel != EFireLevels.Fire_Large)
            {
                IncreaseLevel();
            }
            yield return new WaitForSeconds(checkFireUpgradeWait);
        }
    }

    private IEnumerator ResetIncreaseWaitLatch(float time)
    {
        yield return new WaitForSeconds(time);
        bLevelIncreaseCoolDown = false;
    }
    private IEnumerator ResetDecreaseWaitLatch(float time)
    {
        yield return new WaitForSeconds(time);
        bLevelDecreaseCoolDown = false;
    }

    private void UpdateFireParticles()
    {
        var MainSettings = FirePartSystem.main;
        var SecondSettings = FirePartChild.main;

        switch (FireLevel)
        {
            case EFireLevels.Fire_Small:
                MainSettings.startLifetime = 0.3f;
                SecondSettings.startLifetime = 0.1f;
                break;
            case EFireLevels.Fire_Medium:
                MainSettings.startLifetime = 0.6f;
                SecondSettings.startLifetime = 0.25f;
                break;
            case EFireLevels.Fire_Large:
                MainSettings.startLifetime = 1.0f;
                SecondSettings.startLifetime = 0.6f;
                break;
        }

        if (bIsIncreasingLevel)
        {
            FirePartSystem.emission.SetBursts(new ParticleSystem.Burst[] {new ParticleSystem.Burst(FirePartSystem.time,new ParticleSystem.MinMaxCurve(20 * (int)FireLevel),1, 100000)});

            bIsIncreasingLevel = false;
        }
    }
}

public class FireGroup
{
    public List<FireMechanics> FireElements;

}
