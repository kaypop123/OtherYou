using UnityEngine;

public class BossSummonState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    private bool summonStarted;

    public BossSummonState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        summonStarted = false;

        boss.StopMovement();
        boss.StartSummon();
    }

    public void Update()
    {
        if (!summonStarted)
        {
            if (boss.IsSummoning)
                summonStarted = true;

            return;
        }

        if (boss.IsSummoning)
            return;
        if (!boss.HasTarget)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        if (boss.CanUseUltimate())
        {
            stateMachine.ChangeState(boss.UltimateState);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
    }

    public void Exit()
    {
        Debug.Log("보스 소환상태 종료");
    }
}