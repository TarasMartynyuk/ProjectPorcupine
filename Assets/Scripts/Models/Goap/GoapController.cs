using System.Collections.Generic;

public abstract class GoapController
{

    // specialization Queues <Specialization : queue>
    // e.g Hauling : Job from goal1 <- Job from goal1 <- job from goal2
    //     Constructing : {construct bed} <- {constr wall} <- ..
    Dictionary<string, List<Goal>> specQueues;

    public void RegisterGoal(Goal goal)
    {
        // insert in the corresponding queue at right index calculated by priority
        AddByPriority(goal);
       
    }

    public void UnregisterGoal(Goal goal) { specQueues[goal.specialization].Remove(goal); }
    
    // Agent calls when Idle
    public Goal GetNextGoalFor(GoapAgent agent){ return null;}

    private void AddByPriority(Goal goal){}
}