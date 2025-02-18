using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Transform attacckCheck;
    public float attackCheckRadius;
    public GameObject slashEffect1, slashEffect2, slashEffectAlt1;

    public bool isBusy { get; private set; }

    [Header("Move info")]
    public float moveSpeed = 4f;
    public float jumpForce;

    [Header("Dash info")]
    public bool isDashing;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir = -1;

    [Header("WallSlide info")]
    public float wallSlideForce;

    public bool fromWall = false;

    #region State
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(stateMachine, this, "Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Move");
        fallState  = new PlayerFallState (stateMachine, this, "Jump");
        jumpState = new PlayerJumpState(stateMachine, this, "Jump");
        dashState = new PlayerDashState(stateMachine, this, "Dash");
        wallSlideState = new PlayerWallSlideState(stateMachine, this, "WallSlide");
        wallJumpState = new PlayerWallJumpState(stateMachine, this, "DoubleJump");
        primaryAttackState = new PlayerPrimaryAttackState(stateMachine, this, "Attack");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base .Update();

        stateMachine.currentState.Update();

        CheckForDashInput();
    }

    public virtual void OpenSlashEffect()
    {
        slashEffect1.SetActive(true);
        slashEffect2.SetActive(true);
    }

    public virtual void CloseSlashEffect()
    {
        slashEffect1.SetActive(false);
        slashEffect2.SetActive(false);
    }

    public virtual void OpenSlashEffectAlt()
    {
        slashEffectAlt1.SetActive(true);
    }

    public virtual void CloseSlashEffectAlt()
    {
        slashEffectAlt1.SetActive(false);
    }

    public virtual void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }
    

    private void CheckForDashInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            if (IsWallDetected())
            {
                fromWall = true;
                dashDir = -facingDir;
            }

            stateMachine.ChangeState(dashState);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(attacckCheck.position, attackCheckRadius);
    }
}
