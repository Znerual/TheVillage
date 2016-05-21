using UnityEngine;
using System.Collections;

public class HausController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Script für added building hinzufügen");
        GameController.Instance.subscribeScript(this);
        gameObject.SetActive(true);
    }
	void placed()
    {
        Debug.Log("Placed");
        gameObject.layer = 0;
    }
    void abort()
    {
        GameController.Instance.deSubscribeScript(this);
        gameObject.SetActive(false);
    }
	// Update is called once per frame
	
}
