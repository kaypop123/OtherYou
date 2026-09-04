using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    public BossAttackState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {

        boss.StopMovement();
        boss.StartAttack();
    }

    public void Update()
    {
        if (boss.IsAttacking)
            return;

        if (!boss.HasTarget)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        if (boss.DistanceToTarget <= boss.AttackRange)
        {
            stateMachine.ChangeState(boss.IdleState);
            return;
        }

        stateMachine.ChangeState(boss.ChaseState);
    }

    public void Exit()
    {
        Debug.Log("보스 공격 상태 종료");
    }
}