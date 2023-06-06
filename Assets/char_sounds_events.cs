using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class char_sounds_events : MonoBehaviour
{
    public bool Debug_Enabled = false; 
    public AK.Wwise.Event robotStart;
    public AK.Wwise.Event frontStep;
    public AK.Wwise.Event backStep;


    // Start is called before the first frame update
    void Start()
    {

    }
    void Play_RobotPlayerInit()
    {
        robotStart.Post(gameObject);
        Debug.Log("RobotInitTriggered");
    }

    void FrontStepAnim()
    {
        frontStep.Post(gameObject);
    }
    void BackStepAnim()
    {
        backStep.Post(gameObject);
    }
}
