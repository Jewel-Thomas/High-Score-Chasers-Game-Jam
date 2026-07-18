using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    private Gamepad pad;
    private Coroutine StopRumbleaftertime;
    //public GameObject car;

    //Collider Carbody = car.GetComponent<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rumblestart(float LF, float HF, float duration)
    {
        
        pad = Gamepad.current;
        if(pad != null)
        {
            pad.SetMotorSpeeds(LF,HF);
        }
        StopRumbleaftertime = StartCoroutine(StopRumble(duration,pad));
    }
    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float TimeTaken=0f;
        while(TimeTaken<duration)
        {
            TimeTaken += Time.deltaTime;
            yield return null;
        }
        pad.SetMotorSpeeds(0f,0f);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided");
        
        if (other.gameObject.tag == "level") 
        {
            
            Rumblestart(0.2f, 1f, 0.2f);
        }
        
    }

}
