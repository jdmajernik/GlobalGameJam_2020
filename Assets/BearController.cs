using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BearController : MonoBehaviour
{
    CharacterController cc;

    [SerializeField] float maxHorizontalSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float slowdown;
    [SerializeField] float speedup;

    Vector3 lastMovement = Vector3.zero;
    float verticalSpeed = 0f;



    private void Awake()
    {
        cc = this.GetComponent<CharacterController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 movement = lastMovement;

        // Left/right movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            movement += new Vector3(horizontal / speedup, 0, 0f);
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
            if (Input.GetAxisRaw("Vertical") > 0)
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
        if (transportLocation != Vector3.zero)
        {
            this.transform.position = transportLocation;
            transportLocation = Vector3.zero;

            // Cancel momentum
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
}
