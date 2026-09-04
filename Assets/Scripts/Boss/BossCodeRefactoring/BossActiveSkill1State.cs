using UnityEngine;

public class BossActiveSkill1State : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    private bool skillStarted;

    public BossActiveSkill1State(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {

        skillStarted = false;

        boss.StopMovement();
        boss.StartActiveSkill1();
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

        if (boss.CanUseSummon())
        {
            stateMachine.ChangeState(boss.SummonState);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
        if (boss.CanUseActiveSkill1())
        {
            stateMachine.ChangeState(boss.ActiveSkill1State);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
    }

    public void Exit()
    {
        Debug.Log("보스 스킬1 상태 종료");
    }
}