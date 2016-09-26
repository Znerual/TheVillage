using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIBuiding : MonoBehaviour {
   
    public character selectedCharacter;
    public GameObject optionsPanel;
    public Text stoppBuilding;
    public GameObject progressBar;
    
   // public List<GameObject> learningCharacters = new List<GameObject>();
   public void setProgressBar(float value)
    {
        if (progressBar != null)
        {
            progressBar.GetComponent<ProgressBar>().barDisplay = value;
        }
    }     
    public void stoppBuildingClick()
    {
        HausController currentBuilding = GameController.Instance.selectedBuilding.GetComponent<HausController>();
        
       
        if (currentBuilding.isBuilding())
        {
            currentBuilding.stopBuilding();
            stoppBuilding.text = "Continue Building";
        } else
        {
            currentBuilding.continueBuiding();           
            stoppBuilding.text = "Stopp Building";
        }
    }                                             
}
