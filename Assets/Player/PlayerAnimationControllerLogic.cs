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
        DoFootIK(AvatarIKGoal.LeftFoot, leftFoot, animator.leftFeetBottomHeight, animator.GetFloat("LeftFootGround"), out var rFHit);
        DoFootIK(AvatarIKGoal.RightFoot, rightFoot, animator.rightFeetBottomHeight, animator.GetFloat("RightFootGround"), out var lFHit);

        var offset = GetOffset(lFHit.point, rFHit.point);
        fixPosition(offset);
    }

    private void DoFootIK(AvatarIKGoal foot, Transform footTransform, float footBottomHeight, float weight, out RaycastHit hit)
    {
        Vector3 pos = footTransform.position;
        var rot = footTransform.rotation;

        var ray = new Ray(pos + Vector3.up, Vector3.down);

        var didHit = false;

        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            didHit = true;

            if (hit.transform.CompareTag("Walkable"))
            {
                Vector3 footPos = hit.point;
                footPos.y += footBottomHeight;

                Vector3 distance = footPos - animator.GetIKPosition(foot);

                if (distance.sqrMagnitude > 0.015f)
                {
                    pos = footPos;
                    rot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, didHit ? Color.green : Color.red, 0f, false);

        animator.SetIKPositionWeight(foot, weight);
        animator.SetIKRotationWeight(foot, weight);

        animator.SetIKPosition(foot, pos);
        animator.SetIKRotation(foot, rot);
    }

    float _lastBodyY;
    void fixPosition(float offset)
    {
        var newPos = animator.bodyPosition;
        newPos.y += offset;
        newPos.y = Mathf.MoveTowards(_lastBodyY, newPos.y, 5f * Time.deltaTime);

        animator.bodyPosition = newPos;
        _lastBodyY = newPos.y;
    }

    private float GetOffset(Vector3 lfPos, Vector3 rfPos) => Mathf.Min(lfPos.y - transform.position.y - animator.leftFeetBottomHeight, rfPos.y - transform.position.y - animator.rightFeetBottomHeight);
}