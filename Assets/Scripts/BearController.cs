using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BearController : MonoBehaviour
{
    CharacterController cc;
    Animator a;
    ParticleSystem runningEffect;
    ParticleSystem jumpEffect;

    private float FireCoolOffTime = 3.0f;
    private bool bIsOnFire = false;
    private float EndOnFireTime;

    [Header("Character Movement")]
    [SerializeField] float maxHorizontalSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float slowdown;
    [SerializeField] float speedup;
    Vector3 lastMovement = Vector3.zero;
    float verticalSpeed = 0f;
    [HideInInspector] public bool canMove = true;

    [Header("Bear Attack")]
    [SerializeField] private float AttackDist = 3.0f;
    [SerializeField] private float AttackCooldown = 0.5f;

    public InteractableObject AttackObject { get; private set; }
    float lastAttack = 0f;


    [Header("Bear Attack - Selection")]
    private float maxDistToObject = 2.0f;

    private float asyncLookupThreadSleep = 0.15f;


    private void Awake()
    {
        cc = this.GetComponent<CharacterController>();
        a = this.GetComponentInChildren<Animator>();
        runningEffect = this.transform.Find("RunningEffect").GetComponent<ParticleSystem>();
        jumpEffect = this.transform.Find("JumpEffect").GetComponent<ParticleSystem>();

        var particleChildren = GetComponentsInChildren<ParticleSystem>();

        foreach (var particleObject in particleChildren)
        {
            particleObject.Stop(true);
        }
        StartCoroutine(FindClosestObject());
    }

    void Start()
    {
        if (!runningEffect.isPlaying)
        {
            runningEffect.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {

        //Update fire timer if the bear is still in fire
        EndOnFireTime = Time.time + FireCoolOffTime;

        if (!bIsOnFire)
        {
            if (other.gameObject.GetComponent<FireMechanics>())
            {
                var particleChildren = GetComponentsInChildren<ParticleSystem>();

                foreach (var particleObject in particleChildren)
                {
                    particleObject.Play();
                }
            }

            StartCoroutine(StopFire());
        }
    }

    private IEnumerator StopFire()
    {
        bIsOnFire = true;

        //Wait on updating timer
        while (Time.time < EndOnFireTime)
        {
            yield return new WaitForSeconds(0.25f);
        }

        var particleChildren = GetComponentsInChildren<ParticleSystem>();

        foreach (var particleObject in particleChildren)
        {
            particleObject.Stop();
        }

        bIsOnFire = false;
    }

    void Update()
    {
        CheckInput();

        Vector3 movement = lastMovement;

        // Grounded
        bool grounded = false;
        if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), (cc.height / 2f) + cc.skinWidth))
        {
            verticalSpeed = 0f;

            grounded = true;

            // Trigger Jump
            if (Input.GetAxisRaw("Vertical") > 0 && canMove)
            {
                a.SetTrigger("Jump");
                Jump();
            }
        }

        // Left/right movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0 && canMove)
        {
            movement += new Vector3(horizontal / speedup, 0, 0f);
            this.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(horizontal, 0));
            a.SetBool("Running", true);
            runningEffect.enableEmission = grounded;
        }
        else
        {
            movement -= new Vector3(movement.x / slowdown, 0f, 0f);
            a.SetBool("Running", false);
            runningEffect.enableEmission = false;
        }
        movement.x = Mathf.Clamp(movement.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        
        // Jump
        if (doJump)
        {
            if (grounded)
            {
                verticalSpeed = jumpPower;
                doJump = false;
            }
            else
            {
                doJump = false;
            }
        }

        // Gravity
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        movement.y = verticalSpeed;

        cc.Move(movement * Time.deltaTime);
        lastMovement = movement;

        // Transport
        if (transportLocation != Vector3.zero && canMove)
        {
            this.transform.position = transportLocation;
            transportLocation = Vector3.zero;

            // Stop momentum
            lastMovement = Vector3.zero;
            verticalSpeed = 0f;
        }

        // Failsafe for 2d gameplay
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0f);
    }

    bool doJump = false;
    public void Jump()
    {
        doJump = true;
        if (jumpEffect.isPlaying)
        {
            jumpEffect.Stop();
        }
        jumpEffect.Play();
    }
    
    Vector3 transportLocation = Vector3.zero;
    public void Transport(Vector3 location)
    {
        transportLocation = location;
    }

    /// <summary>
    /// Checks all the Character Inputs on Update
    /// </summary>
    private void CheckInput()
    {
        if (Input.GetButtonDown(GameplayStatics.BearInputLookup[BearInput.Bear_Attack]) && Time.time - lastAttack > AttackCooldown && canMove)
        {
            if (Time.time - lastAttack > AttackCooldown)
            {
                lastAttack = Time.time;
                a.SetTrigger("Attack");
                StartCoroutine(DoAfter(BearAttack, 0.25f));
            }
        }
    }

    public void BearAttack()
    {
        if (AttackObject != null)
        {
            //make sure the object isn't destroyed before trying to attack
            if (AttackObject.bIsDestroyed)
            {
                AttackObject.ClearHighlight();
                AttackObject = null;
                return;
            }
            AttackObject.OnBearInteract();

            //update the object's status if it got destroyed during the attack
            if (AttackObject.bIsDestroyed)
            {
                AttackObject.ClearHighlight();
                AttackObject = null;
            }
        }
    }

    IEnumerator DoAfter(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator FindClosestObject()
    {
        while (true)
        {
            if (AttackObject != null)
            {
                //clears out the attack object if it's too far away
                if (Vector3.Distance(transform.position, AttackObject.gameObject.transform.position) > maxDistToObject)
                {
                    AttackObject.ClearHighlight();
                    AttackObject = null;
                }
            }

            var destructableEnumerator = GameObject.FindObjectsOfType<InteractableObject>()
                .Where(obj => !obj.GetComponent<DragableObject>() && !obj.bIsDestroyed);

            foreach (var item in destructableEnumerator)
            {
                var newObjPos = Vector3.Distance(transform.position, item.gameObject.transform.position);
                if (newObjPos < maxDistToObject)
                {
                    if (AttackObject != null)
                    {
                        var attackObjPos = Vector3.Distance(transform.position, AttackObject.gameObject.transform.position);

                        if (newObjPos > attackObjPos)
                        {
                            AttackObject.ClearHighlight();
                            AttackObject = item;
                        }
                    }
                    else
                    {
                        AttackObject = item;
                    }
                }
            }

            if (AttackObject != null)
            {
                AttackObject.SetHighlight();
            }

            //Sleep for a bit and do this all again
            yield return new WaitForSeconds(asyncLookupThreadSleep);
        }

        yield return null;
    }
}
