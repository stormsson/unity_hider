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

    enum ACTIVITY_STATE {
        IDLE,
        SURPRISED,
        SEEKING,
        SEARCHING
    };

    ACTIVITY_STATE currentStatus;

    
    float runningSpeed = 3f;
    float rotationSpeed = 2f;
    public float minSeekingDistance = 1f;


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


        animatorReference = bodyObjReference.GetComponent<Animator>();
        if (!animatorReference)
        {
            Debug.LogError("animatorReference object inside body object "+ bodyObjReference.name + "not found :(");
        }
        currentStatus = ACTIVITY_STATE.IDLE;



        
    }

    protected void doSeekPlayer()
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

            

            Vector3 directionBetween = (playerRef.transform.position - transform.position);
            directionBetween.y *= 0;

            float angle = Vector3.Angle(transform.forward, directionBetween);            

            bool onTheRight = Vector3.Dot(transform.right, directionBetween) > 0;
            


            if (!onTheRight)
            {
                angle *= -1;
            }
            transform.Translate(0, 0, Time.deltaTime * runningSpeed );
            transform.Rotate(0, angle * Time.deltaTime * rotationSpeed, 0);
           

        }
        else
        {
            Debug.LogError("no player ref");
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

        //Debug.Log("current STatus:" + currentStatus);
        switch (currentStatus)
        {
            case ACTIVITY_STATE.SEEKING:
                doSeekPlayer();
                break;

            default:
                break;
        }
        
        
    }

    public void goIdle()
    {
        currentStatus = ACTIVITY_STATE.IDLE;
        animationManager.goIdle();
    }

    public void goSurprised()
    {
        currentStatus = ACTIVITY_STATE.SURPRISED;
        isSurprised = true;
        
    }

    public void goUnsurprised()
    {
        currentStatus = ACTIVITY_STATE.IDLE;
        isSurprised = false;

        Debug.Log("Blimey, I'm not surprised anymore");
    }

    public void seekPlayer(GameObject player)
    {
        lastPlayerKnownPosition = player.transform.position;
        playerRef = player;

        if (Vector3.Distance(playerRef.transform.position, transform.position) > minSeekingDistance)
        {
            currentStatus = ACTIVITY_STATE.SEEKING;
            animationManager.goRun();
            Debug.Log("seeking player");
        }        
    }

    public void searchPlayer(GameObject player)
    {
        return;
        currentStatus = ACTIVITY_STATE.IDLE;


        animatorReference.SetFloat("speed", 0);
        Debug.Log("searching for player");
    }
}
