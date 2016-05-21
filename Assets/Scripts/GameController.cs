using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    public Camera Scamera;
    public GameObject ResourcenLagerstelle;
    private static GameController instance;
    private GameObject buildObject;
    private List<MonoBehaviour> subscribedScripts = new List<MonoBehaviour>();
    private int[] resources = new int[4];// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    private GameObject selectedObject;
   // private float rotation;


        //Button der UI muss interagierbarer werden, dh ausgeblendet(ausgegraut) falls zu wenig res vorhanden und klickable und normal falls der bau möglich ist
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
    public void placeObject(GameObject GO)
    {
        
     
           buildObject = (GameObject) Instantiate(GO, getMousePosition().point, transform.rotation); //fügt das Object in die Scene ein, dieses muss im Start() sich im GameController subscribeScribt einschreiben
      
            
    }
    public RaycastHit getMousePosition()
    {
        RaycastHit temp;
        Ray ray = Scamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out temp, 100);
       
        return temp;
     
    }
    void Update()
    {
        
       
        
        if (buildObject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {

               
                RaycastHit mp = getMousePosition();
              //  Debug.Log("LMB D - " + mp.transform.gameObject.name);
                if (mp.transform.gameObject.CompareTag("Character"))
                    {
                    if (selectedObject != null)
                    {
                        findScript(selectedObject).Invoke("not_selected", 0);
                        
                    }
                   
                    selectedObject = mp.transform.gameObject;
                    findScript(selectedObject).Invoke("selected", 0);
                    Debug.Log("Selected an character");
                }
             

            }
            else if (Input.GetMouseButtonDown(1))
            {

               
                    if (selectedObject != null)
                    {
                        Debug.Log("Moved Selected Game Object " + selectedObject.GetInstanceID());
                       // selectedObject.transform.GetChild(0).position = MousePositionHit.point;
                        foreach (MonoBehaviour _script in subscribedScripts)
                        {
                            if (_script.gameObject == selectedObject)
                            {
                                _script.Invoke("UpdateTarget", 0.0f);
                            }

                        }
                    }

                
            }
        } else //falls ein Objekt in der Variable buildObject ist = ein Gebäude wird plaziert
        {
           
            if (Input.GetMouseButtonDown(0))
            {
                if (checkBuildable(buildObject))
                {
                   
                    MonoBehaviour pScript = findScript(buildObject);
                    if (pScript != null)
                    {
                        pScript.Invoke("placed", 0.0f); //Ruft die placed Methode im Objekt auf
                        buildObject = null;
                    }

                }
            } else if (Input.GetMouseButtonDown(1))
            {
                MonoBehaviour pScript = findScript(buildObject);
                if (pScript != null)
                {
                    pScript.Invoke("abort", 0.0f); //bricht das plazieren ab, das Objekt muss sich noch selber desubscriben
                    buildObject = null;
                }
            } else
            {
                
                
                    buildObject.transform.position = getMousePosition().point;
                
            }
                
        }
       
    }
    public void addResources(float[] res)
    {
        for (int i = 0; i < 4; i++)
        {
            resources[i] += (int)res[i];

        }
        
        
    }
    MonoBehaviour findScript(GameObject targetGO)
    {
        foreach (MonoBehaviour _script in subscribedScripts)
        {
            if (_script.gameObject == targetGO)
            {
                return _script;
            }

        }
        return null;
    }
    bool checkBuildable(GameObject building)
    {
        //auf höhe und steigung des Bodens überprüfen und ob wasser oder andere res im weg sind
        return true;
    }
 
}
