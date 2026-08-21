using UnityEngine;

public class Warehouse : MonoBehaviour
{
    public static int TotalDelivered { get; private set; } = 0;

    public void SubmitPackage(Package package)
    {
        if (package == null) return;

        TotalDelivered++;
        Debug.Log($"[WAREHOUSE] Received {package.PackageId}. Total: {TotalDelivered}");

        // Посылка исчезает
        Destroy(package.gameObject);
    }

    public static void ResetScore()
    {
        TotalDelivered = 0;
    }
}
