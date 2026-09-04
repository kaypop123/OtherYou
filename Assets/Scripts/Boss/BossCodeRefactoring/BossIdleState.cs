using UnityEngine;

public class BossIdleState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    public BossIdleState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        boss.StopMovement();
    }

    public void Update()
    {
        if (!boss.HasTarget)
            return;

        if (boss.DistanceToTarget <= boss.DetectRange)
        {
            stateMachine.ChangeState(boss.ChaseState);
        }
    }

    public void Exit()
    {
        Debug.Log("보스 대기상태 종료");
    }
}