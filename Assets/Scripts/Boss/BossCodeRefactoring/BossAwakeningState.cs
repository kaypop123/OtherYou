using UnityEngine;

public class BossAwakeningState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    private bool awakeningStarted;

    public BossAwakeningState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {

        awakeningStarted = false;

        boss.StopMovement();
        boss.StartAwakening();
    }

    public void Update()
    {
        if (!awakeningStarted)
        {
            if (boss.IsAwakening)
                awakeningStarted = true;

            return;
        }

        if (boss.IsAwakening)
            return;

        if (!boss.HasTarget)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
    }

    public void Exit()
    {
        Debug.Log("보스 각성 상태 종료");
    }
}