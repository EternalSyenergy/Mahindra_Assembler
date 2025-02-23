using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectfloat : MonoBehaviour
{
    // Start is called before the first frame update


    public Transform positionPoint;

    public bool isfollow;
    public float speed = 2f;

    public bool islookat;

    public GameObject[] uishow;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void LateUpdate()
    {
        if (isfollow)
        {
            //transform.position=Vector3.Lerp(transform.position,positionPoint.position, speed *Time.deltaTime);

            //if (!islookat)
            //{
            // transform.rotation=Quaternion.Lerp(transform.rotation,positionPoint.rotation, speed *Time.deltaTime);

            //}
            //else
            //{
            //    transform.LookAt(PlayerHandInteraction.instance.headLookat);
            //}


        }
    }


    private void OnEnable()
    {
        isfollow = true;
    }

    private void OnDisable()
    {
        isfollow = false;
        

    }




    public void changeLocation(Transform location)
    {
        positionPoint = location;
    }


    public void showPointerImage(int img)
    {
        if (uishow.Length > 0)
        { 
           for(int i = 0; i < uishow.Length; i++)
            {
                uishow[i].SetActive(false);
            }
        
        }
        uishow[img].SetActive(true);

    }
}
