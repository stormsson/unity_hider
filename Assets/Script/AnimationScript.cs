using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    Animator animatorRef;


    private void Awake()
    {
        animatorRef = transform.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animatorRef.GetBool("isWalking");
        bool forwardPressed = Input.GetKey("w");

        if (!isWalking && forwardPressed)
        {
            animatorRef.SetBool("isWalking", true);
            animatorRef.SetFloat("speed", 1.0f);
        }

        if (isWalking && !forwardPressed)
        {
            animatorRef.SetBool("isWalking", false);
            animatorRef.SetFloat("speed", 0.0f);
        }
        
    }
}
