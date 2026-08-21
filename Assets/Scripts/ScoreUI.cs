using UnityEngine;
using TMPro; // Используем стандартный UI. Если у вас TextMeshPro, см. примечание ниже

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Сюда перетащим текстовый элемент из иерархии

    private int lastKnownScore = 0;

    void Update()
    {
        // Проверяем, изменилось ли значение в Warehouse
        if (Warehouse.TotalDelivered != lastKnownScore)
        {
            lastKnownScore = Warehouse.TotalDelivered;
            scoreText.text = "Delivered Stars: " + lastKnownScore;
        }
    }
}
