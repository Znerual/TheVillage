using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    float[] inventory = new float[4]; // 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    NavMeshAgent nAgent;
    private const float RATE_FACTOR = 60f;
    private float[] ColRate = { 0.1f, 0.1f, 0.1f, 0.1f };
    private float buildingRate = 1f;
    private bool walking;
    private string goal, goal2;
    private Vector3 Vgoal2;
    private GameObject goalObj, goalObj2;
    private bool building;

    private int age;
    private int max_invent = 10;
    private string CharacterName;
    private string status;
    private int IQ;
    private bool male;
    private int lifeExpectance;
    private Text CTName, CTMale, CTAge, CTIQ, CTStatus, CTFood, CTWood, CTStone, CTIron;
    public void Awake()
    {
        GameController.Instance.subscribeScript(this);
        nAgent = GetComponent<NavMeshAgent>();
        Debug.Log("Script angemeldet: ");

        IQ = GameController.Instance.getRandomIQ();
        status = "Gesund";
        male = GameController.Instance.isRandomMale();
        CharacterName = GameController.Instance.getRandomName(male);
        lifeExpectance = GameController.Instance.getLifeExpectance(male, IQ);
        age = 14;

        GameObject canvas = GameController.Instance.UI_Character;
        CTName = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        CTMale = canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        CTAge = canvas.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        CTIQ = canvas.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        CTStatus = canvas.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>();
        CTFood = canvas.transform.GetChild(6).transform.GetChild(0).GetComponent<Text>();
        CTWood = canvas.transform.GetChild(7).transform.GetChild(0).GetComponent<Text>();
        CTStone = canvas.transform.GetChild(8).transform.GetChild(0).GetComponent<Text>();
        CTIron = canvas.transform.GetChild(9).transform.GetChild(0).GetComponent<Text>();
    }

    public struct resource
    {
        int id;
        public float amount;
    }
    void Start()
    {

        InvokeRepeating("get_older", 1, 20); // ruft nach 1 sek alle 20 sek die funktion get_older auf
    }
    void OnDestroy()
    {
        GameController.Instance.deSubscribeScript(this);
        Debug.Log("Script abgemeldet");
    }
    void UpdateTarget()
    {
        Vector3 targetPos = GameController.Instance.getMousePosition().point;
        if (Vector3.Distance(nAgent.destination, targetPos) > 1.0f) //überprüft ob das Objekt sich wirklich bewegen muss (distanz zwischen ziel und dem klick punkt)
        {
            walking = true;
            building = false;
            goal = GameController.Instance.getMousePosition(true).transform.gameObject.tag;
            goalObj = GameController.Instance.getMousePosition(true).transform.gameObject;
            if (goal != "Ground" && goal != null && goal != "Untagged")
            {
                nAgent.stoppingDistance = 2f;
                nAgent.destination = goalObj.transform.position;
            }
            else
            {
                nAgent.destination = targetPos;
                nAgent.stoppingDistance = 0;
            }

        }




    }
    public void setTargetPos(Vector3 targetPos)
    {
        if (Vector3.Distance(nAgent.destination, targetPos) > 1.0f)
        {
            goal = "Ground";
            nAgent.stoppingDistance = 0f;
            nAgent.destination = targetPos;
            walking = true;
            building = false;
        }
    }
    void Update()
    {
        if (walking)
        {
            if (this.reachedTarget(nAgent))
            {
                walking = false;
            }
        }
        else
        {
            if (goal != "Ground" && goal != null && goal != "Untagged" && gameObject.activeSelf == true) //überprüft ob das Objekt das beim setzten des Bewegbefehls den Tag Ground hat, falls nicht...
            {

                ResourceController ResourcenScript;
                HausController HausScript;
                switch (goal)
                {
                    
                    case "Wood":
                        ResourcenScript = goalObj.GetComponent<ResourceController>();
                        //Debug.Log("Sammle Holz");
                        inventory[0] += ResourcenScript.getResources(ColRate[0] * Time.deltaTime * RATE_FACTOR);
                        break;
                    case "Stone":
                        //Debug.Log("Sammle Stein");
                        inventory[2] += ColRate[2] * Time.deltaTime * RATE_FACTOR;
                        break;
                    case "Iron":
                        //Debug.Log("Sammle Eisen");
                        inventory[1] += ColRate[1] * Time.deltaTime * RATE_FACTOR;
                        break;
                    case "Food":
                        ResourcenScript = goalObj.GetComponent<ResourceController>();
                        inventory[3] += ResourcenScript.getResources(ColRate[3] * Time.deltaTime * RATE_FACTOR);
                        break;
                    case "House":
                       HausScript = goalObj.GetComponent<HausController>();
                        if (HausScript.status < 10)
                        {
                            HausScript.building(buildingRate);
                            building = true;
                        }
                        else if (!building)
                        {
                            //do nothing - maybe increase SPAWN_RATE
                            //let people move into the house:
                            goal = null;
                            goalObj = null;
                            Vgoal2 = Vector3.zero;
                            HausScript.MoveInside(gameObject);
                        }

                        break;
                    case "Store":
                        HausScript = goalObj.GetComponent<HausController>();
                        if (HausScript.status < 10)
                        {
                            HausScript.building(buildingRate);
                            building = true;
                        }
                        else if(!building) // falls building true ist und der status == 10 dann bedeutet das, dass die bauarbeiten fertig sind, 
                        {
                            GameController.Instance.addResources(inventory);
                            inventory = new float[4];
                            goal = goal2;
                            goal2 = null;
                            if (goalObj2 == null)
                            {
                                nAgent.destination = Vgoal2;
                            }
                            else
                            {
                                nAgent.destination = goalObj2.transform.position;
                                goalObj = goalObj2;
                                goalObj2 = null;
                            }

                            walking = true;
                            nAgent.stoppingDistance = 2f;
                            updateUI();
                        }

                        break;
                    case "Mill":
                        HausScript = goalObj.GetComponent<HausController>();
                        if (HausScript.status < 10)
                        {
                            HausScript.building(buildingRate);
                            building = true;
                        }
                        else if (!building)
                        {
                            //increase productivity
                        }

                        break;
                    case "Field":
                        HausScript = goalObj.GetComponent<HausController>();
                        if (HausScript.status < 10)
                        {
                            HausScript.building(buildingRate);
                            building = true;
                        }
                        else if (!building)
                        {
                            //increase productivity
                        }
                        break;
                    case "School":
                    case "Smelter":
                    case "Smith":
                        HausScript = goalObj.GetComponent<HausController>();
                        if (HausScript.status < 10)
                        {
                            HausScript.building(buildingRate);
                            building = true;
                        }
                        else if (!building)
                        {
                            //learn something, let the player select what to learn
                        }
                        break;
                    default:
                        // Debug.LogError("Falscher Tag gestetzt");
                        break;
                   
                }
                if (transform.FindChild("highlight").gameObject.activeSelf)
                {
                    updateUI();
                }

                if (inventarFull())
                {
                    //bring the res back
                    //save the old target
                    goal2 = goal;
                    goal = "Store";
                    if (goalObj != null)
                    {
                        goalObj2 = goalObj;
                    } else
                    {
                        goalObj2 = null;
                        Vgoal2 = nAgent.destination;
                    }
                     
                    //set the new target
                    float minDis = -1f;
                    GameObject selectedStore = null;
                    foreach (GameObject store in GameObject.FindGameObjectsWithTag("Store"))
                    {
                        HausController tmpHC = store.GetComponent<HausController>();
                        if (tmpHC.status == 10)
                        {
                            if (selectedStore == null)
                            {
                                selectedStore = store;
                                minDis = Vector3.Distance(store.transform.position, transform.position);
                            }
                            else
                            {
                                float dis = Vector3.Distance(store.transform.position, transform.position);
                                if (dis < minDis)
                                {
                                    selectedStore = store;
                                    minDis = dis;
                                }
                            }
                        }


                    }
                    if (selectedStore == null)
                    {
                        selectedStore = GameController.Instance.ResourcenLagerstelle;
                    }
                    nAgent.destination = selectedStore.transform.position;
                    goalObj = selectedStore;
                    //mopve
                    walking = true;
                    nAgent.stoppingDistance = 2f;
                }
            }


        }
    }
   
    void get_older()
    {
        age++;
        if (lifeExpectance - age < 5 || age > lifeExpectance || status == "Krank")
        {
            float schwellwert = 1f / ((lifeExpectance - age) / lifeExpectance);
            if (schwellwert > Random.value)
            {
                die();
            }
        }
        //maybe build a die checker in
    }
    void die()
    {
        Destroy(gameObject);
    }
    void not_selected()
    {
        transform.FindChild("highlight").gameObject.SetActive(false);
    }
    void selected()
    {
        transform.FindChild("highlight").gameObject.SetActive(true);
        updateUI();
    }
    bool reachedTarget(NavMeshAgent mNavMeshAgent)
    {
        /*if (!mNavMeshAgent.pathPending)
        {
            if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
            {
                if (!mNavMeshAgent.hasPath || mNavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
        */
        if (Vector3.Distance(transform.position, nAgent.destination) < mNavMeshAgent.stoppingDistance)
        {
            return true;
        }
        return false;
    }
    bool inventarFull()
    {
        int sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += (int)inventory[i];
        }
        if (sum > max_invent)
        {
            return true;
        }
        return false;
    }
    void updateUI()
    {

        CTName.text = CharacterName;
        CTMale.text = male ? "Männlich" : "Weiblich";
        CTAge.text = age.ToString();
        CTIQ.text = IQ.ToString();
        CTStatus.text = "Gesund";
        CTFood.text = ((int)inventory[3]).ToString();
        CTWood.text = ((int)inventory[0]).ToString();
        CTStone.text = ((int)inventory[2]).ToString();
        CTIron.text = ((int)inventory[1]).ToString();
    }
}
