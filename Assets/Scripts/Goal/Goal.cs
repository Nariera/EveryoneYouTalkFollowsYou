using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    public bool Completed { get; set; }
    public string GoalText { get; set; }
    public int Points { get; set; }

    public GameObject UI { get; set; }

    public event System.Action<Goal> OnSatisfied;

    protected void Satisfy()
    {
        if(OnSatisfied != null)
        {
            OnSatisfied(this);
        }
    }

    public void UpdateText()
    {
        if (associatedUIObject)
            associatedUIObject.GetComponentInChildren<UnityEngine.UI.Text>().text = goalText;
    }

    #region Junk
    public bool completed = false;
	public string goalText = "Hey this is Placeholder text";
	public int pointValue = 1;



	public GameObject associatedUIObject;
    #endregion
}
