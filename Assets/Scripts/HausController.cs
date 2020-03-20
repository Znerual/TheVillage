using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class HausController : MonoBehaviour
{
    
    public int[] price = new int[4];// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    public bool isCollided = false;
    public int status; //0 = nicht initialisert, 10 = fertig gebaut
    public float BUILD_TIME = 1;
    public int MAX_CHAR_INSIDE = 6;
    public GameObject OptionsPanelPrefab;
    private float progress = 0f; // 0 = not bulid to 100
    private float[] collectedResources = new float[4];
    private bool buildingStopped = false;

    private List<character> CharactersInside;
    private Text CTTyp, CTProgress, CTToggleProduction, CTFood, CTWood, CTIron, CTStone, CTTipps;
    private GameObject PanelOptions = null;
    int count = 1;
    void Start()
    {
        GameObject canvas = GameController.Instance.UI_Building;
        CTTyp = canvas.transform.Find("TypBorder").GetChild(0).GetComponent<Text>();
        CTProgress = canvas.transform.Find("BaufortschrittBorder").GetChild(0).GetComponent<Text>();
        CTToggleProduction = canvas.transform.Find("ToggleProduction").GetChild(0).GetComponent<Text>();
       
        CTFood = canvas.transform.Find("FoodImage").GetChild(0).GetComponent<Text>();
        CTWood = canvas.transform.Find("WoodImage").GetChild(0).GetComponent<Text>();
        CTIron = canvas.transform.Find("IronImage").GetChild(0).GetComponent<Text>();
        CTStone = canvas.transform.Find("StoneImage").GetChild(0).GetComponent<Text>();
       

        CharactersInside = new List<character>();
        Debug.Log("Script für added building hinzufügen");
        GameController.Instance.subscribeScript(this);
        gameObject.SetActive(true);
        
    }

    public void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            isCollided = false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            isCollided = true;
        }
    }

    void placed()
    {
        Debug.Log("Placed");
        //gameObject.layer = 8;
        if (!GameController.Instance.DEBUG_FREE_BUILDING)
        {
            GameController.Instance.subtractResources(price);
        }
        
        Destroy(GetComponent<Rigidbody>());
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    void abort()
    {
        GameController.Instance.deSubscribeScript(this);
        gameObject.SetActive(false);
    }
    void selected()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        updateUI();
        if (status == 10)
        {
            updateUIOptions();
        }
    }
    void not_selected()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        cancleOptions();
    }
    public void stopBuilding() { buildingStopped = true; }
    public void continueBuiding() { buildingStopped = false; }
    public bool isBuilding() { return !buildingStopped; }
    public void building(float rate)
    {
        if (progress + rate < 100 && !buildingStopped)
        {
            progress += (rate * Time.deltaTime * 60) / BUILD_TIME;
            if (progress > (status +1) * 10)
            {
                ++status;
            }
            updateUIBuilding();
        } else if (progress + rate >= 100)
        {
            progress = 100f;
            status = 10;
            updateUI();
            updateUIOptions();
        }
        float rat = (float) 100 / gameObject.transform.childCount;
        
        if ((int) rat * (count +1) == (int) progress  && count < gameObject.transform.childCount)
        {
            gameObject.transform.GetChild(count).gameObject.SetActive(true);
            count++;
            //Debug.Log("Zeige: "+ count);
        }
     
    }
    public  void collectingResources(float amount, int resource)
    {
        collectedResources[resource] += amount;
        updateUI();
    } 
    

    
    public void MoveInside(character Character)
    {
        if (CharactersInside.Count < MAX_CHAR_INSIDE)
        {
            CharactersInside.Add(Character);
            Character.getGameObject().SetActive(false);
            
           
            if (GameController.Instance.selectedBuilding == gameObject)
            {
                GameController.Instance.updateUI_CharactersInside();
            }


        } else
        {
            Debug.Log("Building Full");
        }
        
    }
    public List<character> getCharactersInside()
    {
        return CharactersInside;
    }
    public void MoveOutside(character Character)
    {
        Character.getGameObject().SetActive(true);
        CharactersInside.Remove(Character);
        if (GameController.Instance.selectedBuilding == gameObject)
        {
            GameController.Instance.updateUI_CharactersInside();
        }
        Debug.Log("Moved Char outside");
    }
    // Update is called once per frame
    void updateUI()
    {
        CTTyp.text = tag;
        CTProgress.text = ((int)progress).ToString();
        CTToggleProduction.text = "Produktion stoppen";
        

        // 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
        CTWood.text = ((int)(collectedResources[0])).ToString();
        CTIron.text = ((int)collectedResources[1]).ToString();
        CTStone.text = ((int)collectedResources[2]).ToString();
        CTFood.text = ((int)collectedResources[3]).ToString();
        GameController.Instance.selectedBuilding = gameObject;
        //zeige optionen
       
       // lösche alten eintrag und adde neues prefab.
    }
    void updateUIOptions()
    {
        if (PanelOptions == null)
        {
            PanelOptions = (GameObject)Instantiate(OptionsPanelPrefab, Vector3.zero, Quaternion.identity);
            PanelOptions.transform.SetParent(GameController.Instance.UI_Building.transform.Find("OptionsPanel"), false);

        }
        else if (PanelOptions.name != OptionsPanelPrefab.name)
        {
            Destroy(PanelOptions);
            PanelOptions = (GameObject)Instantiate(OptionsPanelPrefab, Vector3.zero, Quaternion.identity);
            PanelOptions.transform.SetParent(GameController.Instance.UI_Building.transform.Find("OptionsPanel"), false);
        }
        GameController.Instance.UI_Building.GetComponent<UIBuiding>().optionsPanel = PanelOptions;
    }
    void updateUIBuilding()
    {
        if (GameController.Instance.selectedBuilding == gameObject)
        {
            CTProgress.text = ((int)progress).ToString();
        }
        
    }
    void cancleOptions()
    {
        Destroy(PanelOptions);
    }
}
