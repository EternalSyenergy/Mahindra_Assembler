using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NpcPath : MonoBehaviour
{
    // Start is called before the first frame update

    public List<NpcPath> nextpath;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 

    private void OnDrawGizmos()
    {
        //if (nextpath == null || nextpath.Count < 1) return;

        Gizmos.color = Color.red; // Set line color

        for (int i = 0; i < nextpath.Count; i++)
        {
            if (nextpath[i] != null)
            {
                Gizmos.DrawLine(transform.position, nextpath[i].gameObject.transform.position);
            }
        }
    }

}
