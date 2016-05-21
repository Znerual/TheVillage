using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour {
    float[] inventory = new float[4]; // 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    NavMeshAgent nAgent;
    
    private float[] ColRate = { 0.1f, 0.1f, 0.1f, 0.1f };
    private bool walking;
    private string goal, goal2;
    private Vector3 Vgoal2;
    private int age = 0;
    private int max_invent = 10;
    

   public struct resource
    {
        int id;
        public float amount;
    }
    void Start () {
        GameController.Instance.subscribeScript(this);
        nAgent = GetComponent<NavMeshAgent>();
        Debug.Log("Script angemeldet: ");
        InvokeRepeating("get_older", 1, 2); // ruft nach 1 sek alle 2 sek die funktion get_older auf
    }
	void OnDestroy()
    {
        GameController.Instance.deSubscribeScript(this);
        Debug.Log("Script abgemeldet");
    }
    void UpdateTarget()
    {
       Vector3 targetPos = GameController.Instance.getMousePosition().point;
       if ((transform.GetChild(0).position - targetPos).magnitude > 0.1f) //überprüft ob das Objekt sich wirklich bewegen muss (distanz zwischen ziel und dem klick punkt)
        {
            transform.GetChild(0).position = targetPos;
            
            walking = true;
            goal = GameController.Instance.getMousePosition().transform.gameObject.tag;
            if (goal != "Ground" && goal != null && goal != "Untagged")
            {
                nAgent.stoppingDistance = 2f;
                nAgent.destination = GameController.Instance.getMousePosition().transform.position;
               
            } else
            {
                nAgent.stoppingDistance = 0;
                nAgent.destination = transform.GetChild(0).position;
            }
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
        } else
        {
            if (goal != "Ground" && goal != null && goal != "Untagged") //überprüft ob das Objekt das beim setzten des Bewegbefehls den Tag Ground hat, falls nicht...
            {
                if (goal == GameController.Instance.ResourcenLagerstelle.tag)
                {
                    GameController.Instance.addResources(inventory);
                    inventory = new float[4];
                    goal = goal2;
                    goal2 = null;
                    transform.GetChild(0).position = Vgoal2;
                    nAgent.destination = Vgoal2;
                    walking = true;                
                    nAgent.stoppingDistance = 2f;
                   
                }
                else
                {
                    if (inventarFull())
                    {
                        //bring the res back
                        goal2 = goal;
                        goal = GameController.Instance.ResourcenLagerstelle.tag;
                        Vgoal2 = nAgent.destination;
                        transform.GetChild(0).position = GameController.Instance.ResourcenLagerstelle.transform.position;
                        nAgent.destination = transform.GetChild(0).position;
                        walking = true;
                        nAgent.stoppingDistance = 2f;
                    }
                    else
                    {

                        switch (goal)
                        {
                            case "Wood":
                                Debug.Log("Sammle Holz");
                                inventory[0] += ColRate[0];
                                break;
                            case "Stone":
                                Debug.Log("Sammle Stein");
                                inventory[2] += ColRate[2];
                                break;
                            case "Iron":
                                Debug.Log("Sammle Eisen");
                                inventory[1] += ColRate[1];
                                break;
                            default:
                                Debug.LogError("Falscher Tag gestetzt");
                                break;
                        }

                    }
                }
            }
        }
    }
    void get_older()
    {
        age++;
        //maybe build a die checker in
    }
    void not_selected()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
    void selected()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
    bool reachedTarget(NavMeshAgent mNavMeshAgent)
    {
        if (!mNavMeshAgent.pathPending)
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
    }
    bool inventarFull()
    {
        int sum = 0;
        for (int i = 0; i< 4; i++) 
        {
            sum += (int)inventory[i];
        }
        if (sum > max_invent)
        {
            return true;
        }
        return false;
    }
}
