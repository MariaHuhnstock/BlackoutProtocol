using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridPathMover))]
public class EnemyController : MonoBehaviour, IDamageable
{
    private enum State { Searching, Moving, Attacking, Dead }

    private static readonly List<EnemyController> active = new List<EnemyController>();
    public static IReadOnlyList<EnemyController> Active => active;
    public static event Action<EnemyController> OnEnemyKilled;

    [SerializeField] private EnemyData data;
    [SerializeField] private float retryDelay = 0.5f;

    private GridPathMover mover;
    private StructureHealth target;
    private State state;
    private float currentHealth;
    private float attackTimer;
    private float retryTimer;

    public EnemyData Data => data;
    public bool IsDead => state == State.Dead;

    private void Awake()
    {
        mover = GetComponent<GridPathMover>();
        mover.MoveSpeed = data.moveSpeed;
        currentHealth = data.maxHealth;

        mover.OnPathCompleted += HandlePathCompleted;
        mover.OnPathBlocked += HandlePathBlocked;
    }

    private void OnEnable() => active.Add(this);
    private void OnDisable() => active.Remove(this);

    private void Start() => ChangeState(State.Searching);

    private void Update()
    {
        switch (state)
        {
            case State.Searching: UpdateSearching(); break;
            case State.Moving: UpdateMoving(); break;
            case State.Attacking: UpdateAttacking(); break;
        }
    }

    // ── Zustände / States ────────────────────────────────

    private void UpdateSearching()
    {
        retryTimer -= Time.deltaTime;
        if (retryTimer > 0f) return;
        retryTimer = retryDelay;

        target = data.targeting.FindTarget(transform.position);
        if (target == null) return;

        if (IsAdjacentToTarget())
        {
            ChangeState(State.Attacking);
            return;
        }

        if (mover.MoveTo(target.GridPosition, stopBeforeTarget: true))
            ChangeState(State.Moving);
    }

    private void UpdateMoving()
    {
        // Ziel unterwegs zerstört? / Target destroyed on the way?
        if (target == null || target.IsDestroyed)
        {
            mover.Stop();
            ChangeState(State.Searching);
        }
    }

    private void UpdateAttacking()
    {
        if (target == null || target.IsDestroyed || !IsAdjacentToTarget())
        {
            ChangeState(State.Searching);
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            target.TakeDamage(data.attackDamage);
            attackTimer = data.attackInterval;
        }
    }

    private void ChangeState(State newState)
    {
        state = newState;

        if (newState == State.Searching) retryTimer = 0f;
        if (newState == State.Attacking) attackTimer = data.attackInterval * 0.5f;
    }

    // ── Events ───────────────────────────────────────────

    private void HandlePathCompleted()
    {
        ChangeState(target != null && IsAdjacentToTarget() ? State.Attacking : State.Searching);
    }

    private void HandlePathBlocked() => ChangeState(State.Searching);

    // ── Schaden / Damage ────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (state == State.Dead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        state = State.Dead;
        mover.Stop();
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }

    // ── Hilfsmethoden / Helpers ─────────────────────────

    private bool IsAdjacentToTarget()
    {
        Vector2Int myPos = GridManager.Instance.GetGridPosition(transform.position);
        Vector2Int diff = target.GridPosition - myPos;
        return Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}