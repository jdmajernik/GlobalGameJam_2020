using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    ///<summary>Editor-defined Material for the object once it's destroyed</summary>
    [SerializeField]private Material OnDestroyedMaterial;

    [SerializeField] private Material DefaultMaterial;

    private bool AsyncLatch = false;

    private bool MouseOverObject = false;

    private float TimerIncrement = 0.1f;

    private int RepairIncrement = 10;

    private int Durability = 100;

    private int Damage = 0;

    private bool bIsDestroyed = false;

    public virtual void OnBearInteract()
    {
        if (!bIsDestroyed)
        {
            this.Damage = this.Durability;  //Damage can be changed later
            if (this.Damage >= this.Durability)  
            {
                this.Damage = this.Durability;
                this.bIsDestroyed = true;
                this.gameObject.GetComponent<Renderer>().material = OnDestroyedMaterial;
            }
        }
    }

    private void RepairerInteract()
    {
        if (bIsDestroyed)
        {
            this.Damage = this.Damage - this.RepairIncrement;
            if(this.Damage <= 0)
            {
                this.Damage = 0;
                this.bIsDestroyed = false;
                this.gameObject.GetComponent<Renderer>().material = DefaultMaterial;
            }
        }
    }

    void OnMouseOver()
    {
        MouseOverObject = true;
        if (Input.GetMouseButtonDown(0) && !AsyncLatch && bIsDestroyed == true)
        {
            StartCoroutine("OnTimerExecute");
        }
    }
    void OnMouseExit()
    {
        MouseOverObject = false;
    }


    protected IEnumerator OnTimerExecute()
    {
        if (this.AsyncLatch)
        {
            yield break;
        } //Check for prexisting asynch thread
    
        while (Input.GetMouseButton(0))
        {
            this.AsyncLatch = true;
            //this.RepairLatch = true;

            yield return new WaitForSeconds(TimerIncrement);

            if (Input.GetMouseButton(0) && this.MouseOverObject)
            {
                RepairerInteract();
            }
            this.AsyncLatch = false;
        }
    }


}   
