using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    void Update()
    {
        // Показать все активные посылки
        Package[] packages = FindObjectsOfType<Package>();
        int activeCount = 0;
        foreach (Package p in packages)
        {
            if (p.gameObject.activeSelf) activeCount++;
        }

        // Показать склад
        Warehouse warehouse = FindObjectOfType<Warehouse>();
        if (warehouse == null)
        {
            Debug.LogError("WAREHOUSE NOT FOUND!");
        }
        else
        {
            Debug.Log($"Active packages: {activeCount}, Warehouse at: {warehouse.transform.position}");
        }
    }
}
