using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HausController : MonoBehaviour
{
    
    public int[] price = new int[4];// 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    public bool isCollided = false;
    public int status; //0 = nicht initialisert, 10 = fertig gebaut
    public float BUILD_TIME = 1;
    private float progress = 0f; // 0 = not bulid to 100
    private List<GameObject> CharactersInside;
    int count = 1;
    void Start()
    {
        CharactersInside = new List<GameObject>();
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
        GameController.Instance.subtractResources(price);
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
    }
    void not_selected()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void building(float rate)
    {
        if (progress + rate <= 100)
        {
            progress += (rate * Time.deltaTime * 60) / BUILD_TIME;
            if (progress > (status +1) * 10)
            {
                ++status;
            }
        } else
        {
            progress = 100f;
            status = 10;
        }
        float rat = (float) 100 / gameObject.transform.childCount;
        
        if ((int) rat * (count +1) == (int) progress  && count < gameObject.transform.childCount)
        {
            gameObject.transform.GetChild(count).gameObject.SetActive(true);
            count++;
            //Debug.Log("Zeige: "+ count);
        }
     
    }
    public void MoveInside(GameObject gObj)
    {
        gObj.SetActive(false);
        CharactersInside.Add(gObj);
        
        
        Debug.Log("Moved Character inside");
    }
    public List<GameObject> getCharactersInside()
    {
        return CharactersInside;
    }
    public void MoveOutside(GameObject gObj)
    {
        gObj.SetActive(true);
        CharactersInside.Remove(gObj);
        Debug.Log("Moved Char outside");
    }
    // Update is called once per frame
    void updateUI()
    {
        GameObject canvas = GameController.Instance.UI_Building;
        BuildingUI Script = canvas.GetComponent<BuildingUI>();
        Script.selectedBuilding = gameObject;
        //verändere das Bild und zeige status...
    }
}
