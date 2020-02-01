using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FireMechanics : MonoBehaviour
{
    private float spawnDist = 0.8f;
    private float MinDistToSpawn = 0.6f;

    private GameObject FirePrefab;

    private float TimeToWait = 3.0f;
    private float EndTime;

    private bool bLatch = false;

    void Awake()
    {
        FirePrefab = Resources.Load<GameObject>("FireObject");

        EndTime = Time.time + TimeToWait;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameplayStatics.EXTINGUISH_COLLIDER_TAG))
        {
            Destroy(this.gameObject);
        }
    }
}
