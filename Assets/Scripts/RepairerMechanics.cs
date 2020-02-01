using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairerMechanics : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Pressing Fire key");
        }
    }

    async void OnInputPressed()
    {

    }
}
