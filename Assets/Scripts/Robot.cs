using UnityEngine;
using System.Collections;

public abstract class Robot : MonoBehaviour
{
    [SerializeField] protected float baseSpeed = 3f;
    [SerializeField] protected float obstacleAvoidanceDistance = 1.5f;
    [SerializeField] protected LayerMask obstacleLayer;

    public string SerialNumber { get; private set; }
    public int MaxCapacity { get; protected set; } = 1;
    protected int CurrentCargo { get; private set; } = 0;
    protected Package HeldPackage { get; private set; }

    protected Collider warehouseCollider;

    protected virtual void Awake()
    {
        // Находим склад для проверки коллизий
        Warehouse warehouse = FindObjectOfType<Warehouse>();
        if (warehouse != null)
        {
            warehouseCollider = warehouse.GetComponent<Collider>();
        }
    }

    public void Initialize(string serial)
    {
        SerialNumber = serial;
    }

    public void StartDelivery()
    {
        StartCoroutine(DeliveryRoutine());
    }

    private IEnumerator DeliveryRoutine()
    {
        while (true)
        {
            Package nearest = FindNearestPackage();

            if (nearest == null)
            {
                Debug.Log($"[ROBOT] {SerialNumber} waiting for packages...");
                yield return new WaitForSeconds(1f);
                continue;
            }

            Debug.Log($"[ROBOT] {SerialNumber} moving to package {nearest.PackageId}");
            yield return StartCoroutine(MoveTo(nearest.transform.position, false));

            if (nearest == null || !nearest.gameObject.activeSelf)
            {
                Debug.Log($"[ROBOT] {SerialNumber} package was taken. Searching for new one...");
                continue;
            }

            if (Pickup(nearest))
            {
                nearest.gameObject.SetActive(false);
                Debug.Log($"[ROBOT] {SerialNumber} carrying {HeldPackage.PackageId} to warehouse");
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            Warehouse warehouse = FindObjectOfType<Warehouse>();
            if (warehouse != null)
            {
                Debug.Log($"[ROBOT] {SerialNumber} moving to warehouse");
                // При движении к складу останавливаемся у коллайдера
                yield return StartCoroutine(MoveTo(warehouse.transform.position, true));

                warehouse.SubmitPackage(HeldPackage);
                HeldPackage = null;
                CurrentCargo = 0;
                Debug.Log($"[ROBOT] {SerialNumber} delivered package");
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected Package FindNearestPackage()
    {
        Package[] packages = FindObjectsOfType<Package>();
        if (packages.Length == 0) return null;

        Package nearest = null;
        float minDist = float.MaxValue;

        foreach (Package p in packages)
        {
            if (!p.gameObject.activeSelf) continue;

            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }

    protected abstract IEnumerator MoveTo(Vector3 destination, bool stopAtCollider);

    public bool Pickup()
    {
        Package nearest = FindNearestPackage();
        if (nearest != null) return Pickup(nearest);
        return false;
    }

    public virtual bool Pickup(Package target)
    {
        if (target == null) return false;
        if (CurrentCargo < MaxCapacity)
        {
            HeldPackage = target;
            CurrentCargo++;
            Debug.Log($"[ROBOT] {SerialNumber} picked up {target.PackageId}");
            return true;
        }
        Debug.LogWarning($"[ROBOT] {SerialNumber} cargo full!");
        return false;
    }

    // Проверка препятствий через Raycast
    protected bool CheckObstacle(Vector3 direction, out float distance)
    {
        RaycastHit hit;
        distance = 0f;

        if (Physics.Raycast(transform.position, direction, out hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            distance = hit.distance;
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            return true;
        }

        Debug.DrawRay(transform.position, direction * obstacleAvoidanceDistance, Color.green);
        return false;
    }

    // Проверка расстояния до склада
    protected float DistanceToWarehouse()
    {
        if (warehouseCollider == null) return float.MaxValue;

        Vector3 closestPoint = warehouseCollider.ClosestPoint(transform.position);
        return Vector3.Distance(transform.position, closestPoint);
    }
}
