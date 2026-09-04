using UnityEngine;

public class BossDashState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    public BossDashState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        boss.StartForwardDash();
    }

    public void Update()
    {
        if (boss.IsDashing)
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
        Debug.Log("보스 대쉬 상태 종료");
    }
}