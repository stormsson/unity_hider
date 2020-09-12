using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Behavior : MonoBehaviour
{

    public GameObject fovObjReference;
    public GameObject bodyObjReference;
    Animator animatorReference;
    AnimationManager animationManager;

    public GameObject movementTarget;

    Vector3 lastPlayerKnownPosition;
    GameObject playerRef;
    GameObject fleeingFromRef;

    enum ACTIVITY_STATE {
        IDLE,
        SURPRISED,
        SEEKING,
        SEARCHING,
        FLEEING
    };

    ACTIVITY_STATE previousState;
    ACTIVITY_STATE currentState;

    
    float runningSpeed = 3f;
    float rotationSpeed = 2f;
    public float minSeekingDistance = 1f;

    protected bool oppositeSeek = false;

    protected bool isSurprised = false;


    // Start is called before the first frame update
    void Start()
    {
        
        fovObjReference = transform.Find("FOV").gameObject;
        if(fovObjReference)
        {
            fovObjReference.GetComponent<FOV>().seePlayerEvt.AddListener(seekPlayer);
            fovObjReference.GetComponent<FOV>().lostPlayerEvt.AddListener(searchPlayer);
        }
        else
        {
            Debug.LogError("FOV Obj reference not found :(");
        }
        
        if(!bodyObjReference) {
            Debug.LogError("Body object with animator not found :(");
        }

        animationManager = bodyObjReference.GetComponent<AnimationManager>();
        if(!animationManager)
        {
            Debug.LogError("animationManager inside body object " + bodyObjReference.name + "not found :( How far astray did we wander from the light? ");
        }

        // when the animation ends, we signal the behavior manager that must change the state
        animationManager.endSurpriseEvt.AddListener(goUnsurprised);

        // when the animation ends, we signal the behavior manager that must change the state
        animationManager.endPivotEvt.AddListener(restorePreviousState);


        animatorReference = bodyObjReference.GetComponent<Animator>();
        if (!animatorReference)
        {
            Debug.LogError("animatorReference object inside body object "+ bodyObjReference.name + "not found :(");
        }

        currentState = ACTIVITY_STATE.IDLE;
        previousState = ACTIVITY_STATE.IDLE;

    }

    protected void RunTowardTarget(Vector3 target)
    {
        if (Vector3.Distance(target, transform.position) <= minSeekingDistance)
        {
            goIdle();
            return;
        }
       
        Vector3 directionBetween = (target - transform.position);
        directionBetween.y *= 0;

        float angle = Vector3.Angle(transform.forward, directionBetween);

        bool onTheRight = Vector3.Dot(transform.right, directionBetween) > 0;



        if (!onTheRight)
        {
            angle *= -1;
        }

        if(Mathf.Abs(angle) > 135)
        {
            animationManager.pivot180();

        } else
        {
            animationManager.goRun();

            transform.Translate(0, 0, Time.deltaTime * runningSpeed);
            transform.Rotate(0, angle * Time.deltaTime * rotationSpeed, 0);
        }

        

    }
    
    protected void DoSeekPlayer()
    {
        if (playerRef)
        {

            if (Vector3.Distance(playerRef.transform.position, transform.position) <= minSeekingDistance)
            {

                goIdle();
                return;
            }

            if (movementTarget)
            {
                movementTarget.transform.position = playerRef.transform.position;
            }
            

            if (playerRef.transform.position.y != 0.5f)
            {
                playerRef.transform.position = new Vector3(playerRef.transform.position.x, 0, playerRef.transform.position.z);
            }

            RunTowardTarget(playerRef.transform.position);           

        }
        else
        {
            Debug.LogError("I should seek player but I have no player ref");
        }
        
    }

    void DoFlee()
    {
        if(fleeingFromRef)
        {
            // if too close, die

            //if (Vector3.Distance(playerRef.transform.position, transform.position) <= minSeekingDistance)
            //{

            //    goIdle();
            //    return;
            //}

            Vector3 directionBetween = (fleeingFromRef.transform.position - transform.position);
            directionBetween.y *= 0;

            Vector3 target = fleeingFromRef.transform.position + directionBetween;
            RunTowardTarget(target);

            float angle = Vector3.Angle(transform.forward, directionBetween);

            bool onTheRight = Vector3.Dot(transform.right, directionBetween) > 0;



            if (!onTheRight)
            {
                angle *= -1;
            }


        }
        else
        {
            Debug.LogError("I should flee but I have no predator to flee from");

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {            
            isSurprised = !isSurprised;
            animationManager.goSurprised();            
        }

        if (Input.GetKeyDown("s"))
        {

            seekPlayer(GameObject.Find("Player"));
            
        }

        if (Input.GetKeyDown("p"))
        {            
            animationManager.pivot180();
        }

        if (Input.GetKeyDown("i"))
        {
            animationManager.goIdle();

        }

        if (Input.GetKeyDown("r"))
        {
            RunTowardTarget(GameObject.Find("Player").transform.position);            
        }

        //Debug.Log("current STatus:" + currentStatus);
        switch (currentState)
        {
            case ACTIVITY_STATE.SEEKING:
                DoSeekPlayer();
                break;

            case ACTIVITY_STATE.FLEEING:
                DoFlee();
                break;

            default:
                break;
        }
        
        
    }

    public void goIdle()
    {
        previousState = currentState;
        currentState = ACTIVITY_STATE.IDLE;
        animationManager.goIdle();
    }

    public void goSurprised()
    {
        previousState = currentState;
        currentState = ACTIVITY_STATE.SURPRISED;
        isSurprised = true;
        
    }

    public void goUnsurprised()
    {
        previousState = currentState;
        currentState = ACTIVITY_STATE.IDLE;
        isSurprised = false;

        Debug.Log("Blimey, I'm not surprised anymore");
    }

    public void seekPlayer(GameObject player)
    {
        lastPlayerKnownPosition = player.transform.position;
        playerRef = player;

        if (Vector3.Distance(playerRef.transform.position, transform.position) > minSeekingDistance)
        {
            previousState = currentState;
            currentState = ACTIVITY_STATE.SEEKING;            
            Debug.Log("seeking player");
        }        
    }

    public void fleeFromTarget(GameObject target)
    {
        fleeingFromRef = target;
        previousState = currentState;
        currentState = ACTIVITY_STATE.FLEEING;
        animationManager.goRun();
        Debug.Log("Fleeing");
    }

    public void searchPlayer(GameObject player)
    {
        return;
        currentState = ACTIVITY_STATE.IDLE;


        animatorReference.SetFloat("speed", 0);
        Debug.Log("searching for player");
    }

    public void restorePreviousState()
    {
        currentState = previousState;
        Debug.Log("Restored state " + currentState);
    }
}
