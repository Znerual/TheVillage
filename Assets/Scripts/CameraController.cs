using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public float cameraSpeed = 0.1f;
	// Use this for initialization
	
	
	
	void LateUpdate () {
	    if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0.0f, 0.0f,cameraSpeed);
        } else if(Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0.0f, 0.0f, -cameraSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-cameraSpeed, 0.0f, 0.0f);
        } else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(cameraSpeed, 0.0f, 0.0f);
        }
	}
}
