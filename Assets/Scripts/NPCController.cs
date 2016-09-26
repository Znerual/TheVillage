using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
public class NPCController : MonoBehaviour
{
   
    character Character;
    private Text CTName, CTMale, CTAge, CTIQ, CTStatus, CTFood, CTWood, CTStone, CTIron;
    
    private Animator anim;
   
    public void Awake()
    {
        
        GameController.Instance.subscribeScript(this);
       // nAgent = GetComponent<NavMeshAgent>();
        Debug.Log("Script angemeldet: ");

        int IQ = GameController.Instance.getRandomIQ();
     //   status = "Gesund";
       bool male = GameController.Instance.isRandomMale();
       string CharacterName = GameController.Instance.getRandomName(male);
       int lifeExpectance = GameController.Instance.getLifeExpectance(male, IQ);
        Character = new character(gameObject, CharacterName, IQ, male, lifeExpectance, GetComponent<NavMeshAgent>());

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

        anim = GetComponent<Animator>();
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
        Character.moveCharacterToMousePosition();
    }
    public void setTargetPos(Vector3 targetPos)
    {
        Character.moveCharacterToPosition(targetPos);
    }
    public void learnSkill(character.SKILL skill)
    {
        Character.learnSkill(skill);
    }
    public string getName()
    {
        return Character.getName();
    }
    public float getProgressGain()
    {
        return Character.getProgressGain();
    }
    public character getCharacter() { return Character; }
    void Update()
    {
        if (Character.isWalking())
        {
            anim.SetFloat("speed", Character.walkingSpeed());
            if (Character.reachedTarget())
            {
                Character.stopWalking();
                anim.SetFloat("speed", 0f);
            }
        }
        else
        {
            if (Character.getTargetObject() != null && Character.getTargetObject().tag != "Ground" && Character.getTargetObject().tag != null && Character.getTargetObject().tag != "Untagged" && gameObject.activeSelf == true) //überprüft ob das Objekt das beim setzten des Bewegbefehls den Tag Ground hat, falls nicht...
            {      
                switch (Character.getTargetObject().tag)
                {
                    case "Wood":
                    case "Stone":                      
                    case "Iron":                      
                    case "Food":
                        if (!checkInventoryFull())
                        {
                            Character.collectResources();
                        }                       
                        break;
                    case "House":
                       if (!Character.building()) { 
                            Character.moveInside();
                        }
                        break;
                    case "Store":
                        if (!Character.building() && !Character.isBuilding())
                        {                                                  
                            GameController.Instance.addResources(Character.getResources());
                            Character.clearInventory();
                            Character.goBackFromStore();
                            updateUI();
                        }

                        break;
                    case "Mill":
                        if (!Character.building() && !Character.isBuilding())
                        {
                            Character.moveInside();
                        }

                        break;
                    case "Field":
                    case "Farm":
                        if (!Character.building() && !Character.isBuilding())
                        {
                            if (!checkInventoryFull())
                            {
                                Character.collectResources(true);
                            }
                            
                        }
                        break;
                    case "School":
                        if (!Character.building() && !Character.isBuilding())
                        {
                            Character.moveInside();
                        }
                        break;
                    case "University":
                    case "WoodcutterGuild":
                    case "BuilderGuild":
                    case "Smelter":
                    case "Hospital":
                    case "Smith":
                        //falls faehigkeit vorhanden kann npc in dem gebäude arbeiten, verbraucht material pro zeit, muss wie für das Lernen einer fähigkeit eine Liste geben die vom gameController durchgegangen wird
                    case "Baker":
                        if (!Character.building() && !Character.isBuilding())
                        {
                            Character.moveInside();
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
            }


        }
    }
    private bool checkInventoryFull()
    {
        if (Character.isInventoryFull())
        {
            Character.goToStore();
            return true;
        }
        return false;
    }
    void get_older()
    {
        if (!Character.getOlderIsAlive()) { die(); }
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
    
    void updateUI()
    {
        if (gameObject == GameController.Instance.selectedCharacter)
        {
            CTName.text = Character.getName();
            CTMale.text = Character.getMale() ? "Männlich" : "Weiblich";
            CTAge.text = Character.getAge().ToString();
            CTIQ.text = Character.getIQ().ToString();
            CTStatus.text = Character.getState().ToString();
            CTFood.text = Character.getInventory().food.ToString();
            CTWood.text = Character.getInventory().wood.ToString();
            CTStone.text = Character.getInventory().stone.ToString();
            CTIron.text = Character.getInventory().iron.ToString();
            for (int i = 0; i < GameController.Instance.SkillPanel.transform.childCount; i++)
            {
                Destroy(GameController.Instance.SkillPanel.transform.GetChild(i).gameObject);

            }
            for (int j = 0; j < Character.getSkills().Count; j++)
            {
                GameObject skillGO = Instantiate(GameController.Instance.SkillUI);
                skillGO.transform.SetParent(GameController.Instance.SkillPanel.transform);
                skillGO.transform.localPosition = new Vector3(0f, 30 * j);
                skillGO.transform.GetChild(0).GetComponent<Text>().text = Character.getSkills()[j].ToString();
            }
        }
        
    }
}
