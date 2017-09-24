using UnityEngine;

namespace Enemy
{
    public class StateMachine<T>
    {// The current state that the machine is in, as well as the owner of that state 
        public State<T> CurrentState { get; private set; }
        public T Owner;

        public StateMachine(T Owner)
        {// Setting the owner inside the statemachine class, and setting the starting state to null
            this.Owner = Owner;
            CurrentState = null;
        }
        // The method taking in a new state class to change the machine to
        public void ChangeState(State<T> newState)
        {
            if(CurrentState != null)// If null(When starting) don't run the exit as it has nothing to exit
                CurrentState.ExitState(Owner);
            // Exit state, switch, call EnterState of the new state
            CurrentState = newState;
            CurrentState.EnterState(Owner);
        }

        public void Update()
        {
            if (CurrentState != null)
                CurrentState.UpdateState(Owner);
        }
    }
    // This is abstract as it is the base class for the states
    public abstract class State<T>
    {
        public abstract void EnterState(T Owner);
        public abstract void ExitState(T Owner);
        public abstract void UpdateState(T Owner);
    }
}