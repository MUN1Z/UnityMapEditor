using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

    Transform cameraMover;
    
    public float cameraSpeed = 100;
    bool rotating;

	void Start () {
        cameraMover = this.transform;
    }
    
    void FixedUpdate () {
        HandleCameraMovement();
	}

    void HandleCameraMovement()
    {
        //float hor = Input.GetAxis("Horizontal");
        //float vert = Input.GetAxis("Vertical");

        //Vector3 right = Camera.main.transform.right * hor * cameraSpeed;
        //Vector3 forward = Camera.main.transform.forward * vert * cameraSpeed;
        //forward.y = 0;

        //Vector3 newPos = right + forward;

        //cameraMover.transform.position += newPos;
        
        if (Input.GetKey(KeyCode.Q))
        {
            if (!rotating)
            {
                Vector3 rot = transform.eulerAngles;

                rot.y += 90;

                StartCoroutine(RotateAroundSelft(Quaternion.Euler(rot)));
                
                rotating = true;
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (!rotating)
            {
                Vector3 rot = transform.eulerAngles;

                rot.y -= 90;

                StartCoroutine(RotateAroundSelft(Quaternion.Euler(rot)));

                rotating = true;
            }
        }
    }
    
    IEnumerator RotateAroundSelft(Quaternion targetRot)
    {
        Quaternion rot = transform.rotation;
        Quaternion tRot = targetRot;
        float t = 0;

        while( t < 1)
        {
            t += Time.deltaTime * cameraSpeed;

            transform.rotation = Quaternion.Slerp(rot, tRot, t);
            
            yield return null;
        }

        rotating = false;
    }
}
