using UnityEngine;

public class BossUltimateState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    private bool ultimateStarted;

    public BossUltimateState(
        AngryGodAiCoreRE boss,
        BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        ultimateStarted = false;

        boss.StopMovement();
        boss.StartUltimate();
    }

    public void Update()
    {
        if (!ultimateStarted)
        {
            if (boss.IsUltimateActive)
                ultimateStarted = true;

            return;
        }

        if (boss.IsUltimateActive)
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
        Debug.Log("보스 궁극기 상태 종료");
    }
}