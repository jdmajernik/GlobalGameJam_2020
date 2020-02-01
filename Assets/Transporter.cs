using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour
{
    public enum TransporterInput { VerticalAxisUp, VerticalAxisDown }

    [SerializeField] Transform endpoint;
    [SerializeField] TransporterInput inputDirection;
    [HideInInspector] public Transform Endpoint => endpoint;

    Transform bear = null;



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<BearController>())
        {
            bear = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.GetComponent<BearController>())
        {
            bear = null;
        }
    }

    private void Update()
    {
        if (bear == null)
        {
            return;
        }

        switch (inputDirection)
        {
            case TransporterInput.VerticalAxisUp:
                if (Input.GetAxisRaw("Vertical") > 0f)
                {
                    DoTransport(bear);
                }
                break;
            case TransporterInput.VerticalAxisDown:
                if (Input.GetAxisRaw("Vertical") < 0f)
                {
                    DoTransport(bear);
                }
                break;
        }
    }

    void DoTransport(Transform target)
    {
        target.GetComponent<BearController>().Transport(endpoint.position);
    }
}
