using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControllerLogic : MonoBehaviour
{
    private Animator animator;

    const int MAX_IDLE_ANIMATIONS = 3;

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

    void OnDrawGizmos()
    {
        if (!TryGetComponent<Animator>(out var anim))
            return;

        anim.logWarnings = false;

        Gizmos.DrawSphere(anim.GetIKPosition(AvatarIKGoal.LeftFoot), 0.1f);
        Gizmos.DrawSphere(anim.GetIKPosition(AvatarIKGoal.RightFoot), 0.1f);

        anim.logWarnings = true;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        DoFootIK(AvatarIKGoal.LeftFoot, leftFoot, animator.GetFloat("LeftFootGround"), out var rFHit);
        DoFootIK(AvatarIKGoal.RightFoot, rightFoot, animator.GetFloat("RightFootGround"), out var lFHit);

        if (lFHit.point.y <= rFHit.point.y)
        {
            fixPosition(lFHit.point);
        }
        else
        {
            fixPosition(rFHit.point);
        }
    }

    private void DoFootIK(AvatarIKGoal foot, Transform footTransform, float weight, out RaycastHit hit)
    {
        animator.SetIKPositionWeight(foot, weight);
        animator.SetIKRotationWeight(foot, weight);

        Vector3 pos = footTransform.position;

        var ray = new Ray(pos + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, 5.0f))
        {
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPos = hit.point;
                footPos.y += distanceToGround;

                var rot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                animator.SetIKRotation(foot, rot);

                Vector3 distance = footPos - animator.GetIKPosition(foot);

                if (distance.sqrMagnitude > 0.015f)
                {
                    animator.SetIKPosition(foot, footPos);

                }
            }
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