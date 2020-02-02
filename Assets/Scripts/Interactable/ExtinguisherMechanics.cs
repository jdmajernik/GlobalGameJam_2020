using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtinguisherMechanics : DragableObject
{
    private float Charge;
    private const float MAX_CHARGE = 100f;

    private const float COROUTINE_WAIT = 0.1f;

    private bool bIsUsingExtinguisher = false;

    private Image DepletionBar;

    [SerializeField] private float depletionRate = -5.0f;
    [SerializeField] private float RechargeRate = 5.0f;

    void Awake()
    {
        Charge = MAX_CHARGE;
        foreach (var image in GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<Image>())
        {
            if (image.CompareTag("Extinguisher_LoadingBar"))
            {
                DepletionBar = image;
                break;
            }
        }
    }
    // Start is called before the first frame update
    protected override void UseItem()
    {
        if (!bIsUsingExtinguisher)
        {
            StopAllCoroutines();
            StartCoroutine("UseExtinguisher");
        }
    }

    IEnumerator UseExtinguisher()
    {
        bIsUsingExtinguisher = true;

        while (Input.GetButton(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_UseItem]))
        {
            UpdateChargeAmount(depletionRate);
            yield return new WaitForSeconds(COROUTINE_WAIT);
        }

        bIsUsingExtinguisher = false;
        StartCoroutine("RechargeExtinguisher");
        yield return null;
    }

    IEnumerator RechargeExtinguisher()
    {
        while (Charge < MAX_CHARGE)
        {
            UpdateChargeAmount(RechargeRate);
            yield return  new WaitForSeconds(COROUTINE_WAIT);
        }
    }

    private void UpdateChargeAmount(float _amount)
    {
        Charge = Mathf.Clamp(Charge + _amount, 0, MAX_CHARGE);
        DepletionBar.fillAmount = Charge / MAX_CHARGE;
    }
}
