using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    public Camera camera;
    private static GameController instance;
    private List<MonoBehaviour> subscribedScripts = new List<MonoBehaviour>();
    private List<int> subscribedPC = new List<int>();
    private GameObject selectedObject;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();
#if UNITY_EDITOR
                if (FindObjectsOfType<GameController>().Length > 1)
                {
                    Debug.LogError("More than one GameController in Game");
                }
#endif
            }
            return instance;
        }
    }
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	public void subscribeScript(MonoBehaviour pScript) { subscribedScripts.Add(pScript); }
    public void deSubscribeScript(MonoBehaviour pScript) { if (subscribedScripts.Contains(pScript)) subscribedScripts.Remove(pScript);  }
    public void subscribePC(int pPCID) { subscribedPC.Add(pPCID); }
    public void deSubscribePC(int pPCID) { if (subscribedPC.Contains(pPCID)) subscribedPC.Remove(pPCID); }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (subscribedPC.Contains(hit.transform.gameObject.GetInstanceID()))
                {
                    Debug.Log("Selected Game Object set " + hit.transform.gameObject.GetInstanceID());
                    selectedObject = hit.transform.gameObject;
                }
            }

        } else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (selectedObject != null) 
                {
                    Debug.Log("Moved Selected Game Object " + selectedObject.GetInstanceID());
                    selectedObject.transform.GetChild(0).position = hit.point;
                    foreach (MonoBehaviour _script in subscribedScripts)
                    {
                        if (_script.gameObject == selectedObject)
                        {
                            _script.Invoke("UpdateTarget", 0.0f);
                        }
                        
                    }
                }
                
            }
        }
    }
  
}
