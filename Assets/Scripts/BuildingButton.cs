using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class BuildingButton : MonoBehaviour, IPointerClickHandler
{
    public BuildingUI BuildingUIScript;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            HausController selScript = BuildingUIScript.selectedBuilding.GetComponent<HausController>();
            
            if ( selScript.getCharactersInside().Count >= 1) 
                
            {
               
                selScript.MoveOutside(selScript.getCharactersInside()[0]);
            }
        }

        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            
        }

    }
   

}
