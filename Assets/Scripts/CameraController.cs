using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float MOVEMENT_SPEED = 0.1f;
    public float MAX_CAM_HEIGHT = 50.0f;
    public float MIN_CAM_HEIGHT = 20.0f;
    public float ZOOM_FACTOR = 2.0f;
    public float ROTATE_FACTOR = 20f;
    public float MAX_ROTATION = 45f;
    public float MIN_ROTATION = -45f;
    public bool AUTO_MOVE_CAM = true;
    public float MOVE_EDGE_DISTANCE = 200f;
    // Use this for initialization
   
    private Vector3 targetScreenPos,targetPos;
    private Vector3 curRot;
    private Vector3 targetRot;
  
   void Awake()
    {
        curRot = transform.localEulerAngles;
       // curPos = transform.position;
        targetRot = curRot;
        targetPos = transform.position;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            targetRot.y += Time.deltaTime * ROTATE_FACTOR * -1;
            
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetRot.y += Time.deltaTime * ROTATE_FACTOR ;
            
        }
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += new Vector3(0.0f, 0.0f, MOVEMENT_SPEED * Time.deltaTime );
            targetRot.x += Time.deltaTime * ROTATE_FACTOR * -1;
            targetRot.x = Mathf.Clamp(targetRot.x, MIN_ROTATION, MAX_ROTATION);
            


        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.Rotate(Vector3.right * Time.deltaTime * ROTATE_FACTOR, Space.World);
            targetRot.x += Time.deltaTime * ROTATE_FACTOR;
            targetRot.x = Mathf.Clamp(targetRot.x, MIN_ROTATION, MAX_ROTATION);
            
        }
        curRot = new Vector3(Mathf.LerpAngle(curRot.x, targetRot.x, Time.deltaTime * 2),
             Mathf.LerpAngle(curRot.y, targetRot.y, Time.deltaTime * 2),
             Mathf.LerpAngle(curRot.z, targetRot.z, Time.deltaTime * 2));

        transform.localEulerAngles = curRot;
        if (Input.GetButtonDown("Fire3"))
        {
            targetScreenPos = new Vector3 (Input.mousePosition.x,targetPos.y, Input.mousePosition.y);
        }
        if (Input.GetButton("Fire3"))
        {
            if (AUTO_MOVE_CAM == false)
            {
                Vector3 current_pos = new Vector3(Input.mousePosition.x, transform.position.y, Input.mousePosition.y);
                Vector3 direction = targetScreenPos - current_pos;
                float speed = direction.magnitude / 40;
                direction.Normalize();

                targetPos = transform.position + direction * -1 * speed;
            } 
           // targetPos = current_pos;
        }

        if (AUTO_MOVE_CAM)
        {
            
                Vector3 screenCenter = new Vector3(Screen.width / 2, 0f, Screen.height / 2);
                Vector3 current_pos = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y);
                if (Vector3.Distance(screenCenter, current_pos) > MOVE_EDGE_DISTANCE && ((Input.mousePosition.x < Screen.width - 200) || (!GameController.Instance.UI_Building.activeSelf && !GameController.Instance.UI_Character.activeSelf)) && Input.mousePosition.x > 120)
                {
                   
                    Vector3 direction = current_pos - screenCenter;
                    direction = Quaternion.AngleAxis(curRot.y,Vector3.up) * direction;
                    float length = direction.magnitude - 150;
                    float speed = (length * length) / 45000;
                    targetPos = transform.position + direction.normalized * speed;
                        //move
                   
                }
            
        }
        float scroll = Input.GetAxis("MouseScrollWheel");
        if (scroll > 0.01f)
        {
            if (targetPos.y < MAX_CAM_HEIGHT)
            {
                targetPos += new Vector3(0.0f, scroll * ZOOM_FACTOR, 0.0f);
            }
        }
        else if (scroll < -0.01f)
        {
            if (targetPos.y > MIN_CAM_HEIGHT)
            {
                targetPos += new Vector3(0.0f, scroll * ZOOM_FACTOR, 0.0f);
            }
        }
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPos.x, Time.deltaTime * MOVEMENT_SPEED),
            Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * MOVEMENT_SPEED),
            Mathf.Lerp(transform.position.z, targetPos.z, Time.deltaTime * MOVEMENT_SPEED));
    }
    public void jumpToPosition(Vector3 position)
    {
        targetPos.x = position.x;
        targetPos.z = position.z;
        transform.position = targetPos;
    }
}
