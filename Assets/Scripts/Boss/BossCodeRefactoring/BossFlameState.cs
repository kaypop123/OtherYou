using UnityEngine;

public class BossFlameState : IBossState
{
    private readonly AngryGodAiCoreRE boss;
    private readonly BossStateMachine stateMachine;

    private bool flameStarted;

    public BossFlameState(AngryGodAiCoreRE boss, BossStateMachine stateMachine)
    {
        this.boss = boss;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        flameStarted = false;

        boss.StopMovement();
        boss.StartFlame();
    }

    public void Update()
    {
        if (!flameStarted)
        {
            if (boss.IsFlaming)
            {
                flameStarted = true;
            }

            return;
        }
        if (boss.IsFlaming)
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
        Debug.Log("보스 화염스킬 상태 종료");
    }
}