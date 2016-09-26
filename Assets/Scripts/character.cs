using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class character  {
    private static List<character> s_characters = new List<character>();
    public static int getCharacterAmount() { return s_characters.Count; }
    public static List<character> getCharacters() { return s_characters; }

    public enum SKILL { None, Bauen, FortgeschrittenesBauen, MeisterlichesBauen, Schmieden, Eisenverarbeitung, Lehren, Landplanung, Bäcker, Forscher, Arzt, Sammler, FortgeschrittenesHolzfällen, FortgeschrittenesSteinEisenMetzen };
    public enum STATE { HEALTHY, SICK, DEAD };

    public struct INVENTORY
    {
        public int wood;
        public int iron;
        public int stone;
        public int food;
    };
    private const float RATE_FACTOR = 60f;
    private const int MAX_AGE = 100;
    private const float MIN_CLICK_DISTANCE = 1f;
    private const float STOPPING_DISTANCE_BUILDING = 2f;
    private const float STOPPING_DISTANCE_GROUND = 0f;
    private const int MAX_INVENTORY_PLACE = 50;
    private const float MIN_BUILDING_RATE = 0.1f;
    private const float MAX_BUILDING_RATE = 5f;
    

    float[] m_inventory;// = //new float[4]; // 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung

    private GameObject m_character;

    private SKILL m_learningSkill = SKILL.None;
    public float m_learningProgress = 0f;
    private float m_learningGain = 0.1f;
    private List<SKILL> m_learnedSkills; //initialisieren

    private int m_age = 1;
    private string m_name;
    private int m_IQ;
    private bool m_male;
    private int m_lifeExpectance;

    
    private float[] m_ColRate = { 0.1f, 0.1f, 0.1f, 0.1f };
    

    private NavMeshAgent m_navMeshAgent;

    private STATE m_status = STATE.HEALTHY;
    private bool m_building;
    private bool m_walking;
   
    private GameObject m_targetObject, m_oldObject;
    private Vector3 m_oldGroundPosition, m_targetGroundPosition;

    public character(GameObject character,  string name, int iq, bool male, int lifeExpectance, NavMeshAgent navMeshAgent)
    {
      //  useGUILayout = false;
        m_inventory = new float[4];
        m_character = character;
        m_learnedSkills = new List<SKILL>();

        m_name = name;
        m_IQ = iq;
        m_male = male;
        m_lifeExpectance = lifeExpectance;
        m_navMeshAgent = navMeshAgent;
        s_characters.Add(this);
    }
    ~character()
    {
        s_characters.Remove(this);
    }
    public bool getOlderIsAlive()
    {
        if (m_status == STATE.DEAD)
        {
            return false;
        }
        m_age++;
        if (m_lifeExpectance - m_age < 10 || m_status == STATE.SICK)
        {
            float schwellwert = 10f / (Mathf.Abs(m_lifeExpectance - m_age) + 0.1f);
            if ((schwellwert > UnityEngine.Random.value && m_lifeExpectance - m_age < 0) || (schwellwert < UnityEngine.Random.value && m_lifeExpectance - m_age > 0) || m_age == MAX_AGE)
            {
                return false;
            }
        }
        return true;
    }
    public void moveCharacterToMousePosition()
    {
        RaycastHit mousePosition = GameController.Instance.getMousePosition(true);
        if (Vector3.Distance(mousePosition.point, m_navMeshAgent.destination) > MIN_CLICK_DISTANCE)
        {
            moveCharacter(mousePosition);

        }
    }
    public void moveCharacterToPosition(Vector3 position)
    {
        RaycastHit targetPosition;
        Vector3 pos = position;
        pos.y = 100;
        Physics.Raycast(pos, -Vector3.up, out targetPosition);
        moveCharacter(targetPosition);

    }
    public bool reachedTarget()
    {
        if (Vector3.Distance(m_character.transform.position, m_navMeshAgent.destination) < m_navMeshAgent.stoppingDistance)
        {
            return true;
        }
        return false;
    }
    private void moveCharacter(RaycastHit position)
    {
        m_walking = true;
        m_building = false;
        m_targetObject = position.transform.gameObject;
        m_targetGroundPosition = position.point;
        if (m_targetObject != null && m_targetObject.tag != null && m_targetObject.tag != "Untagged" && m_targetObject.tag != "Ground")
        {
            m_navMeshAgent.stoppingDistance = STOPPING_DISTANCE_BUILDING;
            m_navMeshAgent.SetDestination(m_targetObject.transform.localPosition);
            m_targetGroundPosition = m_targetObject.transform.localPosition;
        }
        else
        {
            m_navMeshAgent.stoppingDistance = STOPPING_DISTANCE_GROUND;
            m_navMeshAgent.SetDestination(position.point);
            
        }
    }
    public SKILL getLearningSkill() { return m_learningSkill; }
    public void learnSkill(SKILL newSkill) { m_learnedSkills.Add(newSkill); }
    public float getProgressGain() { return m_learningGain; }
    public string getName() { return m_name; }
    public int getIQ() { return m_IQ; }
    public int getAge() { return m_age; }
    private int getMaxInventory()
    {
        return m_status == STATE.HEALTHY ? (int)(Mathf.Ceil((float)(m_age / 20f) * MAX_INVENTORY_PLACE - 2* m_age)) +10 : 5;
    }
    private float getBuildingRate()
    {
        float result = 0f;
        if (m_status == STATE.SICK)
        {
            return MIN_BUILDING_RATE;
        }
        result +=(float) (m_IQ / 70f);
        result += m_age >= 20 ? (m_age >= 40 ? (m_age >= 60 ? -1f : 0.25f) : 0.5f) : -1f;
        result += m_learnedSkills.Exists(x => x == SKILL.Bauen) ? 1 : 0;
        result += m_learnedSkills.Exists(x => x == SKILL.FortgeschrittenesBauen) ? 2 : 0;
        result += m_learnedSkills.Exists(x => x == SKILL.MeisterlichesBauen) ? 3 : 0;
        return Mathf.Clamp(result, MIN_BUILDING_RATE, MAX_BUILDING_RATE) * Time.deltaTime * RATE_FACTOR;
        
    }
    public INVENTORY getInventory()
    {
        INVENTORY localInventory = new INVENTORY();
        localInventory.wood = (int)(Mathf.Ceil(m_inventory[0]));
        localInventory.iron = (int)(Mathf.Ceil(m_inventory[1]));
        localInventory.stone = (int)(Mathf.Ceil(m_inventory[2]));
        localInventory.food = (int)(Mathf.Ceil(m_inventory[3]));
        return localInventory;
    }
    public float[] getResources() { return m_inventory; }
    public STATE getState() { return m_status; }
    public bool getMale() { return m_male; }
    public List<SKILL> getSkills() { return m_learnedSkills; }
    public bool isInventoryFull()
    {
        int sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += (int)(Mathf.Ceil(m_inventory[i]));
        }
        if (sum >= getMaxInventory())
        {
            return true;
        }
        return false;
    }
    public void isHungry()
    {
        if (Random.Range(0f,1f) < 0.5f)
        {
            if (m_status == STATE.HEALTHY)
            {
                m_status = STATE.SICK;
            } else
            {
                m_status = STATE.DEAD;
            }
            
        }
    }
    public void collectResources(bool fromBuilding = false)
    {
        ResourceController resource = m_targetObject.GetComponent<ResourceController>();
        int rT = (int)resource.resourceType;
        float amount = m_ColRate[rT] * Time.deltaTime * RATE_FACTOR;
        switch (resource.resourceType)
        {
            case ResourceController.RESOURCE.FOOD:
                amount += m_learnedSkills.Exists(x => x == SKILL.Sammler) ? 1 : 0;
                break;
            case ResourceController.RESOURCE.IRON:
            case ResourceController.RESOURCE.STONE:
                amount += m_learnedSkills.Exists(x => x == SKILL.FortgeschrittenesSteinEisenMetzen) ? 1 : 0;
                break;
            case ResourceController.RESOURCE.WOOD:
                amount += m_learnedSkills.Exists(x => x == SKILL.FortgeschrittenesHolzfällen) ? 1 : 0;
                break;
        }
        
        m_inventory[rT] += resource.getResources(amount);
        if (fromBuilding)
        {
            HausController HausScript = m_targetObject.GetComponent<HausController>();
            HausScript.collectingResources(amount, rT);
        }
    }
    public bool building(SKILL neededSkill = SKILL.None)
    {
        if (neededSkill == SKILL.None || m_learnedSkills.Exists(x => x == neededSkill))
        {
            HausController HausScript = m_targetObject.GetComponent<HausController>();
            if (HausScript.status < 10)
            {
                HausScript.building(getBuildingRate());
                m_building = true;
            }
        }
        return m_building;

    }
    public void moveInside()
    {
        m_walking = false;
        m_building = false;
        HausController HausScript = m_targetObject.GetComponent<HausController>();
        m_oldObject = null;
        m_targetObject = null;
        m_oldGroundPosition = Vector3.zero;
        m_targetGroundPosition = Vector3.zero;
        HausScript.MoveInside(this);
       
      
    }
    public void clearInventory()
    {
        m_inventory = new float[4];
    }
    public bool isWalking() { return m_walking; }
    public void stopWalking() { m_walking = false; }
    public bool isBuilding() { return m_building; }
    public float walkingSpeed() { return m_navMeshAgent.velocity.magnitude; }
    public GameObject getTargetObject() { return m_targetObject; }
    public GameObject getGameObject() { return m_character; }
    public void goToStore()
    {
        m_oldObject = m_targetObject;
        m_oldGroundPosition = m_targetGroundPosition;
        Vector3 selectedStore = GameController.Instance.ResourcenLagerstelle.transform.position;
        float bestDistance = Vector3.Distance(selectedStore, m_character.transform.position);
        foreach (GameObject store in GameObject.FindGameObjectsWithTag("Store"))
        {
            bool buildingBuilded = store.GetComponent<HausController>().status == 10;
            if (buildingBuilded)
            {
                float distance = Vector3.Distance(store.transform.position, m_character.transform.position);
                if (distance < bestDistance)
                {
                    selectedStore = store.transform.position;
                    bestDistance = distance;
                }
            }
        }
        moveCharacterToPosition(selectedStore);
    }
    public void goBackFromStore()
    {
        if (m_oldObject != null)
        {
            m_targetObject = m_oldObject;
        }
        moveCharacterToPosition(m_oldGroundPosition);
    }
}