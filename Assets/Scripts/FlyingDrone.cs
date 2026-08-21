using UnityEngine;
using System.Collections;

public class FlyingDrone : Robot
{
    private float flightHeight = 3f;

    private void Awake()
    {
        base.Awake();
        MaxCapacity = 1;
        baseSpeed = 6f;
    }

    protected override IEnumerator MoveTo(Vector3 destination, bool stopAtCollider)
    {
        // Взлёт
        Vector3 ascendPos = new Vector3(transform.position.x, flightHeight, transform.position.z);
        yield return StartCoroutine(MoveStraight(ascendPos));

        // Полёт к цели
        Vector3 targetPos = new Vector3(destination.x, flightHeight, destination.z);
        yield return StartCoroutine(MoveStraight(targetPos));

        // Посадка на склад с проверкой коллайдера
        if (stopAtCollider && warehouseCollider != null)
        {
            float distToWarehouse = DistanceToWarehouse();
            if (distToWarehouse > 1.5f)
            {
                yield return StartCoroutine(MoveStraight(destination));
            }
            else
            {
                Debug.Log($"[DRONE] {SerialNumber} already at warehouse (distance: {distToWarehouse:F2})");
            }
        }
        else
        {
            yield return StartCoroutine(MoveStraight(destination));
        }
    }

    private IEnumerator MoveStraight(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.5f)
        {
            float step = baseSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, target, step);

            Vector3 moveDirection = target - transform.position;
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            transform.position = newPos;
            yield return null;
        }
        transform.position = target;
    }
}
