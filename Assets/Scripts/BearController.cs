using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BearController : MonoBehaviour
{
    CharacterController cc;
    Animator a;

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
    float lastAttack = 0f;



    private void Awake()
    {
        cc = this.GetComponent<CharacterController>();
        a = this.GetComponentInChildren<Animator>();

        var particleChildren = GetComponentsInChildren<ParticleSystem>();

        foreach (var particleObject in particleChildren)
        {
            particleObject.Stop(true);
        }

    }

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<FireMechanics>())
        {
            var particleChildren = GetComponentsInChildren<ParticleSystem>();

            foreach (var particleObject in particleChildren)
            {
                particleObject.Play();
            }
        }
    }

    void Update()
    {
        CheckInput();

        Vector3 movement = lastMovement;

        // Left/right movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0 && canMove)
        {
            movement += new Vector3(horizontal / speedup, 0, 0f);
            this.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(horizontal, 0));
            a.SetBool("Running", true);
        }
        else
        {
            movement -= new Vector3(movement.x / slowdown, 0f, 0f);
            a.SetBool("Running", false);
        }

        movement.x = Mathf.Clamp(movement.x, -maxHorizontalSpeed, maxHorizontalSpeed);

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
            }
        }

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
        if (Input.GetButtonDown(GameplayStatics.BearInputLookup[BearInput.Bear_Attack]) && Time.time - lastAttack > AttackCooldown)
        {
            if (Time.time - lastAttack > AttackCooldown)
            {
                BearAttack();
                a.SetTrigger("Attack");
            }
        }
    }

    public void BearAttack()
    {
        lastAttack = Time.time;

        Vector3 playerPos = this.gameObject.transform.position;
        Vector3 AttackEndPos = playerPos + (this.gameObject.transform.forward * AttackDist);
        Ray attackRay = new Ray(playerPos, AttackEndPos);
        Physics.Raycast(attackRay, out RaycastHit hit, AttackDist);

        hit.transform?.gameObject?.GetComponent<InteractableObject>()?.OnBearInteract();
    }
}
