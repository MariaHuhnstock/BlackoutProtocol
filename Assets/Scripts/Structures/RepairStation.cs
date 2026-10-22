using UnityEngine;

public class RepairStation : PoweredStructure
{
    [Header("Repair")]
    [SerializeField] private Transform drone;
    [SerializeField] private LineRenderer repairBeam;

    private RepairData repairData;
    private StructureHealth selfHealth;
    private StructureHealth target;
    private Vector3 homePosition;
    private float repairTimer;

    protected override void Awake()
    {
        base.Awake();

        repairData = data as RepairData;
        if (repairData == null)
            Debug.LogError($"{name}: Data muss ein RepairData-Asset sein / Data must be a RepairData asset");

        selfHealth = GetComponent<StructureHealth>();
        if (repairBeam != null) repairBeam.enabled = false;
    }

    protected override void Start()
    {
        base.Start();

        // Nach GridOccupant.Start steht die Station auf der Zellenmitte / Station is snapped to the cell by now
        homePosition = transform.position;
        if (drone != null) drone.position = homePosition;
    }

    private void Update()
    {
        if (drone == null || repairData == null) return;

        if (!IsPowered)
        {
            target = null;
            SetBeam(false);
            MoveDroneTowards(homePosition);
            return;
        }

        if (!IsValidTarget(target))
        {
            target = FindTarget();
            repairTimer = repairData.repairInterval;
        }

        if (target == null)
        {
            SetBeam(false);
            MoveDroneTowards(homePosition);
            return;
        }

        Vector3 hoverPoint = target.transform.position + Vector3.up * repairData.hoverHeight;
        if (!MoveDroneTowards(hoverPoint))
        {
            SetBeam(false);
            return;
        }

        SetBeam(true);

        repairTimer -= Time.deltaTime;
        if (repairTimer <= 0f)
        {
            target.Repair(repairData.repairAmount);
            repairTimer = repairData.repairInterval;
        }
    }

    private bool IsValidTarget(StructureHealth structure)
    {
        if (structure == null || structure.IsDestroyed) return false;
        if (structure == selfHealth) return false;
        if (structure.CurrentHealth >= structure.MaxHealth) return false;

        float range = repairData.range;
        return (structure.transform.position - homePosition).sqrMagnitude <= range * range;
    }

    // Wählt das Gebäude mit dem niedrigsten Lebensanteil / Picks the structure with the lowest health ratio
    private StructureHealth FindTarget()
    {
        StructureHealth best = null;
        float lowestRatio = float.MaxValue;

        foreach (StructureHealth structure in StructureHealth.All)
        {
            if (!IsValidTarget(structure)) continue;

            float ratio = structure.CurrentHealth / structure.MaxHealth;
            if (ratio < lowestRatio)
            {
                lowestRatio = ratio;
                best = structure;
            }
        }

        return best;
    }

    // Gibt true zurück, wenn die Drohne angekommen ist / Returns true when the drone has arrived
    private bool MoveDroneTowards(Vector3 destination)
    {
        drone.position = Vector3.MoveTowards(drone.position, destination, repairData.droneSpeed * Time.deltaTime);
        return (drone.position - destination).sqrMagnitude < 0.0025f;
    }

    private void SetBeam(bool active)
    {
        if (repairBeam == null) return;

        repairBeam.enabled = active;
        if (active)
        {
            repairBeam.SetPosition(0, drone.position);
            repairBeam.SetPosition(1, target.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        RepairData rd = data as RepairData;
        if (rd == null) return;

        Gizmos.color = new Color(0.3f, 1f, 0.5f, 0.6f);
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, rd.range);
    }
}