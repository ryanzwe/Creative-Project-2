using UnityEngine;

namespace Enemy
{
    public class StateMachine<T>
    {// The current state that the machine is in, as well as the owner of that state 
        public State<T> CurrentState { get; private set; }

        //private State<T> previousState;
        public T Owner;

        public StateMachine(T Owner)
        {// Setting the owner inside the statemachine class, and setting the starting state to null
            this.Owner = Owner;// The state machine will be of the type used in the constructor
            CurrentState = null;
        }
        // The method taking in a new state class to change the machine t
        public void ChangeState(State<T> newState)
        {
            if(CurrentState != null)// If null(When starting) don't run the exit as it has nothing to exit
                CurrentState.ExitState(Owner);
            // Exit state, switch, call EnterState of the new state
          //  previousState = CurrentState;
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
        public abstract void DebugState(T Owner);
    }
}