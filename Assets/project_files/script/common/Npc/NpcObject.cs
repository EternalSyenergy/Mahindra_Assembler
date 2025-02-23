using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcObject : MonoBehaviour
{



    public NpcObjectRef npcObjectRef;
    // Start is called before the first frame update
    void Start()
    {
        npcObjectRef.lastWalkSpeed = npcObjectRef.walkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        ActionHandler();
    }


    void ActionHandler()
    {
        npcAction();
    }

    #region sequenceEvent
    public void SwitchActionType()
    {

    }

    #endregion


    private string lastAnimation = "";

    public void AnimationHandler(string newAnim)
    {
        if (npcObjectRef.anim == null || string.IsNullOrEmpty(newAnim)) return;

        AnimatorStateInfo currentState = npcObjectRef.anim.GetCurrentAnimatorStateInfo(0);

        if (lastAnimation != newAnim && !currentState.IsName(newAnim))
        {
            npcObjectRef.anim.CrossFadeInFixedTime(newAnim, 1f);
            lastAnimation = newAnim;
        }
    }

    void npcAction()
    {
        if (npcObjectRef.currentPath == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            npcObjectRef.currentPath.transform.position,
            npcObjectRef.walkingSpeed * Time.deltaTime
        );

        string newAnim = npcObjectRef.walkingAnim;

        if (npcObjectRef.npcType == NpcType.walking)
        {
            RotateTowardsTarget(npcObjectRef.currentPath.transform.position);
            npcObjectRef.walkingSpeed = npcObjectRef.lastWalkSpeed;

            if (Vector3.Distance(transform.position, npcObjectRef.currentPath.transform.position) < 0.1f)
            {
                GetNextPath();
            }
        }
        else if (npcObjectRef.npcType == NpcType.working)
        {
            if (Vector3.Distance(transform.position, npcObjectRef.currentPath.transform.position) < 1.5f)
            {
                newAnim = npcObjectRef.working;
                npcObjectRef.walkingSpeed = 0f;
            }
        }

        AnimationHandler(newAnim);
    }




    void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    void GetNextPath()
    {
        if (npcObjectRef.currentPath.nextpath.Count > 0)
        {
            int randomIndex = Random.Range(0, npcObjectRef.currentPath.nextpath.Count);

            npcObjectRef.currentPath = npcObjectRef.currentPath.nextpath[randomIndex];
        }
    }







}
public enum NpcType
{
    standing,
    working,
    walking,
    talking,

}

[System.Serializable]
public class NpcObjectRef
{
    public NpcType npcType = NpcType.standing;
    public Animator anim;
    public NpcPath currentPath;

    public float walkingSpeed = 0.2f;

    ///hide function
    [HideInInspector]
    public float lastWalkSpeed;
    [HideInInspector]
    public string walkingAnim = "walking";
    [HideInInspector]
    public string working = "working";
    [HideInInspector]
    public string standing = "standing";
    [HideInInspector]
    public string talking = "talking";
}