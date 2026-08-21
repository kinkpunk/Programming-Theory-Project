using UnityEngine;
using System.Collections;

public class HeavyLoader : Robot
{
    private void Awake()
    {
        base.Awake();
        MaxCapacity = 3;
        baseSpeed = 2f;
        obstacleAvoidanceDistance = 2.5f;
    }

    protected override IEnumerator MoveTo(Vector3 destination, bool stopAtCollider)
    {
        float currentSpeed = baseSpeed;
        if (CurrentCargo > 0) currentSpeed = baseSpeed * 0.5f;

        Vector3 targetPos = new Vector3(destination.x, transform.position.y, destination.z);
        Vector3 startPos = transform.position;

        while (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            // Проверка склада
            if (stopAtCollider && warehouseCollider != null)
            {
                float distToWarehouse = DistanceToWarehouse();
                if (distToWarehouse < 2f)
                {
                    Debug.Log($"[HEAVY LOADER] {SerialNumber} reached warehouse (distance: {distToWarehouse:F2})");
                    break;
                }
            }

            // Проверка препятствий
            Vector3 direction = (targetPos - transform.position).normalized;
            float obstacleDistance;

            if (CheckObstacle(direction, out obstacleDistance))
            {
                Debug.Log($"[HEAVY LOADER] {SerialNumber} obstacle detected, avoiding...");
                Vector3 avoidDirection = Vector3.Cross(direction, Vector3.up).normalized;
                if (Random.value > 0.5f) avoidDirection = -avoidDirection;

                Vector3 avoidPos = transform.position + avoidDirection * 4f;
                yield return StartCoroutine(MoveStraight(avoidPos, currentSpeed, 1f));
                continue;
            }

            float step = currentSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, step);

            Vector3 moveDirection = targetPos - transform.position;
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            transform.position = newPos;
            yield return null;
        }
    }

    private IEnumerator MoveStraight(Vector3 target, float speed, float maxTime)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < maxTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, target, elapsed / maxTime);

            Vector3 moveDirection = target - transform.position;
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            yield return null;
        }
    }
}
