﻿using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObject : MonoBehaviour
{
    ///<summary>Editor-defined Material for the object once it's destroyed</summary>
    [SerializeField]private Material OnDestroyedMaterial;

    [SerializeField] private Material DefaultMaterial;

    [SerializeField] private Canvas ObjCanvas;

    private bool AsyncLatch = false;

    private bool CombustionLatch = false;

    private bool MouseOverObject = false;

    private float TimerIncrement = 0.1f;

    private float CombustionIncrement = 0.1f;

    private int CombustionCounter = 0;

    private int MinimumCombustionThreshold = 100;

    private int CombustionThreshold = 0;

    private float RepairIncrement = 10;

    private float Durability = 100;

    private float Damage = 0;

    private bool bIsDestroyed = false;

    private Image RepairBar;

    void Awake()
    {
        CombustionThreshold = MinimumCombustionThreshold + UnityEngine.Random.Range(0, 50);
        ObjCanvas.GetComponent<CanvasGroup>().alpha = 0;
        foreach (var image in GetComponentInChildren<Canvas>().gameObject.GetComponentsInChildren<Image>())
        {
            if (image.CompareTag("Extinguisher_LoadingBar"))
            {
                RepairBar = image;
                break;
            }
        }
    }

    public virtual void OnBearInteract()
    {
        if (!bIsDestroyed)
        {
            this.Damage = this.Durability;  //Damage can be changed later
            RepairBar.fillAmount = Damage / Durability;
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
            RepairBar.fillAmount = Damage / Durability;

            if (this.Damage <= 0)
            {
                this.Damage = 0;
                this.bIsDestroyed = false;
                this.gameObject.GetComponent<Renderer>().material = DefaultMaterial;
                ObjCanvas.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }

    void OnMouseOver()
    {
        MouseOverObject = true;
        if (bIsDestroyed == true)
        {
            ObjCanvas.GetComponent<CanvasGroup>().alpha = 1;
            Cursor.SetCursor(Resources.Load<Texture2D>("Hammer"), new Vector2(22, 6), CursorMode.ForceSoftware);
            if (Input.GetMouseButtonDown(0) && !AsyncLatch)
            {
                StartCoroutine("RepairTimer");
            }
        }
    }
    void OnMouseExit()
    {
        MouseOverObject = false;
        ObjCanvas.GetComponent<CanvasGroup>().alpha = 0;
        Cursor.SetCursor(Resources.Load<Texture2D>("pointer"), new Vector2(22, 6), CursorMode.ForceSoftware);
    }

    void Update()
    {
        if(this.CombustionLatch == false && this.bIsDestroyed && !(this.MouseOverObject == true && Input.GetMouseButtonDown(0)))
        {
            StartCoroutine("CombustionTimer");
        }

    }

    protected IEnumerator RepairTimer()
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
            CombustionCounter--;
            if (Input.GetMouseButton(0) && this.MouseOverObject)
            {
                RepairerInteract();
            }
            this.AsyncLatch = false;
        }
    }

    protected IEnumerator CombustionTimer()
    {
        this.CombustionLatch = true;

        yield return new WaitForSeconds(CombustionIncrement);
        this.CombustionCounter++;
        this.CombustionLatch = false;
        if (CombustionCounter >= CombustionThreshold)
        {
            this.CombustionCounter = 0;
            Instantiate(Resources.Load<GameObject>("FireObject"),new Vector3(transform.position.x, transform.position.y, transform.position.z-5),Quaternion.identity);
        }
    }

}   
