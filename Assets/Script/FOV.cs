using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable] public class _UnityEventGObj : UnityEvent<GameObject> { }

public class FOV : MonoBehaviour
{

    [Range(1f, 20f)]
    public float viewDistance = 10;
    public float height = 1.0f;
    public int rayCount = 3;

    [Range(0f, 360f)]
    public float fov = 90.0f;
    
    [Range(0.1f, 1.0f)]
    public float sightCheckInterval = 0.1f;

    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    GameObject player;

    protected Mesh fovMesh;

    // must be public to add a listener from outside
    public _UnityEventGObj seePlayerEvt;
    public _UnityEventGObj lostPlayerEvt;

    protected bool playerInSight = false;

    public Vector3 GetVectorFromAngle(float angle, bool zAxisRotation = false)
    {
        // angle 0 -> 360
        
        if (zAxisRotation)
        {
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            //return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        else
        {
            return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        }
        
    }

    protected void commonSetup()
    {
        fovMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = fovMesh;        
       
    }
    /*
    protected void setupSimpleMesh()
    {
        Mesh fovMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = fovMesh;
        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];

        //triangolo 1
        vertices[0] = Vector3.zero + new Vector3(0, height, 0);
        
        Vector3 left = new Vector3();
        left = transform.position + new Vector3(-viewDistance, height, viewDistance);
        

        Vector3 right = new Vector3();
        right = transform.position + new Vector3(viewDistance, height, viewDistance);

       
        vertices[1] = left;
        vertices[2] = right;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        fovMesh.vertices = vertices;
        fovMesh.uv = uv;
        fovMesh.triangles = triangles;
    }


    protected void setupAdvancedMesh()
    {

        commonSetup();
        float angleIncrease = fov / rayCount;

        Vector3 origin = Vector3.zero + new Vector3(0, height, 0); ;


        // 1 origin + ray count + 1 but i dont know why

        vertices = new Vector3[rayCount + 1 + 1];
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];

        vertices[0] = origin;

        float currentAngle = 0f;

        int vertexIdx = 1;
        int triangleIdx = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 v = origin + viewDistance * GetVectorFromAngle(-fov / 2 + currentAngle, true);
            
            vertices[vertexIdx] = v;

            if (i > 0)
            {
                triangles[triangleIdx + 0] = 0; // always the origin vertex coords
                triangles[triangleIdx + 1] = vertexIdx - 1;
                triangles[triangleIdx + 2] = vertexIdx;

                triangleIdx += 3;
            }


            currentAngle += angleIncrease;
            vertexIdx++;
        }


        fovMesh.vertices = vertices;
        fovMesh.uv = uv;
        fovMesh.triangles = triangles;
        GetComponent<MeshCollider>().sharedMesh = fovMesh;


       

    }

    */  

    public bool inFOV( Transform target, float maxAngle, float radius)
    {
        Collider[] overlaps = new Collider[5];
        int cnt = Physics.OverlapSphereNonAlloc(transform.position, radius, overlaps);
        

        for (int i = 0; i < cnt; i++)
        {
            if (overlaps[i] != null)
            {

                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - transform.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle( transform.forward, directionBetween);                    

                    if (angle < maxAngle/2)
                    {
                        Ray ray = new Ray(transform.position, target.position - transform.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, radius))
                        {
                            if (hit.transform == target)
                            {
                                return true;
                            }
                        }

                    }
                 }
            }
        }

        return false;
    }


    // Start is called before the first frame update
    void Start()
    {

        //setupAdvancedMesh();
        player = GameObject.FindGameObjectWithTag("Player");

    }

    float lastSightCheck = 0;
    
    protected bool canSeePlayer()
    {
        
        
        return true;

    }

    // Update is called once per frame
    void Update()
    {
       if ((lastSightCheck == 0 ) || (Time.time - lastSightCheck > sightCheckInterval))
       {            
          lastSightCheck = Time.time;

          if(inFOV(player.transform, fov, viewDistance))
          {
                // first time i see it
                if(!playerInSight)
                {
                    seePlayerEvt.Invoke(player);
                }
                playerInSight = true;
          } else
            {
                //i was already seeing it
                if (playerInSight)
                {
                    lostPlayerEvt.Invoke(player);
                }
                playerInSight = false;
            }



          
       }
    }


}
