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
        handleContent();
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

        ///
        if (GameManager.Instance.PlayerData.mainPlayerAudio.isPlaying)
            GameManager.Instance.PlayerData.mainPlayerAudio.Stop();
        //
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
    void LanguageHandler()
    {
        if (GameManager.Instance.PlayerData.mainPlayerAudio != null)
        {
            if (startEvent.Language.Count > 0)
            {
                GameManager.Instance.PlayerData.mainPlayerAudio.clip = startEvent.Language[GameManager.Instance.Languages];

                if (GameManager.Instance.PlayerData.mainPlayerAudio.isPlaying)
                    GameManager.Instance.PlayerData.mainPlayerAudio.Stop();
                InvokeRepeating("repeatedSequenceAudio", .5f, startEvent.Language[GameManager.Instance.Languages].length + 25);

            }
        }
    }

    void repeatedSequenceAudio()
    {
        GameManager.Instance.PlayerData.mainPlayerAudio.Play();

    }

    #endregion


    #region handle content

    void handleContent()
    {

        if (startEvent.content.Count > 0)
        {
            //GameManager.Instance.PlayerData.mainPlayerAudio.clip = startEvent.Language[GameManager.Instance.Languages];


            UiManager.instance.hintUi.hintText.text= startEvent.content[GameManager.Instance.Languages];
            //ui manger to update on ui



            ///

        }
    }
    #endregion


}

[Serializable]
public class startEvent
{
    public UnityEvent Event;
    public List<AudioClip> Language;
    public List<String> content;
    public bool isTimeIntercation;
    public float intercationTimer;



}
