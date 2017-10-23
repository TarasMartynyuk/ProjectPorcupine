using System.Collections.Generic;
using System;

/// <summary>
/// this class represents a task of some specialization
/// they're meant to be Agent-bound, 1 goal for 1 agent at a time
/// and so low-level  
/// </summary>
public class Goal
{
    HashSet<KeyValuePair<string,object>> goalWorldProperties;
    int priority;
    public string specialization = "Hauling";

    // allow only 1 agent to work on it , 
    // if something goes wrong and we unbound the Goal from that agent
    // before it's completed, 
    // it will be assigned anew, and action sequence will be calculated again
    bool taken;

    public Goal(HashSet<KeyValuePair<string,object>> goalWorldProperties, int priority, string specialization)
    {
        this.goalWorldProperties = goalWorldProperties;
        this.priority = priority;
        this.specialization = specialization;
    }
    
    // goap agent calls when
    public void OnCompletion(Action callback){}
    
}