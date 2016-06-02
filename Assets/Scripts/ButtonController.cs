using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerClickHandler
{

    public Text Wood, Food, Stone, Iron; // 0=Holz, 1=Eisen, 2=Stein, 3=Nahrung
    public GameObject priceCanvas;
    public GameObject haus;
    private int[] price = new int[4];

    public void Awake()
    {
        price = haus.GetComponent<HausController>().price;
    }

  /*  public void onClick()
    {
        if (GameController.Instance.hasEnoughResources(price))
        {

            GameController.Instance.placeObject(haus);
            priceCanvas.SetActive(true);
            Wood.text = price[0].ToString();
            Iron.text = price[1].ToString();
            Stone.text = price[2].ToString();
            Food.text = price[3].ToString();
        }

    }
    */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            if (GameController.Instance.hasEnoughResources(price))
            {

                GameController.Instance.placeObject(haus);
                priceCanvas.SetActive(false);
            } else
            {
                showPrice();
            }
        }
           
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            showPrice();
        }
            
    }
    private void showPrice()
    {
        priceCanvas.SetActive(true);
        Wood.text = price[0].ToString();
        Iron.text = price[1].ToString();
        Stone.text = price[2].ToString();
        Food.text = price[3].ToString();
    }

}
