using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    public float barDisplay; //current progress
    private Vector2 pos;
    //public Vector2 pos = new Vector2(20, 40);
    private Vector2 size;
    public Texture2D emptyTex;
    public Texture2D fullTex;

    public void Awake()
    {
        GameController.Instance.UI_Building.GetComponent<UIBuiding>().progressBar = gameObject;
    }
    public void OnTransformParentChanged()
    {
        RectTransform rTran = GetComponent<RectTransform>();
        pos = new Vector2(Screen.width - 198, /*Mathf.Abs(rTran.localPosition.y) + Mathf.Abs(*/transform.position.y - 180/*)*/);
        size = new Vector2(rTran.rect.width - 4, rTran.rect.height - 4);
    }


    void OnGUI()
    {
        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();
    }

    

    public void OnDestroy()
    {
        GameController.Instance.UI_Building.GetComponent<UIBuiding>().progressBar = null;
    }
}
