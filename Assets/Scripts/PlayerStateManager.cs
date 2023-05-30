using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerStateManager : MonoBehaviour
{
    // Context of FSM
    private PlayerBaseState currentState;
    private PlayerBaseState secondPlayerCurrentState;
    public PlayerPositiveState PositiveState = new PlayerPositiveState();
    public PlayerNegativeState NegativeState = new PlayerNegativeState();

    // Collision Detection
    private GameObject secondPlayer = null;
    private bool inRange = false;
    private float range;
    private bool merged = false;
    private Vector3 mergeOffset = new Vector3(0, 1, 0);
    private bool xButtonPressed;

    //Magnet
    [SerializeField] private BoxCollider triggerBox;
    private Vector2 secondPlayerDir;
    public float strength = 1f;
    //public float repelForce = 5f;
    //public float attractForce = 5f;


    public void OnMagnetAbility(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            if (!context.performed)
            {
                return;
            }
            currentState.OnSwitchState(this);
        }
    }
    void Start()
    {
        currentState = NegativeState;
        secondPlayerCurrentState = null;

        currentState.EnterState(this);
        range = triggerBox.size.x / 2;
    }

    void Update()
    {
        currentState.UpdateState(this);

        secondPlayerCurrentState = secondPlayer.gameObject.GetComponent<PlayerStateManager>().currentState;

        if (inRange)
        {
            // Get distance to other player
            secondPlayerDir = new Vector3(secondPlayer.transform.position.x - this.transform.position.x, secondPlayer.transform.position.y - this.transform.position.y, secondPlayer.transform.position.z - this.transform.position.z);

            Vector3 rawDirection = transform.position - secondPlayer.transform.position;
            float distance = rawDirection.magnitude;
            float distanceScale = Mathf.InverseLerp(range, 0f, distance);
            float attractionStrength = Mathf.Lerp(0f, strength, distanceScale);

            //Debug.Log(" i am " + secondPlayerDir + " away from other player");

            if (currentState.ReturnState(this) == secondPlayerCurrentState.ReturnState(this))
            {
                this.gameObject.GetComponent<PlayerController>().repel(rawDirection, attractionStrength);
            }
            if (currentState.ReturnState(this) != secondPlayerCurrentState.ReturnState(this))
            {
                if (distance >= 2f)
                {
                    
                    this.gameObject.GetComponent<PlayerController>().attract(rawDirection, attractionStrength);
                }
                
                
            }
        }

    }
    Vector3 targetDirection(Vector3 target)
    {
        return target - this.transform.position;
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState = state;
        state.EnterState(this);
        Debug.Log("SwitchedState...");
    }


    public GameObject OnTriggerEnter(Collider other)
    {

        secondPlayer = other.gameObject;

        if (other.gameObject.GetComponent<PlayerDetails>().playerID != this.GetComponent<PlayerDetails>().playerID)
        {
            inRange = true;
            Debug.Log("in Range");
            secondPlayer = other.gameObject;
        }

        return secondPlayer;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerDetails>().playerID != this.GetComponent<PlayerDetails>().playerID)
        {
            inRange = false;
            Debug.Log("Out of Range");
        }
    }

}
