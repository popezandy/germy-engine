public class IActionMachine{

    public IActionSystem currentState;
    public IActionSystem previousState;

    public void ChangeState(IActionSystem newActionSystem)
    {
        this.currentState = newActionSystem;
    }

    public void ClearState()
    {
        this.currentState = null;
    }

    public void ExecuteStateUpdate()
    {
        this.currentState.Run();
    }
}
