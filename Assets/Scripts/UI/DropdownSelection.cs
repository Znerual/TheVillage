using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DropdownSelection : MonoBehaviour {

	public void valueChanged()
    {
        //überprüfen ob die auswahl nicht schon gelernt worden ist
        Dropdown ddScript = GetComponent<Dropdown>();
        UIBuiding UIBu = GameController.Instance.UI_Building.GetComponent<UIBuiding>();
        if (UIBu.selectedCharacter != null)
        {
            if (GameController.Instance.isNewSkill(UIBu.selectedCharacter, (character.SKILL)ddScript.value))
            {
                GameController.Instance.addLearningCharacter(UIBu.selectedCharacter);
            }
        }
    }
    public void changeValue(character.SKILL skill)
    {
        Dropdown ddScript = GetComponent<Dropdown>();
        ddScript.value = (int) skill;
    }
}
