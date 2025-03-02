using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnInteraction : MonoBehaviour
{

    public AudioSource btnsound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        
       
    }

    private void OnTriggerExit(Collider other)
    {

        Button temp = other.gameObject.GetComponent<Button>() != null ? other.gameObject.GetComponent<Button>() : null;

        if (temp != null) { 
           temp.onClick.Invoke();
            btnsound.Play();
        }

    }
}
