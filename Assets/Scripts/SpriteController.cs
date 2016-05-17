using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SpriteController : MonoBehaviour {
    
    NavMeshAgent nAgent;
	// Use this for initialization
	void Start () {
         GameController.Instance.subscribeScript(this);
        nAgent = GetComponent<NavMeshAgent>();
        GameController.Instance.subscribePC(transform.gameObject.GetInstanceID());
        Debug.Log("Script angemeldet");
    }
	void OnDestroy()
    {
        GameController.Instance.deSubscribeScript(this);
        GameController.Instance.deSubscribePC(transform.gameObject.GetInstanceID());
        Debug.Log("Script abgemeldet");
    }
    void UpdateTarget()
    {
        if (nAgent.destination != transform.GetChild(0).position)
        {
            nAgent.destination = transform.GetChild(0).position;
        }
       
    }
   
    
}
