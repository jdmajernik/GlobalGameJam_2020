using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BearController : MonoBehaviour
{
    CharacterController cc;

    [Header("Character Movement")] [SerializeField]
    float maxHorizontalSpeed;

    [SerializeField] float jumpPower;
    [SerializeField] float slowdown;
    [SerializeField] float speedup;

    [Header("Bear Attack")] [SerializeField]
    private float AttackDist = 3.0f;

    Vector3 lastMovement = Vector3.zero;
    float verticalSpeed = 0f;
    [HideInInspector] public bool canMove = true;



    private void Awake()
    {
        cc = this.GetComponent<CharacterController>();
    }

    void Start()
    {

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
        }
        else
        {
            movement -= new Vector3(movement.x / slowdown, 0f, 0f);
        }

        movement.x = Mathf.Clamp(movement.x, -maxHorizontalSpeed, maxHorizontalSpeed);

        // Grounded
        if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), (cc.height / 2f) + cc.skinWidth))
        {
            verticalSpeed = 0f;
            if (Input.GetAxisRaw("Vertical") > 0 && canMove)
            {
                verticalSpeed = jumpPower;
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

    Vector3 transportLocation = Vector3.zero;

    public void Transport(Vector3 location)
    {
        transportLocation = location;
    }

    /// <summary>
    /// Checks all the Character Inputs on Update
    /// </summary>
    void CheckInput()
    {
        if (Input.GetButtonDown("BearAttack"))
        {
            Vector3 playerPos = this.gameObject.transform.position;
            Vector3 AttackEndPos = playerPos + (this.gameObject.transform.forward * AttackDist);
            Ray attackRay = new Ray(playerPos, AttackEndPos);
            Physics.Raycast(attackRay, out RaycastHit hit, AttackDist);

            hit.transform?.gameObject?.GetComponent<InteractableObject>()?.OnBearInteract();

        }
    }
}
