using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairerMechanics : MonoBehaviour
{
    protected DragableObject HeldItem;
    protected Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (Input.GetButtonDown(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_UseItem]))
        {
            if (HeldItem == null)
            {
                Ray mouseToWorldRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                int layerMask = LayerMask.GetMask(GameplayStatics.REPAIRER_INTERACTIBLE_OBJECTS_LAYER_NAME);

                var InteractableItemsUnderMouse = Physics.RaycastAll(mouseToWorldRay, 200f,
                    layerMask);
                foreach (var InteractableObject in InteractableItemsUnderMouse)
                {
                    Debug.Log(InteractableObject.collider.gameObject.name);
                    HeldItem = InteractableObject.collider.gameObject.GetComponent<DragableObject>();
                    if (HeldItem != null)
                    {
                        HeldItem.AttachToMouse();
                        break;
                    }
                }
            }

        }
        else if (Input.GetButtonDown(GameplayStatics.RepairerInputLookup[RepairerInput.Repairer_DropItem]) && HeldItem != null)
        {
            HeldItem?.DropItem();
            HeldItem = null;
        }
    }

}
