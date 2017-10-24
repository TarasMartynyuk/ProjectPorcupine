using UnityEngine;
using ProjectPorcupine.Entities;
public abstract class GoapAction
{
    // this can vary depending on world state
    // e.g - how far is agent from target
    // so, it must be computed every time we make a plan
    // -1 means that it has not been computed yet
    protected int cost = -1;
    protected GameObject target;
    // protected bool isDone;
    
    public GameObject Target {get; protected set; }
    // public bool IsDone {get; protected set; }

    /// <summary>
    /// subclasses that have static cost should just return it
    /// subclasses that have state-dependent cost should compute it if it is not computed yet(-1)
    /// if recompute is true - compute anew even if it has been computed
    /// </summary>
    /// <param name="recompute"></param>
    /// <returns></returns>
    public abstract int GetCost(bool recompute);
    public abstract bool RequiresInRange();
    public abstract bool IsInRange();
    
    /// <summary>
    /// performs action during this frame
    /// returns true if action is done after, 
    /// else false
    /// </summary>
    /// <returns></returns>
    public abstract bool Perform(Character character);
}