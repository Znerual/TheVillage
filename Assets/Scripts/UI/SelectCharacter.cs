using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour, IPointerClickHandler
{
    public character Character;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            UIBuiding UIBu = GameController.Instance.UI_Building.GetComponent<UIBuiding>();
            UIBu.selectedCharacter = Character;
            HausController selectedBuilding = GameController.Instance.selectedBuilding.GetComponent<HausController>();
            List<character> charactersInside = selectedBuilding.getCharactersInside();
            
            if (charactersInside.Exists(x => x == Character))
            {
                UIBu.optionsPanel.transform.Find("NameBorder").GetChild(0).gameObject.GetComponent<Text>().text = Character.getName();
                if (UIBu.optionsPanel.transform.Find("SkillSelection") != null)
                {
                    DropdownSelection dropD = UIBu.optionsPanel.transform.Find("SkillSelection").GetComponent<DropdownSelection>();
                    dropD.changeValue(Character.getLearningSkill());
                    UIBu.progressBar.GetComponent<ProgressBar>().barDisplay = Character.m_learningProgress;
                }
                
            }          
           
           
        }
    }

}
