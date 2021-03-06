﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BucketMechanics : DragableObject
{
    [Header("Bucket Mechanics")]

    [SerializeField] private float MaxWaterSplashRadius = 10f;
    [SerializeField] private float RayCastHeight = 2.0f;
    [SerializeField] private float RayCastRadius = 2.0f;

    [SerializeField] private float BucketFillLevel = 0;
    [SerializeField] private float MaxFillLevel = 100;

    private bool FillLatch = false;
    private float FillInterval = 0.1f;
    private float FillRate = 2;
    private float CurrentWaterSplashRadius = 0;
    private float FillPercentage = 0;
    private Image FillBar;

    private GameObject SplashEffect;

    void Awake()
    {
        base.Init();
        SplashEffect = Resources.Load<GameObject>("WaterSplash");
        foreach (var image in GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<Image>())
        {
            if (image.CompareTag(GameplayStatics.LOADING_BAR_TAG))
            {
                FillBar = image;
                break;
            }
        }
    }

    // Start is called before the first frame update
    protected override void UseItem()
    {
        //BUG - Item uses itself when the player immediately picks it up. A cooldown should probably be added
        float closestFloorPos = -1.0f;
        if (FillPercentage < 0.01)
        {
            return;
        }
        

        //Finds the closest floor position to the object
        foreach (var floorPosition in GameplayStatics.FloorYPositionLookup)
        {
            if (floorPosition.Value <= transform.position.y && floorPosition.Value > closestFloorPos)
                closestFloorPos = floorPosition.Value;

        }

        RaycastHit[] outHits;
        var startPos = new Vector3(transform.position.x - (CurrentWaterSplashRadius / 2), closestFloorPos + 0.5f, transform.position.z);
        var endPos = new Vector3(startPos.x + CurrentWaterSplashRadius, startPos.y, startPos.z);
        outHits = Physics.SphereCastAll(startPos, RayCastRadius, Vector3.right, CurrentWaterSplashRadius);
        foreach (var hit in outHits)
        {
            if (hit.collider.gameObject.GetComponent<FireMechanics>())
            {
                if (bShowDebug)
                {
                    Debug.DrawLine(this.transform.position, hit.point, Color.red, ShowDebugTime);
                }

                Destroy(hit.collider.gameObject);
            }
        }

        if (bShowDebug)
        {
            Debug.DrawLine(startPos, new Vector3(startPos.x + CurrentWaterSplashRadius, startPos.y, startPos.z), Color.blue,
                ShowDebugTime);
        }

        for (int i = 0; i < Vector3.Distance(startPos, endPos); i++)
        {
            var spawnPos = new Vector3(startPos.x + i, startPos.y, startPos.z); 
            Instantiate(SplashEffect, spawnPos, Quaternion.identity);
        }

        BucketFillLevel = 0;
        FillPercentage = 0;
        CurrentWaterSplashRadius = 0;
        FillBar.fillAmount = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameplayStatics.WATER_TAG) && FillLatch == false)
        {
            StartCoroutine("FillTimer");
        }
    }

    protected IEnumerator FillTimer()
    {
        this.FillLatch = true;
        yield return new WaitForSeconds(FillInterval);
        BucketFillLevel = BucketFillLevel + FillRate;
        if (BucketFillLevel >= MaxFillLevel)
        {
            BucketFillLevel = MaxFillLevel;
        }

        FillPercentage = BucketFillLevel / MaxFillLevel;
        CurrentWaterSplashRadius = MaxWaterSplashRadius * FillPercentage;
        FillBar.fillAmount = FillPercentage;
        this.FillLatch = false;
    }

}
