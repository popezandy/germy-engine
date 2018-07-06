using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    private IState currentState;
    private IState previousState;


	public void ChangeState(IState newState)
    {
        if (this.currentState != null)
        {
            this.currentState.Exit();
        }
        this.previousState = this.currentState;
        this.currentState = newState;
    }

    public void ExecuteStateUpdate()
    {
        var runningState = this.currentState;
        if (runningState != null)
        {
            runningState.Execute();
        }
    }

    public void SwitchToPreviousState()
    {
        this.currentState.Exit();
        this.currentState = previousState;
        this.currentState.Enter();
    }
}
