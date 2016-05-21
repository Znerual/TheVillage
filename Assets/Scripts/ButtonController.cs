using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

   
    public GameObject haus;
   

    public void onClick()
    {
        GameController.Instance.placeObject(haus);
    }
}
