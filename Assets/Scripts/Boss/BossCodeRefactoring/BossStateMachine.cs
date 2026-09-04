public class BossStateMachine
{
    public IBossState CurrentState { get; private set; }

    public void Initialize(IBossState startState)
    {
        CurrentState = startState;
        CurrentState?.Enter();
    }

    public void ChangeState(IBossState newState)
    {
        if (newState == null)
            return;

        if (CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}