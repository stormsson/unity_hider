using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationManager : MonoBehaviour
{
    Animator animatorReference;

    public UnityEvent endSurpriseEvt;
    public UnityEvent endPivotEvt;

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

    public void pivot180()
    {

        animatorReference.SetBool("isPivoting", true);
    }

    public void endPivot180()
    {
        
        animatorReference.SetBool("isPivoting", false);
        endPivotEvt.Invoke();
        Debug.Log("End pivoting");
    }
}
