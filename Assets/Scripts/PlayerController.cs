using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 3f;
    private bool grounded;
    [SerializeField] private float playerSpeed;
    public Rigidbody rb;
    private Vector3 PlayerMovementInput;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    [SerializeField] private float sensitivity;
    private float lookRotation;
    //public GameObject CamHolder;

    public Transform playerModel;

    //private bool switchPlayer = false;
    private GameObject playerToFollow;
    private GameObject followingPlayer;
    private GameObject player01;
    private GameObject player02;
    public float maxDistance = 2f;

    public Vector3 mergeOffset;
    //private bool canMove = true;

    //Animator
    Animator anim;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {

        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    // Start is called before the first frame update
    void Start()
    {

        if (gameObject.GetComponent<PlayerDetails>().playerID == 1)
        {
            player01 = gameObject;
            Debug.Log(player01 + " hat die ID " + gameObject.GetComponent<PlayerDetails>().playerID);
        }
        if (gameObject.GetComponent<PlayerDetails>().playerID == 2)
        {
            player02 = gameObject;
            Debug.Log(player02 + " hat die ID " + gameObject.GetComponent<PlayerDetails>().playerID);
        }

    }

    // Update is called once per frame
    void Update()
    {

        PlayerMovementInput = new Vector3(movementInput.x, 0f, movementInput.y);

        Move();
        MoveAnimation();

        transform.Rotate(Vector3.up * lookInput.x * sensitivity);
        //lookRotation += (-lookInput.y * sensitivity);
        //CamHolder.transform.eulerAngles = new Vector3(lookRotation, CamHolder.transform.eulerAngles.y, CamHolder.transform.eulerAngles.z);

        //Check if grounded
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

    }

    void Move()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * playerSpeed;
        rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);

        // rotate playerModel in Move direction
        //playerModel.forward = MoveVector;
        
    }

    void MoveAnimation()
    {
        if (PlayerMovementInput != Vector3.zero && grounded)
        {
            anim.SetBool("Walk_Anim", true);
        }
        else
        {
            anim.SetBool("Walk_Anim", false);
        }
    }

    void Jump()
    {
        if (grounded)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    public void repel(Vector3 rawDirection, float attractionStrength)
    {

        rb.AddForce(rawDirection.normalized * attractionStrength, ForceMode.Force);
        Debug.Log("Push away");
    }

    public void attract(Vector3 rawDirection, float attractionStrength)
    {

        //Vector3.MoveTowards(followingPlayer.transform.position, playerToFollow.transform.position, maxDistance);
        //Rigidbody rbForce = followingPlayer.GetComponent<Rigidbody>();

        //rbForce.AddForce(rawDirection.normalized * attractionStrength*(-1), ForceMode.Force);
        rb.AddForce(rawDirection.normalized * attractionStrength * (-1), ForceMode.Force);
        Move();
        Debug.Log("attract");
    }

    /*public void OnSwitchPlayerToFollow (InputAction.CallbackContext context)
    {
        switchPlayer = context.ReadValue<bool>();
    }*/


    /*private void Detach()
    {
        PlayerController.CanMove = true;
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.transform.parent = null;
        merged = false;
    }*/

    public void Attach()
    {
        Debug.Log("merged...");

        followingPlayer.transform.position = playerToFollow.transform.position + mergeOffset;
        followingPlayer.transform.parent = playerToFollow.transform;

        gameObject.GetComponent<Collider>().enabled = false;
    }
}
