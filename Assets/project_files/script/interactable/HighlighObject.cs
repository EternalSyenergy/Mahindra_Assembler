using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlighObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveHighlight(Transform temp)
    {
        transform.SetPositionAndRotation(temp.position, temp.rotation);
    }
}
