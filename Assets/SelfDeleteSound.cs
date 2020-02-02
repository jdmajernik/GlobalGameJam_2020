using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDeleteSound : MonoBehaviour
{
    [SerializeField] float timer;



    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timer);
        Destroy(this.gameObject);
    }
}
