using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;

public class DragableObject : InteractableObject
{

    public bool bIsAttachedToMouse{ get; protected set; }
    protected bool bIsUsingItem = false;

    protected Camera mainCamera;
    protected Rigidbody rb;


    [SerializeField] protected ParticleSystem onUseParticleSystem;
    

    [SerializeField] protected Material highlightMaterial;
    private Material ItemMaterial;

    [Header("Development Options")] 
    
    [SerializeField] protected bool bShowDebug = false;

    [SerializeField] protected float ShowDebugTime = 5.0f;
    void Start()
    {
        mainCamera = GameObject.FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        ItemMaterial = GetComponent<Renderer>().material;
        bIsAttachedToMouse = false;
    }
    // Start is called before the first frame update
    void OnMouseDown()
    {
        
    }

    public void AttachToMouse()
    {
        bIsAttachedToMouse = true;
        rb.useGravity = false;
        StopHighlight();
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

            if (Input.GetButtonUp(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_UseItem]))
            {
                StopUseItem();
            }
        }
    }

    void OnMouseEnter()
    {
        StartHighlight();
    }

    void OnMouseExit()
    {
        StopHighlight();
    }

    protected void StartHighlight()
    {
        GetComponent<Renderer>().material = highlightMaterial;
    }

    protected void StopHighlight()
    {
        GetComponent<Renderer>().material = ItemMaterial;
    }

    protected virtual void UseItem(){}
    protected virtual void StopUseItem(){}

    protected void FollowMouse()
    {
        var plane = new Plane(Vector3.forward, transform.position);

        Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(mouseRay, out float rayDistance))
        {
            transform.position = mouseRay.GetPoint(rayDistance);
        }
    }

    public virtual void DropItem()
    {
        bIsAttachedToMouse = false;
        rb.useGravity = true;
        StartHighlight();
    }

    //Overriding the base implementation to do nothing
    public override void OnBearInteract() { }
}
