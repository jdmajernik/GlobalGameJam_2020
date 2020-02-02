using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool bLevelChangeCoolDown = false;

    private ParticleSystem FirePartSystem;

    [SerializeField] private float MedFireUpgradeWaitTime = 1000.0f;
    [SerializeField] private float LargeFireUpgradeWaitTime = 1500f;

    private float checkFireUpgradeWait = 0.25f; //The time (in seconds) to wait before checking again if the fire needs to be upgraded


    private float LevelDownCooldownTime = 500;

    void Awake()
    {
        FirePrefab = Resources.Load<GameObject>("FireObject");
        FireLevel = EFireLevels.Fire_Small;
        EndTime = Time.time + TimeToWait;

        FirePartSystem = GetComponentInChildren<ParticleSystem>();

        UpdateFireParticles();

        StartCoroutine(TryUpgradeFire());
    }

    void Update()
    {
        if (Time.time > EndTime && !bLatch)
        {
            bLatch = true;
            SpawnFire();
        }
    }

    private async void SpawnFire()
    {
        float spawnDir = Mathf.RoundToInt(Random.Range(0f, 100f) / 100) == 0 ? -1 : 1;
        Debug.Log(spawnDir);

        Vector3 SpawnPoint = new Vector3(spawnDir * spawnDist, 0 , 0) + gameObject.transform.position;
        var FireObjects = GameObject.FindObjectsOfType<FireMechanics>();

        foreach (var fire in FireObjects)
        {
            if(Vector3.Distance(fire.gameObject.transform.position, SpawnPoint) < MinDistToSpawn)
            {
                return;
            }
        }

        Instantiate(FirePrefab, SpawnPoint, Quaternion.identity);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameplayStatics.EXTINGUISH_COLLIDER_TAG) && !bLevelChangeCoolDown)
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
        bLevelChangeCoolDown = true;

        UpdateFireParticles();

        StartCoroutine(ResetWaitLatch(LevelDownCooldownTime));
    }

    private void IncreaseLevel()
    {
        FireLevel = (int)FireLevel + 1 <= (int)EFireLevels.Fire_Large ? (EFireLevels)((int)FireLevel + 1) : EFireLevels.Fire_Large;
        bLevelChangeCoolDown = true;

        Debug.Log("Setting the fire's new level");

        UpdateFireParticles();

        StartCoroutine(ResetWaitLatch(FireLevel == EFireLevels.Fire_Small ? MedFireUpgradeWaitTime : LargeFireUpgradeWaitTime));
    }

    private IEnumerator TryUpgradeFire()
    {
        yield return new WaitForSeconds(MedFireUpgradeWaitTime);
        while (true)
        {
            //I want to try and always upgrade the fire if it's possible, since it might fully upgrade, get extinguished to small and then be allowed to grow again, this is a way to ensure it's always trying to get larger
            if (!bLevelChangeCoolDown && FireLevel != EFireLevels.Fire_Large)
            {
                Debug.Log("Upgrading the fire");
                IncreaseLevel();
            }
            yield return new WaitForSeconds(checkFireUpgradeWait);
        }
    }

    private IEnumerator ResetWaitLatch(float time)
    {
        yield return new WaitForSeconds(time);
        bLevelChangeCoolDown = false;
    }

    private void UpdateFireParticles()
    {
        var MainSettings = FirePartSystem.main;
        switch (FireLevel)
        {
            case EFireLevels.Fire_Small:
                MainSettings.startLifetime = 0.3f;
                break;
            case EFireLevels.Fire_Medium:
                MainSettings.startLifetime = 0.6f;
                break;
            case EFireLevels.Fire_Large:
                MainSettings.startLifetime = 1.0f;
                break;
        }

        
    }
}
