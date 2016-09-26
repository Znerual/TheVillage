using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class CharacterOutButton : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            HausController hausScript = GameController.Instance.selectedBuilding.GetComponent<HausController>();
            UIBuiding UIBu = GameController.Instance.UI_Building.GetComponent<UIBuiding>();
            if (hausScript.getCharactersInside().Count >= 1 && UIBu.selectedCharacter != null)
            {
                hausScript.MoveOutside(UIBu.selectedCharacter);
                UIBu.selectedCharacter = null;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("Right click");

            }

        }


    }
}