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
    // Start is called before the first frame update
    void Start()
    {
        PlayerHeadUiref();
    }

    // Update is called once per frame
    void Update()
    {
        updateStaticUi();
    }
    private void LateUpdate()
    {
        updatePlayerHeadUi();
        //updateStaticUi();
    }

    #region player Head Ui Reference
    void PlayerHeadUiref()
    {

        if (uitype == Uitype.normal)
        {

            playerUiPointer = GameManager.Instance.PlayerData.PlayerHeadUi;
            defaultPlayerUiPointer = GameManager.Instance.PlayerData.PlayerHeadUi;

            defaultPlayerUiPointer.position = new Vector3(defaultPlayerUiPointer.position.x, defaultPlayerUiPointer.position.y, defaultPlayerUiPointer.position.z + 5f);

        }
    
    }

    void updatePlayerHeadUi()
    {
        if (defaultPlayerUiPointer != null)
        {

        }
    }
    #endregion


    #region static ui ref



    void updateStaticUi()
    {
        if (staticUiPointer != null)
        {
           transform.SetPositionAndRotation(staticUiPointer.position, staticUiPointer.rotation);


            if (uitype == Uitype.openVerticle)
            {

                //transform.localScale = new Vector3(transform.localScale.x,Mathf.Lerp(0,1,0.1f), transform.localScale.z);
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
        handleUiTypeEnable();

    }


    private void OnDisable()
    {
        uitype = Uitype.normal;
    }


    #endregion




}

[System.Serializable]
public enum Uitype
{
    normal, openHorizontal, openVerticle
}
