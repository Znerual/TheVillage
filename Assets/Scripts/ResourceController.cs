using UnityEngine;
using System.Collections;

public class ResourceController : MonoBehaviour {
    private float amount;
    public float MAX_AMOUNT;
    private float rat;
    public int status = 10; // 10=full 1 =empty
    public void Awake()
    {
        amount = MAX_AMOUNT;
        rat = MAX_AMOUNT / 10;
    }
	public float getResources(float amount)
    {
        if (amount <= this.amount)
        {
            this.amount -= amount;            
            status = (int)  (this.amount / rat);
            //Debug.Log(status);
            return amount;
        } else
        {
            float return_val = this.amount;
            this.amount = 0f;
            status = 0;
            return return_val;
        }
    }
}
