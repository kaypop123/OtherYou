using UnityEngine;

public class BossChaseState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    public BossChaseState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Boss Enter Chase State");
    }

    public void Update()
    {
        if (!boss.HasTarget)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        if (boss.DistanceToTarget > boss.DetectRange)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        if (boss.DistanceToTarget <= boss.BackDashRange)
        {
            if (boss.CanUseFlame())
            {
                stateMachine.ChangeState(boss.FlameState);
                return;
            }

            if (boss.ShouldBackDash())
            {
                stateMachine.ChangeState(boss.BackDashState);
                return;
            }

            stateMachine.ChangeState(boss.AttackState);
            return;
        }

        if (boss.DistanceToTarget <= boss.AttackRange)
        {
            stateMachine.ChangeState(boss.AttackState);
            return;
        }

        stateMachine.ChangeState(boss.DashState);
    }

    public void Exit()
    {
        boss.StopMovement();

        Debug.Log("Boss Exit Chase State");
    }
}