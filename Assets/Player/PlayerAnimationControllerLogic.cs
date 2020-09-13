using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControllerLogic : MonoBehaviour
{
    private Animator animator;

    const int MAX_IDLE_ANIMATIONS = 3;

    const float HIPS_DEFAULT_HEIGHT = 1.015754f;

    // idle Transitions handling
    private float idleMultiplier;
    private float idleSelector = 0f;
    private float lastIdleSelector = 0f;
    private float idleTarget = 0f;
    private float idleTransitionTime = 0.25f;
    private float idleTimeElapsed = 0f;


    // IK FEET

    [Header("IK Parameters")]
    public Transform leftFoot;
    public Transform rightFoot;
    public float distanceToGround = 0.8f;



    // knees
    //public Transform hintLeft;
    //public Transform hintRight;

    //public float ikWeight = 1;

    Vector3 lFPos;
    Vector3 rFpos;

    Quaternion lFRot;
    Quaternion rFRot;

    float lFWeight;
    float rFWeight;



    Transform hipsTransform;

    private void Start()
    {
        animator = GetComponent<Animator>();
        idleMultiplier = 1f / ((float)MAX_IDLE_ANIMATIONS);

        hipsTransform = transform.Find("mixamorig:Hips");

    }

    private void Update()
    {
        if (Mathf.Abs(idleSelector - idleTarget) > 0.01)
        {




            if (idleTimeElapsed < idleTransitionTime)
            {
                idleSelector = Mathf.Lerp(lastIdleSelector, idleTarget, idleTimeElapsed / idleTransitionTime);
                idleTimeElapsed += Time.deltaTime;
                animator.SetFloat("idleAnimationSelector", idleSelector);
            }
            else
            {
                idleSelector = idleTarget;
                animator.SetFloat("idleAnimationSelector", idleSelector);
            }
        }


        // IK Target


    }

    public void endIdleAnimation()
    {
        // we use -0.01f in order to avoid MAX_IDLE_ANIMATIONS as a result
        float rng = Random.Range(0f, MAX_IDLE_ANIMATIONS - 0.01f);
        float tmp = Mathf.Floor(rng);
        float randomSelector = idleMultiplier * tmp;

        if (randomSelector != lastIdleSelector)
        {
            lastIdleSelector = animator.GetFloat("idleAnimationSelector");
            idleTarget = randomSelector;
            idleTimeElapsed = 0f;
            Debug.Log("I want to change to " + idleTarget);
        }


    }


    private Vector3 tmpL;
    private Vector3 tmpR;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(leftFoot.transform.position, 0.1f);

        Gizmos.DrawSphere(rightFoot.transform.position, 0.1f);

        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);

        Gizmos.DrawSphere(tmpL, 0.05f);
        Gizmos.DrawSphere(tmpR, 0.05f);
    }

    private void OnAnimatorIK(int layerIndex)
    {


        // LEFT FOOT

        lFWeight = animator.GetFloat("LeftFootGround");

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFWeight);

        RaycastHit lFHit;
        Vector3 lPos = leftFoot.transform.position;
        Ray ray = new Ray(lPos + Vector3.up, Vector3.down);



        if (Physics.Raycast(ray, out lFHit, 5.0f))
        {
            if (lFHit.transform.tag == "Walkable")
            {
                Vector3 footPos = lFHit.point;
                footPos.y += distanceToGround;
                tmpL = footPos;



                lFRot = Quaternion.FromToRotation(transform.up, lFHit.normal) * transform.rotation;
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, lFRot);

                Vector3 distance = footPos - animator.GetIKPosition(AvatarIKGoal.LeftFoot);

                if(distance.sqrMagnitude > 0.05f)
                {
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);

                }



            }
        }



        // RIGHT FOOT
        rFWeight = animator.GetFloat("RightFootGround");
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFWeight);

        RaycastHit rFHit;
        Vector3 rPos = rightFoot.transform.position;

        ray = new Ray(rPos + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out rFHit, 5.0f))
        {
            if (rFHit.transform.tag == "Walkable")
            {
                Vector3 footPos = rFHit.point;
                footPos.y += distanceToGround;
                tmpR = footPos;


                rFRot = Quaternion.FromToRotation(transform.up, rFHit.normal) * transform.rotation;
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rFRot);

                Vector3 distance = footPos - animator.GetIKPosition(AvatarIKGoal.RightFoot);

                if (distance.sqrMagnitude > 0.05f)
                {
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);

                }


            }
        }

        if (lFHit.point.y <= rFHit.point.y)
        {
            fixPosition(lFHit.point);
        }
        else
        {
            fixPosition(rFHit.point);
        }




    }

    float posLerp = 0f;
    void fixPosition(Vector3 pos)
    {

        if(Mathf.Abs(transform.position.y - pos.y) > 0.05f)
        {
            if (posLerp > 1.0f)
            {
                posLerp = 0f;
            }

            Vector3 newPos = transform.position;
            newPos.y = Mathf.Lerp(transform.position.y, pos.y, posLerp);
            posLerp += 0.05f * Time.deltaTime;
            transform.position = newPos;
        }

    }

}