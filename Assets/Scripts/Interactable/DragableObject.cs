using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DragableObject : InteractableObject
{

    private bool bIsAttachedToMouse = false;

    private Camera mainCamera;
    private Rigidbody rb;


    [SerializeField] private float WaterSplashRadius = 10f;
    [SerializeField] private float RayCastHeight = 2.0f;
    [SerializeField] private float RayCastRadius = 2.0f;

    [SerializeField] private Material highlightMaterial;
    private Material ItemMaterial;

    void Awake()
    {
        mainCamera = GameObject.FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        ItemMaterial = GetComponent<Renderer>().material;
    }
    // Start is called before the first frame update
    void OnMouseDown()
    {
        if (!bIsAttachedToMouse)
        {
            AttachToMouse();
        }
    }

    private void AttachToMouse()
    {
        bIsAttachedToMouse = true;
        rb.useGravity = false;
        GetComponent<Renderer>().material = ItemMaterial;
    }

    void Update()
    {
        if (bIsAttachedToMouse)
        {
            FollowMouse();

            if (Input.GetButtonDown(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_UseItem]))
            {
               UseItem();
            }

            if (Input.GetButtonDown(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_DropItem]))
            {
                DropItem();
            }
        }
    }

    void OnMouseEnter()
    {
        if (!bIsAttachedToMouse)
        {
            GetComponent<Renderer>().material = highlightMaterial;
        }
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material = ItemMaterial;
    }

    protected virtual void UseItem()
    {
        //BUG - Item uses itself when the player immediately picks it up. A cooldown should probably be added
        Vector3 closestFloorPos = Vector3.zero;

        //Finds the closest floor position to the object
        foreach (var floorPosition in GameplayStatics.FloorPositionLookup)
        {
            if (floorPosition.Value.y >= transform.position.y && floorPosition.Value.y > closestFloorPos.y)
                closestFloorPos = floorPosition.Value;

        }

        RaycastHit[] outHits;
        var startPos = new Vector3(transform.position.x - (WaterSplashRadius / 2), closestFloorPos.y, transform.position.z);
        outHits = Physics.SphereCastAll(startPos, RayCastRadius, Vector3.right, WaterSplashRadius);
        foreach (var hit in outHits)
        {
            if (hit.collider.gameObject.GetComponent<FireMechanics>())
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    protected void FollowMouse()
    {
        var plane = new Plane(Vector3.forward, transform.position);

        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(mouseRay, out float rayDistance))
        {
            transform.position = mouseRay.GetPoint(rayDistance);
        }
    }

    protected virtual void DropItem()
    {
        bIsAttachedToMouse = false;
        rb.useGravity = true;
        GetComponent<Renderer>().material = highlightMaterial;
    }

    //Overriding the base implementation to do nothing
    public override void OnBearInteract() { }
}
