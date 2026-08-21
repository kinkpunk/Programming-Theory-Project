using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wheeledBotPrefab;
    public GameObject flyingDronePrefab;
    public GameObject heavyLoaderPrefab;
    public GameObject warehousePrefab;

    private List<Robot> fleet = new List<Robot>();

    void Start()
    {
        Warehouse.ResetScore();

        // Спавн склада
        if (warehousePrefab != null)
            Instantiate(warehousePrefab, Vector3.zero, Quaternion.identity);

        // Спавн роботов
        SpawnAndInitialize(wheeledBotPrefab, "WB-001", new Vector3(-8, 0, -8));
        SpawnAndInitialize(flyingDronePrefab, "FD-001", new Vector3(8, 0, -8));
        SpawnAndInitialize(heavyLoaderPrefab, "HL-001", new Vector3(0, 0, 8));

        Debug.Log("--- STARTING SHIFT ---");

        // Запускаем доставку
        foreach (Robot robot in fleet)
        {
            robot.StartDelivery();
        }
    }

    private void SpawnAndInitialize(GameObject prefab, string serial, Vector3 position)
    {
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        Robot robot = instance.GetComponent<Robot>();
        if (robot != null)
        {
            robot.Initialize(serial);
            fleet.Add(robot);
        }
    }
}
