using UnityEngine;
using UnityEngine.UI;

public class CardClickHandler : MonoBehaviour
{
    public HandManager handManager;  // Tham chiếu đến HandManager
    public Cards card; // Tham chiếu đến lá bài cụ thể

    // Hàm này được gọi khi lá bài bị nhấn
    public void OnCardClicked()
    {
        // Kiểm tra nếu có handManager và card
        if (handManager != null && card != null)
        {
            // Gọi hàm để thêm lá bài vào danh sách usedCards và truyền cả cardObject
            handManager.AddCardToUsedCards(card, gameObject); // `gameObject` là đối tượng lá bài này
        }
        else
        {
            if (handManager == null)
            {
                Debug.LogError("HandManager is not assigned!");
            }
            if (card == null)
            {
                Debug.LogError("Card is not assigned!");
            }
        }
    }
}
