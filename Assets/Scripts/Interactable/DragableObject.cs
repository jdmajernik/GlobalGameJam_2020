using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableObject : InteractableObject
{

    private bool bIsAttachedToMouse = false;

    private Camera mainCamera;
    private Rigidbody rb;

    [SerializeField] private float WaterSplashRadius = 10f;

    void Awake()
    {
        mainCamera = GameObject.FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
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
    }

    void Update()
    {
        if (bIsAttachedToMouse)
        {
            var plane = new Plane(Vector3.forward, transform.position);

            Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(mouseRay, out float rayDistance))
            {
                transform.position = mouseRay.GetPoint(rayDistance);
            }

            if (Input.GetButtonDown("PlayerFix"))
            {
                RaycastHit[] outHits;
                var startPos = new Vector3(transform.position.x - (WaterSplashRadius /2), transform.position.y, transform.position.z);
                outHits = Physics.CapsuleCastAll();
                foreach(var hit in outHits)
                {
                    
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                bIsAttachedToMouse = false;
                rb.useGravity = true;

            }
        }
    }

    //Overriding the base implementation to do nothing
    public override void OnBearInteract() { }
}
