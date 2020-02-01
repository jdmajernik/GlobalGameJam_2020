using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableObject : InteractableObject
{

    private bool bIsAttachedToMouse = false;

    private Camera mainCamera;

    private float mouseScale = 45;
    private float constCameraDist = 10;

    void Awake()
    {
        mainCamera = GameObject.FindObjectOfType<Camera>();
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
    }

    void Update()
    {
        if (bIsAttachedToMouse)
        {
            Rect ViewportSize = mainCamera.pixelRect;
            Vector2 CenteredMousePosition = new Vector2(Input.mousePosition.x - (ViewportSize.width/2), Input.mousePosition.y - (ViewportSize.height / 2) );
            Debug.Log(Vector2.Distance(CenteredMousePosition, Vector2.zero));

            float scaledDistance = Vector2.Distance(CenteredMousePosition, Vector2.zero) / mouseScale;
            Vector3 SphericalWorldPosFromCamera = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(scaledDistance);
            Vector3 ReverseSphericalWorldPosFromCamera = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(constCameraDist);
            var NormalizedPosition = (SphericalWorldPosFromCamera + ReverseSphericalWorldPosFromCamera) /2;

            this.gameObject.transform.position = new Vector3(NormalizedPosition.x, NormalizedPosition.y, transform.position.z);
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //transform.LookAt(mainCamera.transform);

            if (Input.GetMouseButtonDown(2))
            {
                bIsAttachedToMouse = false;
            }
        }
    }

    //Overriding the base implementation to do nothing
    public override void OnBearInteract() { }
}
