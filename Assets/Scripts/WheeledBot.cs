using UnityEngine;
using System.Collections;

public class WheeledBot : Robot
{
    private void Awake()
    {
        base.Awake();
        MaxCapacity = 1;
        baseSpeed = 4f;
        obstacleAvoidanceDistance = 2f;
    }

    protected override IEnumerator MoveTo(Vector3 destination, bool stopAtCollider)
    {
        Vector3 targetPos = new Vector3(destination.x, transform.position.y, destination.z);
        Vector3 startPos = transform.position;

        while (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            // Проверка: если это склад и мы близко - останавливаемся
            if (stopAtCollider && warehouseCollider != null)
            {
                float distToWarehouse = DistanceToWarehouse();
                if (distToWarehouse < 1.5f)
                {
                    Debug.Log($"[WHEELED BOT] {SerialNumber} reached warehouse (distance: {distToWarehouse:F2})");
                    break;
                }
            }

            // Проверка препятствий
            Vector3 direction = (targetPos - transform.position).normalized;
            float obstacleDistance;

            if (CheckObstacle(direction, out obstacleDistance))
            {
                Debug.Log($"[WHEELED BOT] {SerialNumber} obstacle detected at {obstacleDistance:F2}m, avoiding...");
                // Объезд: поворачиваем вправо или влево
                Vector3 avoidDirection = Vector3.Cross(direction, Vector3.up).normalized;
                if (Random.value > 0.5f) avoidDirection = -avoidDirection;

                Vector3 avoidPos = transform.position + avoidDirection * 3f;
                yield return StartCoroutine(MoveStraight(avoidPos, 0.5f));
                continue;
            }

            // Движение к цели
            float step = baseSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, step);

            // Поворот к цели
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

    private IEnumerator MoveStraight(Vector3 target, float maxTime)
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
