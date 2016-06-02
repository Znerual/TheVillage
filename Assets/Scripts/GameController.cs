using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
public class GameController : MonoBehaviour
{
    public GameObject Character;
    public GameObject ResourcenLagerstelle;
    public GameObject UI_Character;
    public GameObject UI_Building;
    public GameObject Sammelstelle;
    public Text FoodCounter, WoodCounter, StoneCounter, IronCounter;
    

    public float ROTATE_FACTOR = 15.0f;
    public int CHARACTER_FOOD_COST = 50;
    public float START_SPAWN_TIME = 20f;
    public float SPAWN_TIME_INCERMENT = 2f;
    public int[] START_RESOURCES = new int[4];

    private float spawnTime;
    private static GameController instance;
    private GameObject buildObject;
    private List<MonoBehaviour> subscribedScripts = new List<MonoBehaviour>();
    private int[] resources = new int[4];// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    private GameObject selectedObject;
   
    

   //charactere ins gebäude einfügen und nachher wieder ausquartieren lassen (gameObject einfach setActive(false))
    //hauscontroller nach dem bauen entfernen und einen 2. controller (gebäudespez.) verwenden
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
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        spawnTime = START_SPAWN_TIME;
        resources = START_RESOURCES;
        updateResourcesCount();
        Invoke("spawnPeople", spawnTime++);
    }
    public void subscribeScript(MonoBehaviour pScript) { subscribedScripts.Add(pScript); }
    public void deSubscribeScript(MonoBehaviour pScript) { if (subscribedScripts.Contains(pScript)) subscribedScripts.Remove(pScript); }
    public void placeObject(GameObject GO)
    {


        buildObject = (GameObject)Instantiate(GO, getMousePosition().point, transform.rotation); //fügt das Object in die Scene ein, dieses muss im Start() sich im GameController subscribeScribt einschreiben


    }
    public RaycastHit getMousePosition(bool showBuildings = false)
    {
        RaycastHit temp;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask;
        if (showBuildings)
        {
            int layerDef = 1 << 0;
            int layerBui = 1 << 8;
            int layerUI = 1 << 5;
            mask = layerDef | layerBui | layerUI;

        }
        else
        {
            mask = (1 << 0) | (1 << 5);

        }

        Physics.Raycast(ray, out temp, 100, mask);
        return temp;

    }
    private void spawnPeople()
    {
        if (resources[3] >= CHARACTER_FOOD_COST)
        {
           resources[3] -= CHARACTER_FOOD_COST;
           updateResourcesCount();
           GameObject newChar = (GameObject) Instantiate(Character, ResourcenLagerstelle.transform.position, new Quaternion());            
           NPCController NPCScript= newChar.GetComponent<NPCController>();
           NPCScript.setTargetPos(Sammelstelle.transform.position);
            
        }
        spawnTime += SPAWN_TIME_INCERMENT;
        Invoke("spawnPeople", spawnTime);
    }
    void Update()
    {



        if (buildObject == null)
        {
            
            if (Input.mousePosition.x < Screen.width - 200 && Input.mousePosition.x > 120)
            {
                if (Input.GetMouseButtonDown(0))
                {


                    RaycastHit mp = getMousePosition(true);
                    Debug.Log("Klicked : " + mp.transform.gameObject.tag);
                    //  Debug.Log("LMB D - " + mp.transform.gameObject.name);
                    if (selectedObject != null)
                    {
                        findScript(selectedObject).Invoke("not_selected", 0);

                    }
                    switch (mp.transform.gameObject.tag)
                    {

                        case "Character":
                            selectedObject = mp.transform.gameObject;
                            findScript(selectedObject).Invoke("selected", 0);
                            UI_Building.SetActive(false);
                            UI_Character.SetActive(true);
                            Debug.Log("Selected an character");
                            break;
                        case "House":
                        case "Store":
                            selectedObject = mp.transform.gameObject;
                            findScript(selectedObject).Invoke("selected", 0);
                            Debug.Log("Gebäude gewählt");
                            UI_Character.SetActive(false);
                            UI_Building.SetActive(true);
                            break;
                        case "Ground":
                            if (selectedObject != null)
                            {
                                findScript(selectedObject).Invoke("not_selected", 0);
                                UI_Building.SetActive(false);
                                UI_Character.SetActive(false);
                            }
                            break;
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
                    else
                    {
                        UI_Building.SetActive(false);
                        UI_Character.SetActive(false);
                    }


                }

            }

        }
        else //falls ein Objekt in der Variable buildObject ist = ein Gebäude wird plaziert
        {
            if (Input.mousePosition.x < Screen.width - 200 && Input.mousePosition.x > 120)
            {
                RaycastHit mp = getMousePosition(false);
                if (Input.GetMouseButtonDown(0))
                {
                    if (checkBuildable(buildObject))
                    {

                        MonoBehaviour pScript = findScript(buildObject);
                        if (pScript != null)
                        {
                            pScript.Invoke("placed", 0.0f); //Ruft die placed Methode im Objekt auf
                            foreach (MonoBehaviour _script in subscribedScripts)
                            {
                                if (_script.gameObject == selectedObject)
                                {
                                    _script.Invoke("UpdateTarget", 0.0f);
                                }

                            }
                            buildObject = null;
                        }

                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    MonoBehaviour pScript = findScript(buildObject);
                    if (pScript != null)
                    {
                        pScript.Invoke("abort", 0.0f); //bricht das plazieren ab, das Objekt muss sich noch selber desubscriben
                        buildObject = null;
                    }
                }
                else
                {
                    float scroll = Input.GetAxis("MouseScrollWheel");
                    if (scroll > 0.01f || scroll < -0.01f)
                    {
                        buildObject.transform.Rotate(Vector3.up * scroll * ROTATE_FACTOR, Space.World);

                    }


                    buildObject.transform.position = mp.point;

                }

            }
        }


    }
    public void addResources(float[] res)
    {
        for (int i = 0; i < 4; i++)
        {
            resources[i] += (int)res[i];

        }
        updateResourcesCount();

    }
    public bool hasEnoughResources(int[] res)
    {
        for (int i = 0; i < 4; i++)
        {
            if (resources[i] < res[i])
            {
                return false;
            }
        }
        return true;
    }
    public void subtractResources(int[] res)
    {
        for (int i = 0; i < 4; i++)
        {
            resources[i] -= res[i];
        }
        updateResourcesCount();
    }
    public int getRandomIQ()
    {
        //über gaußsche normalverteilung verteilen
        return Gauss(100, 15f);
    }
    private int Gauss(int mittelwert, float standartAbweichung)
    {
        float u1, u2;             
        u1 = Random.value;
        u2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mittelwert + (int)(standartAbweichung * randStdNormal);
    }
    public string getRandomName(bool male)
    {
        if (male)
        {
            string[] namen = File.ReadAllLines(Application.dataPath + "/Other/maleName.txt");
            int randi = Mathf.FloorToInt(Random.Range(0f, (float)namen.Length));
            return namen[randi];
        } else
        {
            string[] namen = File.ReadAllLines(Application.dataPath + "/Other/femaleName.txt");
            int randi = Mathf.FloorToInt(Random.Range(0f, (float)namen.Length));
            return namen[randi];
        }
        
    }
    public bool isRandomMale()
    {
       
      return Random.Range(0f,1f) >= 0.5 ? false : true;
       
    }
    public int getLifeExpectance(bool male, int IQ)
    {
        float IQ_Factor = IQ / 30;
        return male ? Gauss(75, IQ_Factor) : Gauss(80, IQ_Factor);
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
    private void updateResourcesCount()// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    {
        WoodCounter.text = resources[0].ToString();
        IronCounter.text = resources[1].ToString();
        StoneCounter.text = resources[2].ToString();
        FoodCounter.text = resources[3].ToString();

    }
    bool checkBuildable(GameObject building)
    {
        if (!building.GetComponent<HausController>().isCollided)
        {
            return true;
        }
        return false;
        //auf höhe und steigung des Bodens überprüfen und ob wasser oder andere res im weg sind

    }


}
