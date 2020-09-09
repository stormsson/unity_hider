using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    // genericik https://www.youtube.com/watch?v=EggUxC5_lGE

    // footik https://www.youtube.com/watch?v=rGB1ipH6DrM
    Animator anim;
    public Transform leftIKTarget;
    public Transform rightIKTarget;

    // knees
    public Transform hintLeft;
    public Transform hintRight;

    public float ikWeight = 1;


    Vector3 lFPos;
    Vector3 rFpos;

    Quaternion lFRot;
    Quaternion rFRot;

    float lFWeight;
    float rFWeight;

    [Range(0, 1f)]
    public float distanceToGround = 0.09f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnAnimatorIK(int layerIndex)
    {



        // LEFT FOOT

        lFWeight = anim.GetFloat("leftFootStatic");
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);

        RaycastHit lFHit;
        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if(Physics.Raycast(ray, out lFHit, 5.0f))
        {
            if (lFHit.transform.tag =="Walkable")
            {
                Vector3 footPos = lFHit.point;
                footPos.y += distanceToGround;

                leftIKTarget.position = footPos;
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKTarget.position);

            }
        }


        
        



        // RIGHT FOOT
        rFWeight = anim.GetFloat("rightFootStatic");
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);

        RaycastHit rFHit;
        ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out rFHit, 5.0f))
        {
            if (rFHit.transform.tag == "Walkable")
            {
                Vector3 footPos = rFHit.point;
                footPos.y += distanceToGround;

                rightIKTarget.position = footPos;
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKTarget.position);

            }
        }




        /*
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKTarget.position);

        

        anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, ikWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, ikWeight);

        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, hintLeft.position);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, hintRight.position);
        

        // Rotation

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftIKTarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightIKTarget.rotation);
        */



    }
}
