using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketMechanics : DragableObject
{
    [Header("Bucket Mechanics")]

    [SerializeField] private float WaterSplashRadius = 10f;
    [SerializeField] private float RayCastHeight = 2.0f;
    [SerializeField] private float RayCastRadius = 2.0f;

    [SerializeField] private float BucketFillLevel = 0;

    // Start is called before the first frame update
    protected override void UseItem()
    {
        //BUG - Item uses itself when the player immediately picks it up. A cooldown should probably be added
        float closestFloorPos = -1.0f;

        //Finds the closest floor position to the object
        foreach (var floorPosition in GameplayStatics.FloorYPositionLookup)
        {
            if (floorPosition.Value <= transform.position.y && floorPosition.Value > closestFloorPos)
                closestFloorPos = floorPosition.Value;

        }

        RaycastHit[] outHits;
        var startPos = new Vector3(transform.position.x - (WaterSplashRadius / 2), closestFloorPos, transform.position.z);
        outHits = Physics.SphereCastAll(startPos, RayCastRadius, Vector3.right, WaterSplashRadius);
        foreach (var hit in outHits)
        {
            if (hit.collider.gameObject.GetComponent<FireMechanics>())
            {
                if (bShowDebug) { Debug.DrawLine(this.transform.position, hit.point, Color.red, ShowDebugTime);}
                Destroy(hit.collider.gameObject);
            }
        }

        if (bShowDebug)
        {
            Debug.DrawLine(startPos, new Vector3(startPos.x + WaterSplashRadius, startPos.y, startPos.z), Color.blue, ShowDebugTime);
        }
    }
}
