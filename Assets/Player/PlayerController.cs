using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Animator animator;
    CharacterController controller;

   
    [Header("Movement configuration")]
    public float directionDampTime = 0.25f;
    public float hSpeedMultiplier = 3f;
    public float vSpeedMultiplier = 10f;

    public float rotSpeed = 1f;


    float speed = 0f;
    float h = 0f;
    float v = 0f;

    Vector3 direction;

    protected GameObject GFX;

    Camera m_MainCamera;
    public GameObject feetCameraObj;
    Camera m_CameraTwo;
    // Start is called before the first frame update
    void Start()
    {
        GFX = transform.Find("GFX").gameObject;

        animator = GFX.GetComponent<Animator>();
        //controller = GetComponent<CharacterController>();
        controller = GameObject.Find("Player").GetComponent<CharacterController>();

        if(!animator )
        {
            Debug.LogError("Animator component needed!");
        }

        if (!controller)
        {
            Debug.LogError("CharacterController component needed!");
        }

        m_MainCamera = Camera.main;

        m_CameraTwo = feetCameraObj.GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        speed = new Vector2(h*hSpeedMultiplier, v*vSpeedMultiplier).sqrMagnitude;

        direction = new Vector3(0, 0, v).normalized;
        direction = transform.TransformDirection(direction);

        if(animator)
        {
            animator.SetFloat("h", h);
            animator.SetFloat("v", v);
            animator.SetFloat("speed", speed);

            //animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }


        if(direction.magnitude > 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }

        

        // UTILITIES
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(controller.isGrounded);
            //Check that the Main Camera is enabled in the Scene, then switch to the other Camera on a key press
            if (m_MainCamera.enabled)
            {
                //Enable the second Camera
                m_CameraTwo.enabled = true;

                //The Main first Camera is disabled
                m_MainCamera.enabled = false;
            }
            //Otherwise, if the Main Camera is not enabled, switch back to the Main Camera on a key press
            else if (!m_MainCamera.enabled)
            {
                //Disable the second camera
                m_CameraTwo.enabled = false;

                //Enable the Main Camera
                m_MainCamera.enabled = true;
            }
        }

    }

}
