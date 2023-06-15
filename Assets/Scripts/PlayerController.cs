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
    private bool canMove = true;
    private bool merged = true;

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
       
        if (canMove)
        {
            Move();
            MoveAnimation();
        }

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

    public void repel(Vector3 rawDirection, float attractionStrength, float newAttractionStrength)
    {
        if (merged)
        {
            rb.mass = 1;
            rb.useGravity = true;
            gameObject.transform.parent = null;
            //rb.AddForce(Vector3.up * (jumpHeight*2), ForceMode.Impulse);
            rb.AddForce(rawDirection.normalized * newAttractionStrength* playerSpeed*3, ForceMode.Impulse);
            gameObject.GetComponent<Collider>().enabled = true;
            merged = false;
           
            return;
        }
        rb.AddForce(rawDirection.normalized * newAttractionStrength * playerSpeed*3, ForceMode.Force);
        Debug.Log("Push away");
    }

    public void attract(Vector3 rawDirection, float attractionStrength, float distance)
    {
        //deactivate rb
        //rb.isKinematic = false;

        //transform.position += rawDirection * (attractionStrength * 1 / distance) * Time.deltaTime;

       
        rb.AddForce(rawDirection.normalized * (attractionStrength * playerSpeed) * (-1), ForceMode.Force);
        Move();
        Debug.Log("attract");
    }

    /*public void OnSwitchPlayerToFollow (InputAction.CallbackContext context)
    {
        switchPlayer = context.ReadValue<bool>();
    }*/


    private void checkIfMerged()
    {
        if (merged)
        {
            canMove = false;
        }
        if (!merged)
        {
            canMove = true;
        }
    }
    private void detach()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.transform.parent = null;
        merged = false;
    }

    public void attach(GameObject secondPlayer)
    {
        Debug.Log("merged...");
        rb.useGravity = false;
        rb.mass = 0;
        transform.position = secondPlayer.transform.position + mergeOffset;
        gameObject.transform.parent = secondPlayer.transform;

        gameObject.GetComponent<Collider>().enabled = false;
        merged = true;
    }
}
