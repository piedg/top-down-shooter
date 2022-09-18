using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    private readonly int SpeedHash = Animator.StringToHash("Speed");

    private const float CrossFadeduration = 0.1f;
    private const float AnimatorDumpTime = 0.1f;

    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (!stateMachine.CooldownManager.CooldownActive(stateMachine.JumpAttackCooldown.ToString()))
            stateMachine.CooldownManager.BeginCooldown(stateMachine.JumpAttackCooldown.ToString(), stateMachine.JumpAttackCooldown);

        if (!stateMachine.CooldownManager.CooldownActive(stateMachine.MissileAttackCooldown.ToString()))
            stateMachine.CooldownManager.BeginCooldown(stateMachine.MissileAttackCooldown.ToString(), stateMachine.MissileAttackCooldown);

        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeduration);
    }

    public override void Tick(float deltaTime)
    {
        if (!IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        else if (HasJumpAttack())
        {
            stateMachine.SwitchState(new EnemyJumpAttackState(stateMachine));
            return;
        }
        else if (HasMissileAttack())
        {
            stateMachine.SwitchState(new EnemyShotMissileState(stateMachine));
            return;
        }
        else if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }

        MoveToPlayer(deltaTime);
        RotateToPlayer(deltaTime);

        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDumpTime, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.Agent.ResetPath();
        stateMachine.Agent.velocity = Vector3.zero;
    }

    private void MoveToPlayer(float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = stateMachine.Player.transform.position;

            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
        }

        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
    }

    private bool IsInAttackRange()
    {
        if (stateMachine.Player.GetComponent<Health>().IsDead) { return false; }

        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;

        return playerDistanceSqr <= stateMachine.AttackRange * stateMachine.AttackRange;
    }
}
