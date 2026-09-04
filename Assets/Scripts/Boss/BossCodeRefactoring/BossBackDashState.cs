using UnityEngine;

public class BossBackDashState : IBossState
{
    private AngryGodAiCoreRE boss;
    private BossStateMachine stateMachine;

    public BossBackDashState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        boss.StartBackDash();
    }
    public void Update()
    {
        if (boss.IsBackDashing)
            return;

        if (!boss.HasTarget)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        if (boss.CanUseActiveSkill1())
        {
            stateMachine.ChangeState(boss.ActiveSkill1State);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
    }

    public void Exit()
    {
        Debug.Log("보스 백대쉬 상태 종료");
    }
}