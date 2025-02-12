using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{


    public startEvent startEvent;

    SubSequence subSequence;

    private void Awake()
    {
        subSequence = gameObject.transform.GetComponentInParent<SubSequence>();

    }
    void Start()
    {
    }

    void Update()
    {

    }

    #region sequence intercation Handler
    public void StartInteractionEvent()
    {
        startEvent.Event.Invoke();
        LanguageHandler();
        HandleTimerInteraction();

    }

    void HandleTimerInteraction()
    {
        if (startEvent.isTimeIntercation)
        {
            CancelInvoke("CompleteInteractionEvent");
            Invoke("CompleteInteractionEvent", startEvent.intercationTimer);

        }

    }

    void handleCompleteEvent()
    {
        subSequence.currentInteractionCount++;
        subSequence.StartInteraction();
    }
    public void CompleteInteractionEvent()
    {

        if (!startEvent.isTimeIntercation)
        {

            CancelInvoke("handleCompleteEvent");
            Invoke("handleCompleteEvent", startEvent.intercationTimer);

        }
        else
        {
            handleCompleteEvent();
        }


    }


    #endregion


    #region Langugae player
    public void LanguageHandler()
    {

    }
    #endregion


}

[Serializable]
public class startEvent
{
    public UnityEvent Event;
    public List<AudioClip> Language;
    public bool isTimeIntercation;
    public float intercationTimer;


}
