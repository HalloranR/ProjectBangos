using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Water Surface")]
    public Transform waterTransform;

    [Header("Player Child Asset")]
    public GameObject playerGraphic;

    [Header("Physics vars")]
    public float gravity = 1f;
    public float boyancy = 1f;

    public float horizontalMaxSpeed;
    public float horizontalAcceleration;
    public float horizontalDeacceleration;

    [Header("Button Vars")]
    public bool jumpPressed = false;
    public bool divePressed = false;
    public float diveTimeCap = 2f;
    public float jumpTimeCap = 2f;
    public float diveRate = 1;

    [Header("Private Vars")]
    //playerControls
    PlayerControls controls;
    PlayerControls.PlayerMovementActions movementActions;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 inputVelocity;
    [SerializeField] Vector2 horizontalInputs;
    [SerializeField] float diveTime = 0;
    [SerializeField] float jumpTime = 0;

    private void Awake()
    {
        controls = new PlayerControls();
        movementActions = controls.PlayerMovement;
    }

    //enable player movement after awake
    private void OnEnable()
    {
        controls.Enable();
    }

    //disable player movement when done
    private void OnDisable()
    {
        controls.Disable();
    }

    public void OnHorizontalMovement(InputValue value) => horizontalInputs = value.Get<Vector2>();

    public void OnJump(InputValue value) => Jump();
    public void OnDive(InputValue value) => Dive();

    void FixedUpdate()
    {

        if(divePressed && (diveTime <= diveTimeCap))
        {
            //Debug.Log("Here");
            diveTime += Time.deltaTime;

            inputVelocity.y = inputVelocity.y - diveRate;
        }
        else if (divePressed) //if the time has exceeded the limit but the player still holds the button
        {
            //do the same thing as if let go underwater
            inputVelocity.y = inputVelocity.y + boyancy;
        }

        //start of the physics system
        if(transform.position.y > waterTransform.position.y) //if above the water bring down
        {
            inputVelocity.y = - gravity;
        }
        else if(transform.position.y < waterTransform.position.y && !divePressed) //if below the water drag up
        {
            inputVelocity.y = inputVelocity.y + boyancy;
        }


        //Decay horizontal movement if movement is stopped
        if(horizontalInputs.x == 0 && rb.velocity.x != 0)
        {
            inputVelocity.x = Mathf.Lerp(inputVelocity.x, 0, horizontalDeacceleration);
        }
        else //Accelerate
        {
            float tempX = inputVelocity.x + horizontalAcceleration * horizontalInputs.x;

            inputVelocity.x = Mathf.Clamp(tempX, -horizontalMaxSpeed, horizontalMaxSpeed);
        }

        if (horizontalInputs.y == 0 && rb.velocity.z != 0)
        {
            inputVelocity.z = Mathf.Lerp(inputVelocity.z, 0, horizontalDeacceleration);
        }
        else //accelerate
        {
            float tempY = inputVelocity.z + horizontalAcceleration * horizontalInputs.y;

            inputVelocity.z = Mathf.Clamp(tempY, -horizontalMaxSpeed, horizontalMaxSpeed);
        }

        //move the player by adding the velocity
        MovePlayer();
    }

    public void MovePlayer()
    {
        //add velocity to current
        rb.velocity = inputVelocity;

        if(inputVelocity != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(inputVelocity, Vector3.up);

            playerGraphic.transform.rotation = toRotate;
        }
    }

    public void Jump()
    {
        if (jumpPressed) //button was already pressed
        {
            Debug.Log("Button Released");
            jumpPressed = false;
        }
        else
        {
            Debug.Log("Button Pressed");
            jumpPressed = true;
        }
    }

    public void Dive()
    {
        if (divePressed) //button was already pressed
        {
            Debug.Log("Dive Button Released");

            //reset the time and the bool
            diveTime = 0;
            divePressed = false;
        }
        else
        {
            Debug.Log("Dive Button Pressed");
            divePressed = true;
        }
    }
}
