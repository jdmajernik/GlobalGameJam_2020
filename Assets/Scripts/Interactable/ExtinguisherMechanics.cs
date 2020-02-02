using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Collider ExtinguisherCollider;
    ParticleSystem effect;

    void Awake()
    {
        base.Init();
        Charge = MAX_CHARGE;
        foreach (var image in GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<Image>())
        {
            if (image.CompareTag(GameplayStatics.LOADING_BAR_TAG))
            {
                DepletionBar = image;
                break;
            }
        }

        var ExtinguishColliders = gameObject.GetComponentsInChildren<Collider>()
            .Where(obj => obj.CompareTag(GameplayStatics.EXTINGUISH_COLLIDER_TAG));

        foreach (var collider in ExtinguishColliders)
        {
            ExtinguisherCollider = collider;
            break;
        }

        ExtinguisherCollider.enabled = false;

        effect = this.GetComponentInChildren<ParticleSystem>();
        effect.Play();
        effect.enableEmission = false;
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
        effect.enableEmission = true;

        while (Input.GetButton(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_UseItem]) && Charge > 0)
        {
            ExtinguisherCollider.enabled = true;
            UpdateChargeAmount(depletionRate);
            yield return new WaitForSeconds(COROUTINE_WAIT);
        }
        ExtinguisherCollider.enabled = false;

        bIsUsingExtinguisher = false;
        effect.enableEmission = false;
        StartCoroutine("RechargeExtinguisher");
        yield return null;
    }

    IEnumerator RechargeExtinguisher()
    {
        ExtinguisherCollider.enabled = false;
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
