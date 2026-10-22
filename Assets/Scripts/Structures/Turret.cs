using UnityEngine;

public class Turret : PoweredStructure
{
    [Header("Turret")]
    [Tooltip("Optional: dreht sich zum Ziel / Optional: rotates towards target")]
    [SerializeField] private Transform barrelPivot;
    [SerializeField] private LineRenderer beam;
    [SerializeField] private float beamDuration = 0.06f;

    private TurretData turretData;
    private EnemyController currentTarget;
    private float fireTimer;
    private float beamTimer;

    protected override void Awake()
    {
        base.Awake();

        turretData = data as TurretData;
        if (turretData == null)
            Debug.LogError($"{name}: Data muss ein TurretData-Asset sein / Data must be a TurretData asset");

        if (beam != null) beam.enabled = false;
    }

    private void Update()
    {
        UpdateBeam();

        if (!IsPowered || turretData == null)
        {
            currentTarget = null;
            return;
        }

        if (!IsValidTarget(currentTarget))
            currentTarget = FindTarget();

        fireTimer = Mathf.Max(0f, fireTimer - Time.deltaTime);

        if (currentTarget == null) return;

        AimAt(currentTarget.transform.position);

        if (fireTimer <= 0f)
        {
            Fire();
            fireTimer = turretData.fireInterval;
        }
    }

    private bool IsValidTarget(EnemyController enemy)
    {
        if (enemy == null || enemy.IsDead) return false;

        Vector3 pos = enemy.transform.position;
        if ((pos - transform.position).sqrMagnitude > turretData.range * turretData.range) return false;

        return LightSource.IsPositionLit(pos);
    }

    private EnemyController FindTarget()
    {
        EnemyController best = null;
        float bestDistance = float.MaxValue;

        foreach (EnemyController enemy in EnemyController.Active)
        {
            if (!IsValidTarget(enemy)) continue;

            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy;
            }
        }

        return best;
    }

    private void AimAt(Vector3 targetPos)
    {
        if (barrelPivot == null) return;

        Vector2 dir = targetPos - barrelPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        barrelPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Fire()
    {
        Vector3 start = barrelPivot != null ? barrelPivot.position : transform.position;
        Vector3 end = currentTarget.transform.position;

        currentTarget.TakeDamage(turretData.damage);

        if (beam != null)
        {
            beam.SetPosition(0, start);
            beam.SetPosition(1, end);
            beam.enabled = true;
            beamTimer = beamDuration;
        }
    }

    private void UpdateBeam()
    {
        if (beamTimer <= 0f) return;

        beamTimer -= Time.deltaTime;
        if (beamTimer <= 0f && beam != null)
            beam.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        TurretData td = data as TurretData;
        if (td == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, td.range);
    }
}