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
    public GameObject UI_Building_CharacterInside;
    public GameObject Button_Pref;
    public GameObject selectedBuilding, selectedCharacter;
    public Text FoodCounter, WoodCounter, StoneCounter, IronCounter;
    public GameObject SkillUI;
    public GameObject SkillPanel;

    public float ROTATE_FACTOR = 15.0f;
    public int CHARACTER_FOOD_COST = 50;
    public const float START_SPAWN_TIME = 10f;
    public const float MAX_SPAWN_TIME = 100f;
    public const float MIN_SPAWN_TIME = 5f;
    public int[] START_RESOURCES = new int[4];
    public const int EAT_FOOD_AMOUNT = 5;
    public const float EAT_TIME = 10f;
    public const float START_EAT_TIME = 60f;
    public bool DEBUG_QUICK_SPAWN;
    public bool DEBUG_FREE_BUILDING;
    public bool DEBUG_NO_EATING;
    
   
    private static GameController instance;
    private GameObject buildObject;
    private List<MonoBehaviour> subscribedScripts = new List<MonoBehaviour>();
    private int[] resources = new int[4];// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    private GameObject selectedObject;
    private RaycastHit mp;
    private List<character> m_learningCharacters;
    private int jumpIndex = 0;

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

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        resources = START_RESOURCES;
        updateResourcesCount();
        m_learningCharacters = new List<character>();
        if (DEBUG_QUICK_SPAWN)
        {
            Invoke("spawnPeople", 1f);
        }
        Invoke("spawnPeople", START_SPAWN_TIME);
        Invoke("peopleEat", START_EAT_TIME );
    }
    void gameOver() {




    }
    public void subscribeScript(MonoBehaviour pScript) { subscribedScripts.Add(pScript); }
    public void deSubscribeScript(MonoBehaviour pScript) { if (subscribedScripts.Contains(pScript)) subscribedScripts.Remove(pScript); }
    public List<MonoBehaviour> getScripts() { return subscribedScripts; }
    public void placeObject(GameObject GO)
    {
        buildObject = (GameObject)Instantiate(GO, getMousePosition().point, transform.rotation); //fügt das Object in die Scene ein, dieses muss im Start() sich im GameController subscribeScribt einschreiben
        
    }
   
    public RaycastHit getMousePosition(bool showBuildings = false, bool onlyGround = false)
    {
        RaycastHit temp;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask;
        if (showBuildings)
        {
            mask = (1 << 0) | (1 << 8) | (1 << 5) | (1 << 9);
        }
        else
        {
            mask = (1 << 0) | (1 << 5) | (1 << 9);
        }
        if (onlyGround)
        {
            mask = 1 << 9;
           
        }

        Physics.Raycast(ray, out temp, 1000, mask);
        return temp;

    }
   
    private void spawnPeople()
    {
        Debug.Log("Spawn People wurde aufgerufen");
        if (resources[3] >= CHARACTER_FOOD_COST || DEBUG_QUICK_SPAWN)
        {
            resources[3] -= CHARACTER_FOOD_COST;
            updateResourcesCount();
            GameObject newChar = (GameObject)Instantiate(Character, ResourcenLagerstelle.transform.position, new Quaternion());
            NPCController NPCScript = newChar.GetComponent<NPCController>();
            NPCScript.setTargetPos(Sammelstelle.transform.position);
            if (selectedCharacter == null)
            {
                findScript(newChar).Invoke("selected", 0f);
                selectedCharacter = newChar;
                selectedObject = selectedCharacter;
            }
           
            
        } else
        {
            Debug.Log("Zu wenig resourcen");
        }
        
        Invoke("spawnPeople", calculateSpawnTime());
    }
    private void peopleEat()
    {
        Debug.Log("People eat");
        if (character.getCharacterAmount() == 0)
        {
            gameOver();
        }
        if (!DEBUG_NO_EATING)
        {
            if (resources[3] > 0)
            {
                resources[3] -= character.getCharacterAmount() * EAT_FOOD_AMOUNT;
                updateResourcesCount();
            }
            else
            {
                foreach (character chara in character.getCharacters())
                {

                    chara.isHungry();
                }
            }
        }
       
        
        Invoke("peopleEat", EAT_TIME);
    }
    private float calculateSpawnTime()
    {
        if (DEBUG_QUICK_SPAWN)
        {
            return 1f;
        }
        float part1 = START_SPAWN_TIME;
      
        part1 /= (Statistics.getAllBuildingsFromType("House").Count +1)* (Statistics.getAllCharacterInBuildingsFromType("House").Count +1) * (Mathf.Sqrt(resources[3]) +1);
        part1 *= (Statistics.getAverageAge(Statistics.getAllCharacterInBuildingsFromType("House"))+1) * (Statistics.getAverageIQ(Statistics.getAllCharacterInBuildingsFromType("House"))+1);

        return part1;
    }
    void Update()
    {
        jumpBetweenCharacters();
        if (Input.mousePosition.x < Screen.width - 200 && Input.mousePosition.x > 120)
        {
            if (buildObject == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mp = getMousePosition(true);
                    selectSomething();
                    
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    moveSomething();
                }
            }
            else //falls ein Objekt in der Variable buildObject ist = ein Gebäude wird plaziert
            {
                mp = getMousePosition(false, true);
                rotateSomething();
                buildObject.transform.position = mp.point; //check if empty
                if (Input.GetMouseButtonDown(0))
                {
                    buildSomething();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    cancelBuilding();
                }
                
            }
        }
        

    }
    public void addLearningCharacter(character Char)
    {
        Char.m_learningProgress = 0f;
        m_learningCharacters.Add(Char);
    }
    public bool alreadyLearning(character Char)
    {
       return m_learningCharacters.Exists(x => x == Char);
    }
    public bool isNewSkill(character Char, character.SKILL skill)
    {
        if (alreadyLearning(Char))
        {
            if (Char.getSkills().Exists(x=> x==skill))
            {
                return true;
            }
        }
        return false;
    }
    public void removeLearningCharacter(character Char)
    {
        m_learningCharacters.Remove(Char);
    }
    private void learning()
    {
        foreach (character Character in m_learningCharacters)
        {
            Character.m_learningProgress += Character.getProgressGain();
            if (Character.m_learningProgress >= 1)
            {
                Character.m_learningProgress = 0;
                Character.learnSkill(Character.getLearningSkill());
                m_learningCharacters.Remove(Character);
            }
        }
    }

    private void moveSomething()
    {
        if (selectedObject != null)
        {
            Debug.Log("Moved Selected Game Object " + selectedObject.GetInstanceID());
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
            setUIVisible(false, false);
        }
    }
    private void selectSomething()
    {
        
        Debug.Log("Klicked : " + mp.transform.gameObject.tag);
        if (selectedObject != null) { findScript(selectedObject).Invoke("not_selected", 0); }

        switch (mp.transform.gameObject.tag)
        {
            case "Character":
                selectedObject = mp.transform.gameObject;
                selectedCharacter = selectedObject;
                findScript(selectedObject).Invoke("selected", 0);
                setUIVisible(true, false);
                Debug.Log("Selected an character");
                break;
            case "House":
            case "Store":
            case "Baker":
            case "Smith":
            case "School":
            case "Field":
            case "Mill":
            case "Farm":
            case "University":
            case "WoodcutterGuild":
            case "BuilderGuild":
            case "Hospital":
            case "Smelter":
                selectedObject = mp.transform.gameObject;
                selectedCharacter = null;
                findScript(selectedObject).Invoke("selected", 0);
                Debug.Log("Gebäude gewählt");
                setUIVisible(false, true);
                updateUI_CharactersInside();
                break;
            case "Ground":
                selectedCharacter = null;
                if (selectedObject != null)
                {
                    findScript(selectedObject).Invoke("not_selected", 0);
                    setUIVisible(false, false);
                    selectedObject = null;
                }
                break;
        }
    }
    private void buildSomething()
    {

        if (!checkBuildable(buildObject))
        {
            return;
        }
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
            return;
        }
        
    }
    private void cancelBuilding()
    {
        MonoBehaviour pScript = findScript(buildObject);
        if (pScript != null)
        {
            pScript.Invoke("abort", 0.0f); //bricht das plazieren ab, das Objekt muss sich noch selber desubscriben
            buildObject = null;
        }
    }
    private void rotateSomething()
    {
        if (Input.GetButton("E"))
        {
            buildObject.transform.Rotate(Vector3.up * ROTATE_FACTOR, Space.World);
        }
        else if (Input.GetButton("Q"))
        {
            buildObject.transform.Rotate(Vector3.up * ROTATE_FACTOR * -1, Space.World);
        }
        
    }
    private void jumpBetweenCharacters()
    {
        if (Input.GetButtonDown("Jump"))
        {
            List<character> allCharacters = character.getCharacters();
            Camera.main.GetComponent<CameraController>().jumpToPosition(allCharacters[jumpIndex].getGameObject().transform.position);
            jumpIndex = allCharacters.Count == (jumpIndex +1) ? 0 : (jumpIndex + 1);

        }
    }
    private void setUIVisible(bool character, bool building)
    {
        UI_Character.SetActive(character);
        UI_Building.SetActive(building);

    }
    public void addResources(float[] res)
    {
        for (int i = 0; i < 4; i++)
        {
            resources[i] += (int)(Mathf.Ceil(res[i]));
        }
        updateResourcesCount();

    }
    public bool hasEnoughResources(int[] res)
    {
        if (DEBUG_FREE_BUILDING) { return true; }
        for (int i = 0; i < 4; i++)
        {
            if (Mathf.Ceil(resources[i]) < res[i] && res[i] != 0)
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
    public float getProgressGain(NPCController npcScript)
    {
        return npcScript.getProgressGain();
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
        }
        else
        {
            string[] namen = File.ReadAllLines(Application.dataPath + "/Other/femaleName.txt");
            int randi = Mathf.FloorToInt(Random.Range(0f, (float)namen.Length));
            return namen[randi];
        }

    }
    public bool isRandomMale()
    {

        return Random.Range(0f, 1f) >= 0.5 ? false : true;

    }
    public int getLifeExpectance(bool male, int IQ)
    {
        float IQ_Factor = IQ / 35;
        return male ? Gauss(65, IQ_Factor) : Gauss(67, IQ_Factor);
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
    public void updateUI_CharactersInside()
    {
        for (int i = 0; i < UI_Building_CharacterInside.transform.childCount; i++)
        {
            Destroy(UI_Building_CharacterInside.transform.GetChild(i).gameObject);
        }
        HausController selScript = selectedObject.GetComponent<HausController>();
        if (selScript != null)
        {
            List<character> charInside = selScript.getCharactersInside();
            for (int i = 0; i < charInside.Count; i++)
            {
                addButton(i, UI_Building_CharacterInside.transform, Button_Pref, charInside[i]);

            }

        }

    }
    public void addButton(int index, Transform parent, GameObject ButtonPrefab, character representedCharacter)
    {
        GameObject button = Instantiate(ButtonPrefab);
        button.transform.SetParent(parent, false);
        button.transform.localPosition = new Vector3((index % 4 == 0) ? 25f : 25f + 50f * ((index % 4)), (index / 4 < 1) ? -25f : -25f + ((index / 4) * -50f), 0f);
        button.GetComponent<SelectCharacter>().Character = representedCharacter;
        //den script den character mitteilen den er representiert
        Debug.Log(button.transform.localPosition.ToString() + ": " + button.transform.position.ToString());
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
