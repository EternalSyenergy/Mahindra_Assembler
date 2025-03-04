using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{


    public static UiManager instance;
    public LogoHandler logoHandler;
    public StartUi UiFloater;

    public HintUi hintUi;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }








}

[System.Serializable]
public class LogoHandler
{
    public int logoCount;
    public List<Sprite> logos = new List<Sprite>();

}



[System.Serializable]
public class scoreUiHandler
{
    public int PlayerScore;

}

[System.Serializable]
public class StartUi
{
    public UiFloater StartUiFloater;

}

[System.Serializable]
public class HintUi
{
    public TMP_Text hintText;

}


