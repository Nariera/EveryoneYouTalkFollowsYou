using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GoalEvent
{

}

public sealed class GoalEvents
{
    public static GoalEvents Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GoalEvents();
            }
            return _Instance;
        }
    }
    private static GoalEvents _Instance;

    public delegate void EventDelegate<T>(T e) where T : GoalEvent;
    private delegate void EventDelegate(GoalEvent e);

    private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
    private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();

    public void AddListener<T>(EventDelegate<T> del) where T : GoalEvent
    {
        // Early-out if we've already registered this delegate
        if (delegateLookup.ContainsKey(del))
            return;

        // Create a new non-generic delegate which calls our generic one.
        // This is the delegate we actually invoke.
        EventDelegate internalDelegate = (e) => del((T)e);
        delegateLookup[del] = internalDelegate;

        EventDelegate tempDel;
        if (delegates.TryGetValue(typeof(T), out tempDel))
        {
            delegates[typeof(T)] = tempDel += internalDelegate;
        }
        else
        {
            delegates[typeof(T)] = internalDelegate;
        }
    }

    public void RemoveListener<T>(EventDelegate<T> del) where T : GoalEvent
    {
        EventDelegate internalDelegate;
        if (delegateLookup.TryGetValue(del, out internalDelegate))
        {
            EventDelegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
            {
                tempDel -= internalDelegate;
                if (tempDel == null)
                {
                    delegates.Remove(typeof(T));
                }
                else
                {
                    delegates[typeof(T)] = tempDel;
                }
            }

            delegateLookup.Remove(del);
        }
    }

    public void Raise(GoalEvent e)
    {
        EventDelegate del;
        if (delegates.TryGetValue(e.GetType(), out del))
        {
            del.Invoke(e);
        }
    }
}

