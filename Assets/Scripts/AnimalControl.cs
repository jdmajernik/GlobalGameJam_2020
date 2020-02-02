using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimalControl : MonoBehaviour
{
    Animator a;
    Transform sirens;

    bool sirensOn = false;



    private void Awake()
    {
        a = GetComponent<Animator>();
        sirens = this.transform.Find("Siren").transform;
    }

    private void Start()
    {
        TurnLights(false);
    }

    void TurnLights(bool b)
    {
        foreach (Light l in sirens.GetComponentsInChildren<Light>())
        {
            l.enabled = b;
        }
    }

    private void Update()
    {
        if (sirensOn)
        {

            sirens.transform.Rotate(Vector3.up, 250f * Time.deltaTime);
        }
    }

    public void Animate()
    {
        a.SetTrigger("Enter");
    }

    public void BreakWall()
    {
        this.GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(DoAfter(() => { TurnLights(true); sirensOn = true; }, 4f));
    }

    IEnumerator DoAfter(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
