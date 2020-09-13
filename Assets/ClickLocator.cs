using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickLocator : MonoBehaviour
{
    public GameObject clickObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.layer == 8)
                {
                    clickObj.transform.position = hit.point;
                }
            }

            
            
        }
    }
}
