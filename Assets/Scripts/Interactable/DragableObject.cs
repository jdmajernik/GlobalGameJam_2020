using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
//using UnityEngine.XR.WSA.Persistence; // This threw build errors and intellisense says it was not in use - sorry in advance if it broke something

public class DragableObject : MonoBehaviour
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
    protected void Init()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        
        if (GetComponent<Renderer>())
        {
            ItemMaterial = GetComponent<Renderer>().material;
        }
        else if(GetComponentInChildren<Renderer>())
        {
            ItemMaterial = GetComponentInChildren<Renderer>().material;
        }
        bIsAttachedToMouse = false;
    }
    // Start is called before the first frame update
    void OnMouseDown()
    {
        
    }

    public void AttachToMouse()
    {
        this.bIsAttachedToMouse = true;
        this.rb.useGravity = false;
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
        if (!bIsAttachedToMouse)
        {
            StartHighlight();
        }
    }

    void OnMouseExit()
    {
        StopHighlight();
    }

    protected void StartHighlight()
    {
        if (GetComponent<Renderer>())
        {
            GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.033f);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_OutlineWidth", 0.033f);
        }
    }

    protected void StopHighlight()
    {
        if (GetComponent<Renderer>())
        {
            GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.0f);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_OutlineWidth", 0.0f);
        }
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

}
