using System.Collections.Generic;

public abstract class GoalController
{

    // specialization Queues <Specialization : queue>
    Dictionary<string, List<Goal>> specQueues;
    // e.g Hauling : Job from goal1 <- Job from goal1 <- job from goal2
    // Constructing : {construct bed} <- {constr wall} <- ..

    void RegisterGoal(Goal goal)
    {
        // insert in the corresponding queue at right index calculated by priority
        addByPriority(goal);
        // remove goal when completed so it will be garbage collected 
        goal.onCompletion(specQueues[goal.specialization].remove(goal));
    }

    // Agent calls when Idle
    public bool GetNextGoalFor(GoapAgent agent){ return false;}

    private void addByPriority(Goal goal){}
}