using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiFloater : MonoBehaviour
{

    public Uitype uitype = Uitype.normal;


    Transform playerUiPointer;
    Transform defaultPlayerUiPointer;

    public Transform staticUiPointer;


    float scaleProgress = 0;
    bool isDefaultPosition = false;
    private Vector3 velocity = Vector3.zero; // Required for SmoothDamp
    private float smoothTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerHeadUiref();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void LateUpdate()
    {
        updatePlayerHeadUi();
        updateStaticUi();

    }

    #region player Head Ui Reference
    void PlayerHeadUiref()
    {

        playerUiPointer = GameManager.Instance.PlayerData.PlayerHeadUi;
        defaultPlayerUiPointer = GameManager.Instance.PlayerData.PlayerHeadUi;

        defaultPlayerUiPointer = new GameObject().transform;
        defaultPlayerUiPointer.position = GameManager.Instance.PlayerData.PlayerHeadUi.position + new Vector3(0, 0, 5f);
        defaultPlayerUiPointer.rotation = GameManager.Instance.PlayerData.PlayerHeadUi.rotation;

    }

    void updatePlayerHeadUi()
    {
        if (playerUiPointer != null)
        {

            if (!isDefaultPosition)
            {

                transform.SetPositionAndRotation(defaultPlayerUiPointer.position,defaultPlayerUiPointer.rotation);
                isDefaultPosition = true;
            }


            //transform.position = Vector3.SmoothDamp(transform.position, playerUiPointer.position, ref velocity, smoothTime);
            transform.position = Vector3.Lerp(transform.position, playerUiPointer.position, Time.deltaTime * smoothTime);


            transform.rotation = Quaternion.Slerp(transform.rotation, playerUiPointer.rotation, Time.deltaTime * 5);
        }
    }
    #endregion


    #region static ui ref



    void updateStaticUi()
    {
        if (staticUiPointer != null)
        {


            if (uitype == Uitype.openVerticle)
            {
                transform.SetPositionAndRotation(staticUiPointer.position, staticUiPointer.rotation);
                scaleProgress = Mathf.Clamp01(scaleProgress + Time.deltaTime * 0.5f); // Gradually increase over time
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(0, 1, scaleProgress), transform.localScale.z);
            }
        }
    }


    #endregion


    #region handle Sequnce


    public void SetUiType(Uitype temp)
    {
        uitype = temp;
    }
    public void EnableUi(Transform temp)
    {

        staticUiPointer = temp;
        gameObject.SetActive(true);

    }

    #endregion

    #region Handle ui type
    void handleUiTypeEnable()
    {
        switch (uitype)
        {
            case Uitype.normal:
                transform.localScale = new Vector3(1, 1, 1);
                break;


            case Uitype.openHorizontal:

                transform.localScale = new Vector3(0, 1, 1);
                break;



            case Uitype.openVerticle:

                transform.localScale = new Vector3(1, 0, 1);
                break;




            default:
                break;
        }
    }



    private void OnEnable()
    {

        isDefaultPosition = false;
        handleUiTypeEnable();

    }


    private void OnDisable()
    {
        isDefaultPosition = false;
        scaleProgress = 0;
        //uitype = Uitype.normal;
    }


    #endregion




}

[System.Serializable]
public enum Uitype
{
    normal, openHorizontal, openVerticle
}
