using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    ///<summary>Editor-defined Material for the object once it's destroyed</summary>
    [SerializeField]private Material OnDestroyedMaterial;

    [SerializeField] private Material DefaultMaterial;

    private bool bIsDestroyed = false;

    public virtual void OnBearInteract()
    {
        if (!bIsDestroyed)
        {
            this.bIsDestroyed = true;
            this.gameObject.GetComponent<Renderer>().material = OnDestroyedMaterial;
        }
    }

    private void RepairerInteract()
    {
        if (bIsDestroyed)
        {
            this.bIsDestroyed = false;
            this.gameObject.GetComponent<Renderer>().material = DefaultMaterial;
        }
    }

    void OnMouseDown()
    {
        RepairerInteract();
    }
}
