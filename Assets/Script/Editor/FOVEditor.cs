using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FOV))]
public class FOVEditor : Editor
{


    private void OnSceneGUI()
    {
        FOV fow = (FOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewDistance) ;

        //Vector3 viewAngleA =  fow.GetVectorFromAngle( -fow.fov / 2, true);
        //Vector3 viewAngleB = fow.GetVectorFromAngle( +fow.fov / 2, true);

        Vector3 leftFOVLimit = Quaternion.AngleAxis(-fow.fov / 2, fow.transform.up) * fow.transform.forward * fow.viewDistance;
        Vector3 rightFOVLimit = Quaternion.AngleAxis(fow.fov / 2, fow.transform.up) * fow.transform.forward * fow.viewDistance;




        Handles.color = Color.blue;
        Handles.DrawLine(fow.transform.position, fow.transform.position + leftFOVLimit );
        Handles.color = Color.white;
        Handles.DrawLine(fow.transform.position, fow.transform.position + rightFOVLimit );

        Handles.color = Color.red;
        Handles.DrawLine(fow.transform.position, fow.transform.position + fow.transform.forward * fow.viewDistance);
    }
}
