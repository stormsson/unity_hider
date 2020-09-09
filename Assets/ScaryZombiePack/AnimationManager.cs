using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationManager : MonoBehaviour
{
    Animator animatorReference;

    public UnityEvent endSurpriseEvt;

    // Start is called before the first frame update
    void Start()
    {
        animatorReference = GetComponent<Animator>();
    }

    public void goIdle()
    {
        animatorReference.SetFloat("speed", 0f);
    }

    public void goRun()
    {
        animatorReference.SetFloat("speed", 1);
    }

    public void goSurprised()
    {

        animatorReference.SetBool("isSurprised", true);
    }

    public void goUnsurprised()
    {
        animatorReference.SetBool("isSurprised", false);
        endSurpriseEvt.Invoke();
        
    }
}
