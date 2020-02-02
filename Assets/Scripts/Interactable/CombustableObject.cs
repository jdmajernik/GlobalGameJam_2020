using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombustableObject : InteractableObject
{
    private GameObject FireObject;
    private GameObject SpawnedFire;

    [SerializeField] private float FireSpawnZOffset = 2.0f;

    void Awake()
    {
        FireObject = Resources.Load<GameObject>("FireObject");
    }
    public override void OnBearInteract()
    {
        base.OnBearInteract();
        SpawnFire();
    }

    protected virtual void SpawnFire()
    {
        if(SpawnedFire == null) SpawnedFire = Instantiate(FireObject, this.transform.position + new Vector3(0, 0,0 - FireSpawnZOffset), Quaternion.identity);
        SpawnedFire.GetComponent<FireMechanics>().SetLevel(EFireLevels.Fire_Large);
    }
}
