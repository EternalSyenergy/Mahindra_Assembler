using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;

    [Tooltip("English - 0, Tamil - 1 ,Hindi - 2 , Malayalam - 3 ")]
    public int Languages=0;
    public PlayerData PlayerData;
    public void Awake()
    {
        Instance=this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void teleportPlayer(Transform temp)
    {
        PlayerData.PlayerObject.transform.SetPositionAndRotation(temp.position,temp.rotation);

    }
}

[System.Serializable]
public class PlayerData
{
    public GameObject PlayerObject;
    public AudioSource mainPlayerAudio;
    public Transform PlayerHeadUi;
}
