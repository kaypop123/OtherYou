using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryGodAiCoreRE : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Range")]
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float attackRange = 2f;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;

    private BossStateMachine stateMachine;

    public BossIdleState IdleState { get; private set; }
    public BossChaseState ChaseState { get; private set; }

    public bool HasTarget => target != null;
    [Header("Attack")]
    [SerializeField] private float attackHitTiming = 0.5f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float chaseDashTriggerRange = 3f;

    [Header("Chase Dash")]
    [SerializeField] private float chaseDashSpeedMultiplier = 2f;
    [SerializeField] private float chaseDashDuration = 0.2f;


    [Header("BackDash")]
    [SerializeField] private float backdashRange = 1.5f;
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float backdashUpwardFactor = 0.3f;

    public float BackDashRange => backdashRange;

    private bool isBackDashing;

    public bool IsBackDashing => isBackDashing;

    public BossBackDashState BackDashState { get; private set; }

    private bool isAttacking;
    private bool isChaseDashing;

    public bool IsAttacking => isAttacking;
    public float DistanceToTarget
    {
        get
        {
            if (target == null)
                return Mathf.Infinity;

            return Vector2.Distance(transform.position, target.position);
        }
    }

    public float DetectRange => detectRange;
    public float AttackRange => attackRange;

    public BossAttackState AttackState { get; private set; }

    [Header("AI Action Probability")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashProbability = 0.6f;


    private AngryGodFlameSkill flameSkill;
    public BossFlameState FlameState { get; private set; }


    public BossDashState DashState { get; private set; }
    private bool isDashing;

    public bool IsDashing => isDashing;

    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private AfterImagePool afterImagePool;

    [SerializeField] private float afterImageInterval = 0.05f;

    private Coroutine afterImageCoroutine;

    private SpriteRenderer spriteRenderer;

    private AngryGodActiveSkill1 activeSkill1;

    public BossActiveSkill1State ActiveSkill1State { get; private set; }

    public bool IsActiveSkill1 => activeSkill1 != null && activeSkill1.IsSkillActive;

    private BossSummoner bossSummoner;

    public BossSummonState SummonState { get; private set; }

    public bool IsSummoning => bossSummoner != null && bossSummoner.IsSummoning;

    private AngryGodUltimateSkill ultimateSkill;

    public BossUltimateState UltimateState { get; private set; }

    public bool IsUltimateActive => ultimateSkill != null && ultimateSkill.IsUltimateActive;

    private bool awakeningRequested;
    private bool isAwakening;

    private BossHurt bossHurt;

    public BossAwakeningState AwakeningState { get; private set; }

    public bool IsAwakening => isAwakening;
    [Header("Awakening Settings")]
    [SerializeField] private float awakeningAnimationDuration = 3f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        flameSkill = GetComponent<AngryGodFlameSkill>();

        stateMachine = new BossStateMachine();

        IdleState = new BossIdleState(this, stateMachine);
        ChaseState = new BossChaseState(this, stateMachine);
        AttackState = new BossAttackState(this, stateMachine);
        BackDashState = new BossBackDashState(this, stateMachine);

        // 추가
        FlameState = new BossFlameState(this, stateMachine);
        DashState = new BossDashState(this, stateMachine);
        spriteRenderer = GetComponent<SpriteRenderer>();

        activeSkill1 = GetComponent<AngryGodActiveSkill1>();

        ActiveSkill1State =
            new BossActiveSkill1State(this, stateMachine);

        bossSummoner = GetComponent<BossSummoner>();

        SummonState =
            new BossSummonState(this, stateMachine);

        ultimateSkill = GetComponent<AngryGodUltimateSkill>();

        UltimateState =
            new BossUltimateState(this, stateMachine);

        bossHurt = GetComponent<BossHurt>();

        AwakeningState =
            new BossAwakeningState(this, stateMachine);

    }

    private void Start()
    {
        stateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (awakeningRequested && CanEnterAwakening())
        {
            awakeningRequested = false;

            stateMachine.ChangeState(AwakeningState);
            return;
        }

        stateMachine.Update();
    }
    private bool CanEnterAwakening()
    {
        if (isAwakening)
            return false;

        if (stateMachine.CurrentState == AwakeningState)
            return false;

        if (IsAttacking)
            return false;

        if (IsBackDashing)
            return false;

        if (IsDashing)
            return false;

        if (IsFlaming)
            return false;

        if (IsActiveSkill1)
            return false;

        if (IsSummoning)
            return false;

        if (IsUltimateActive)
            return false;

        return true;
    }
    public void StopMovement()
    {
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public void MoveToTarget()
    {
        if (target == null || rb == null)
            return;

        float direction = Mathf.Sign(target.position.x - transform.position.x);

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("NomalAttack");
    }
    public void OnAttackEnd()
    {
        if (!HasTarget)
        {
            stateMachine.ChangeState(IdleState);
            return;
        }

        if (DistanceToTarget <= AttackRange)
        {
            stateMachine.ChangeState(IdleState);
            return;
        }

        stateMachine.ChangeState(ChaseState);
    }

    public void StartAttack()
    {
        if (isAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMovement();
        FaceTarget();

        animator.SetTrigger("NomalAttack");

        float timeElapsed = 0f;

        while (timeElapsed < attackHitTiming && isAttacking)
        {
            if (HasTarget && !isChaseDashing)
            {
                if (DistanceToTarget > chaseDashTriggerRange)
                {
                    StartCoroutine(ChaseDashDuringAttack());
                }
            }

            yield return new WaitForSeconds(0.1f);

            timeElapsed += 0.1f;

            if (!HasTarget)
            {
                isAttacking = false;
                yield break;
            }
        }

        while (isChaseDashing)
        {
            yield return null;
        }

        if (isAttacking)
        {
            PerformAttackHit();
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
    private IEnumerator ChaseDashDuringAttack()
    {
        if (target == null || rb == null)
            yield break;

        isChaseDashing = true;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        FaceTarget();

        float speed = moveSpeed * chaseDashSpeedMultiplier;

        rb.velocity = direction * speed;

        yield return new WaitForSeconds(chaseDashDuration);

        if (isAttacking)
            rb.velocity = Vector2.zero;

        isChaseDashing = false;
    }
    public void FaceTarget()
    {
        if (target == null)
            return;

        bool targetIsRight = target.position.x > transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.flipX = !targetIsRight;
    }

    private void PerformAttackHit()
    {
        Debug.Log("보스 공격 판정!");
    }
    public void TriggerAttackLunge()
    {
        if (!isAttacking || isChaseDashing)
            return;

        StartCoroutine(AttackLungeRoutine());
    }

    private IEnumerator AttackLungeRoutine()
    {
        if (target == null || rb == null)
            yield break;

        Vector2 moveDir = target.position.x > transform.position.x ? Vector2.right : Vector2.left;

        float lungeDistance = 1.5f;
        float lungeDuration = 0.15f;
        float lungeSpeed = lungeDistance / lungeDuration;

        rb.velocity = moveDir * lungeSpeed;

        yield return new WaitForSeconds(lungeDuration);

        rb.velocity = Vector2.zero;
    }

    public void StartBackDash()
    {
        if (isBackDashing)
            return;

        isBackDashing = true;

        StopMovement();
        FaceTarget();

        animator.SetTrigger("Backdash");
    }
    public void TriggerBackdashMovementFromAnim()
    {
        if (!isBackDashing)
            return;

        StartCoroutine(BackDashMovementRoutine());
    }

    private IEnumerator BackDashMovementRoutine()
    {
        if (rb == null)
            yield break;

        StartDashEffect(); 

        Vector2 backwardDirection;

        if (target != null && target.position.x > transform.position.x)
        {
            backwardDirection = Vector2.left;
        }
        else
        {
            backwardDirection = Vector2.right;
        }

        Vector2 upwardDirection = Vector2.up * backdashUpwardFactor;

        Vector2 dashDirection = (backwardDirection + upwardDirection).normalized;

        float dashSpeed = dashDistance / dashDuration;

        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;

        StopDashEffect(); 

        isBackDashing = false;
    }

    public bool ShouldBackDash()
    {
        return Random.value < backdashProbability;
    }

    public bool IsFlaming
    {
        get
        {
            return flameSkill != null && flameSkill.IsFlaming;
        }
    }

    public bool CanUseFlame()
    {
        if (flameSkill == null)
            return false;

        return Time.time >= flameSkill.GetLastFlameTime() + 15f;
    }

    public void StartFlame()
    {
        if (flameSkill == null)
            return;

        StartCoroutine(flameSkill.TryUseFlame());
    }

    public void StartForwardDash()
    {
        if (isDashing)
            return;

        StartCoroutine(ForwardDashRoutine());
    }

    private IEnumerator ForwardDashRoutine()
    {
        if (target == null || rb == null)
            yield break;

        isDashing = true;

        StartDashEffect();

        FaceTarget();
        animator.SetTrigger("Dash");

        Vector2 direction = target.position.x > transform.position.x ? Vector2.right : Vector2.left;

        float dashSpeed = dashDistance / dashDuration;

        rb.velocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;

        StopDashEffect();

        isDashing = false;
    }

    private void StartDashEffect()
    {
        if (dashTrail != null)
            dashTrail.emitting = true;

        if (afterImageCoroutine == null)
            afterImageCoroutine = StartCoroutine(LeaveAfterImage());
    }

    private void StopDashEffect()
    {
        if (dashTrail != null)
            dashTrail.emitting = false;

        if (afterImageCoroutine != null)
        {
            StopCoroutine(afterImageCoroutine);
            afterImageCoroutine = null;
        }
    }

    private IEnumerator LeaveAfterImage()
    {
        while (true)
        {
            if (afterImagePool != null)
            {
                afterImagePool.ShowAfterImage(spriteRenderer);
            }

            yield return new WaitForSeconds(afterImageInterval);
        }
    }
    public Transform GetTarget()
    {
        return target;
    }
    public void StartActiveSkill1()
    {
        if (activeSkill1 == null)
            return;

        StartCoroutine(activeSkill1.StartSkill());
    }

    public bool CanUseActiveSkill1()
    {
        return activeSkill1 != null && activeSkill1.CanUseSkill;
    }


    public bool CanUseSummon()
    {
        return bossSummoner != null && bossSummoner.CanUseSummon;
    }

    public void StartSummon()
    {
        if (bossSummoner == null)
            return;

        bossSummoner.StartSummon();
    }
    public bool CanUseUltimate()
    {
        if (ultimateSkill == null || bossHurt == null)
            return false;

        bool isHpConditionMet = bossHurt.currentHealth <= bossHurt.MaxHealth * 0.5f;

        return isHpConditionMet &&
               ultimateSkill.CanUseUltimate;
    }

    public void StartUltimate()
    {
        if (ultimateSkill == null)
            return;

        StartCoroutine(ultimateSkill.TryStartUltimate());
    }

    public void RequestAwakeningSequence()
    {
        if (awakeningRequested || isAwakening)
            return;

        awakeningRequested = true;
    }

    public void StartAwakening()
    {
        if (isAwakening)
            return;

        StartCoroutine(AwakeningRoutine());
    }
    private IEnumerator AwakeningRoutine()
    {
        isAwakening = true;

        StopMovement();
        FaceTarget();

        StartBackDash();

        while (!IsBackDashing)
            yield return null;

        while (IsBackDashing)
            yield return null;

        StopMovement();
        animator.SetTrigger("Awakening");

        yield return new WaitForSeconds(awakeningAnimationDuration);


        if (bossHurt != null && bossHurt.phase2Object != null && !bossHurt.phase2Object.activeSelf)
        {
            bossHurt.phase2Object.SetActive(true);
        }

        isAwakening = false;

    }
}