using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSequence : MonoBehaviour
{

    public int currentInteractionCount = 0;
    public int TotalIntercationCount;
    SequenceManager sequenceManager;

    private void Awake()
    {
        sequenceManager = gameObject.transform.GetComponentInParent<SequenceManager>();
        TotalIntercationCount = gameObject.transform.childCount;


    }
    void Start()
    {



    }

    void Update()
    {
        
    }

    public void StartInteraction()
    {


        if (currentInteractionCount >= TotalIntercationCount)
        {
            sequenceManager.currentSubseuenceCount++;
            sequenceManager.StartsubSequence(sequenceManager.currentSubseuenceCount);


            return;
        }

        sequenceManager.currentInteraction = gameObject.transform.GetChild(currentInteractionCount).gameObject;

        gameObject.transform.GetChild(currentInteractionCount).GetComponent<Interaction>().StartInteractionEvent();
    }
}
