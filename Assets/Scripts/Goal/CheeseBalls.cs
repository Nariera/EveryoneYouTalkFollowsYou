using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CheeseBalls : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("Start Cheeseball");
        GoalEvents.Instance.AddListener<TestEvent>(TestEventListen);

        GoalEvents.Instance.Raise(new TestEvent()
        {
            name = "Bob"
        });
    }

    public void TestEventListen(TestEvent e)
    {
        Debug.Log("Event Works - " + e.name);
    }
}

public class TestEvent : GoalEvent
{
    public string name { get; set; }
}
