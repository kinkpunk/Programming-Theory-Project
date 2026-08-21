using UnityEngine;

public class Package : MonoBehaviour
{
    public string PackageId;
    public int Weight = 1;

    // Визуальное вращение звезды для наглядности
    private void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}
