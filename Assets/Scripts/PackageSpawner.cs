using UnityEngine;

public class PackageSpawner : MonoBehaviour
{
    public GameObject packagePrefab;
    public float spawnInterval = 3f;
    public float spawnAreaSize = 18f; // Размер области спавна на Plane
    public int maxPackagesOnField = 5;

    private int packageCounter = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPackage), 1f, spawnInterval);
    }

    private void SpawnPackage()
    {
        // Ограничиваем количество посылок на поле
        Package[] currentPackages = FindObjectsOfType<Package>();
        if (currentPackages.Length >= maxPackagesOnField) return;

        packageCounter++;
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f),
            0.5f,
            Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f)
        );

        GameObject pkg = Instantiate(packagePrefab, randomPos, Quaternion.identity);
        Package package = pkg.GetComponent<Package>();
        if (package != null)
        {
            package.PackageId = $"PKG-{packageCounter:D3}";
        }

        Debug.Log($"[SPAWNER] Spawned {package.PackageId} at {randomPos}");
    }
}
