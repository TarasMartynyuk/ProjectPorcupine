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
	private Action<Character> state;
	private static readonly Action<Character> idleState; // finds something to do
	private static readonly Action<Character> moveToState; // moves to a target
	private static readonly Action<Character> performActionState; // performs an action

	private Goal currGoal;
	private GoapPlanner planner;
	
	private HashSet<Action> actions;
	private Queue<Action> plannedActions;
	
	private WorldStateSupplier dataProvider; 
	private Character character;

#region Constructors
	GoapAgent(Character character)
	{
		actions = new HashSet<Action> ();
		plannedActions = new Queue<Action> ();
		planner = new GoapPlanner ();
		this.character = character;
		// loadActions ();
	}

	static GoapAgent()
	{
		createIdleState();
		createMoveToState();
		createPerformActionState();
	}
#endregion
	
	// call in characters Update method
	// Note: this class is not Monobehavior!
	void Update () { state(character); }

#region Actions
    public HashSet<Action> GetActions(){return null;}

    public void AddAction(Action a) {
		actions.Add (a);
	}

	public void RemoveAction(Action action) {
		actions.Remove (action);
	}
#endregion
#region Static state creation
	// checks if there are actions to do, if so performs them, else
	// asks GoalController for new goal and plan for it
	private static Action<Character> getIdleState() 
    {
		return (character) => {
			//
			// get the goal we want to plan for
			currGoal = GoalController.GetNextGoalFor(this);

			// Plan
			Queue<Action> plan = planner.plan(this, currGoal);
			if (plan != null) 
				plannedActions = plan;
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
	private static void createMoveToState() 
	{
		moveToState = (fsm, character) => {
			// move the game object
			Action action = plannedActions.Peek();
			if (action.requiresInRange() && action.target == null) 
			{
				Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed." + 
							"You did not assign the target in your Action.checkProceduralPrecondition()");
				state = idleState; 
				return;
			}

			// get the agent to move itself
			
			if ( character.moveTo(/* target of action*/ ) 
				state
		};
	}
	
	// performs action, this state is active if we already are in the needed place
	private static void createPerformActionState() 
	{
		performActionState = (fsm, character) => {
			if (hasActionPlan() == false) // no actions to perform
			{
				Debug.Log("<color=red>Done actions</color>");
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
				return;
			}

			Action action = plannedActions.Peek();
			if ( action.isDone() ) 
				plannedActions.Dequeue(); // the action is done. Remove it so we can perform the next one

			if (hasActionPlan()) // perform the next action
			{
				action = plannedActions.Peek();
				bool inRange = action.requiresInRange() ? action.isInRange() : true;

				if ( inRange ) 
				{
					// we are in range, so perform the action
					bool success = action.perform(gameObj);

					if (success == false) 
					{
						// action failed, we need to plan again
						fsm.popState();
						fsm.pushState(idleState);
						dataProvider.planAborted(action);
					}
				} 
				else 
					fsm.pushState(moveToState); // we need to move there first

			} 
			else 
			{
				// no actions left, move to Plan state
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
			}

		};
	}
#endregion

    private bool HasActionsToDo() { return plannedActions.Count > 0; }
	private void GetReadyToDoAction(Action actionToDo) 
	{
		// check if need to move
		bool alreadyInRange = actionToDo.requiresInRange() ? actionToDo.isInRange() : true;
		state = alreadyInRange ? performActionState : moveToState;
	}

}