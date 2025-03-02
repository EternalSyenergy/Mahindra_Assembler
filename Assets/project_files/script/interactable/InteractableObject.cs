using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    // Start is called before the first frame update

    public bool inHand;
  
    public Transform handPos;
    public Transform fixedPos;


    public UnityEvent pickEvent;
    public UnityEvent dropEvent;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateIsHand();

    }




    void updateIsHand()
    {
        if (inHand)
        {
            transform.SetPositionAndRotation(handPos.position, handPos.rotation);
        }
        else
        {
            if (fixedPos != null)
            {
                transform.SetPositionAndRotation(fixedPos.position, fixedPos.rotation);

            }
        }
    }



    public void IsHand(bool temp)
    {
        inHand = true;
        pickEvent.Invoke();
    }

    public void Fixedpos(Transform temp)
    {
        inHand = false;
        transform.SetPositionAndRotation(temp.position, temp.rotation);
        dropEvent.Invoke();
    }



}



