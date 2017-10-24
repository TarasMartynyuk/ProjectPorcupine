using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ProjectPorcupine.Entities;


/// <summary>
/// Must be a component of every character that wants to use GOAP
/// controls characters behaviour via FSM - performs actions
/// plans action sequences for Goals that are assigned by GoalController
/// </summary>
public sealed class GoapAgent  
{
	// this delegates use instance info, but are still static 
	// and we pass instances as parameters to them
	// because, there is no need to create functions in runtime with static we create Action objects only once
	private Action<GoapAgent> state;
	private static readonly Action<GoapAgent> idleState; // finds something to do
	private static readonly Action<GoapAgent> moveToState; // moves to a target
	private static readonly Action<GoapAgent> performActionState; // performs an action

	private Goal currGoal;
	private GoapPlanner planner;
	
	private HashSet<GoapAction> actions;
	private Queue<GoapAction> plannedActions;
	
	private WorldStateSupplier dataProvider; 
	private Character character;

#region Constructors
	GoapAgent(Character character)
	{
		actions = new HashSet<GoapAction>();
		plannedActions = new Queue<GoapAction>();
		planner = new GoapPlanner();
		this.character = character;
		// loadActions ();
	}

	static GoapAgent()
	{
		idleState = getIdleState();
		createMoveToState();
		createPerformActionState();
	}
#endregion
	
	// call in characters Update method
	// Note: this class is not Monobehavior!
	void Update () { state(this); }

#region Actions
    public HashSet<GoapAction> GetActions(){return null;}

    public void AddAction(GoapAction a) {
		actions.Add (a);
	}

	public void RemoveAction(GoapAction action) {
		actions.Remove (action);
	}
#endregion
#region Static state creation
	// checks if there are actions to do, if so performs them, else
	// asks GoalController for new goal and plan for it
	private static Action<GoapAgent> getIdleState() 
    {
		return (agent) => {
			// check if currentGoal is still not achieved
			if(agent.HasActionsToDo())
				agent.GetReadyToDoAction(agent.plannedActions.Peek());    // pick next action an perform it
			
			// if it is, delete it from GoapController so that garbage collector will destroy the object
			if(agent.currGoal != null)
				WorldController.Instance.GoapController.UnregisterGoal(agent.currGoal);
			// and get the new goal
			agent.currGoal = WorldController.Instance.GoapController.GetNextGoalFor(agent);

			// Plan
			Queue<GoapAction> plan = agent.planner.Plan(agent, agent.currGoal);
			if (plan != null) 
			{
				agent.plannedActions = plan;
				agent.GetReadyToDoAction(agent.plannedActions.Peek());
			}
			else
			{
				//TODO: callback to GoalController and find next suitable goal 
				// // ugh, we couldn't get a plan
				// Debug.Log("<color=orange>Failed Plan:</color>"+prettyPrint(goal));
				// dataProvider.planFailed(goal);
				// fsm.popState (); // move back to IdleAction state
				// fsm.pushState (idleState);
			}
		};
	}
	
	// while in this state character will move to the target specified in his current Action
	// e.g stockpile, or furniture placement spot
	private static Action<GoapAgent> createMoveToState() 
	{
		return (agent) => {
			// move the game object
			GoapAction action = agent.plannedActions.Peek();
			if (action.RequiresInRange() && action.Target == null) 
			{
				Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed." + 
							"You did not assign the target in your Action.checkProceduralPrecondition()");
				agent.state = idleState; 
				return;
			}

			// get the agent to move itself
			
			// if ( character.moveTo(/* target of action*/ ) 
				// state
		};
	}
	
	// performs action, this state is active if we already are in the needed place
	private static Action<GoapAgent> createPerformActionState() 
	{
		return (agent) => {
			// if (agent.HasActionsToDo() == false) // no actions to perform
			// {
			// 	Debug.Log("<color=red>Done actions</color>");
			// 	a
			// 	dataProvider.actionsFinished();
			// 	return;
			// }

			GoapAction action = agent.plannedActions.Peek();
			// perform next action and ,if it is finished this frame,
			// throw it away and change state to idle 
			if (action.Perform(agent.character) == false) 
			{
				// action failed, we need to plan again
				agent.state = idleState;
				agent.plannedActions.Dequeue();
			}
		};
	}
#endregion

    private bool HasActionsToDo() { return plannedActions.Count > 0; }
	private void GetReadyToDoAction(GoapAction actionToDo) 
	{
		// check if need to move
		bool alreadyInRange = actionToDo.RequiresInRange() ? actionToDo.IsInRange() : true;
		state = alreadyInRange ? performActionState : moveToState;
	}

}