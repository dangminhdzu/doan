using System.Collections.Generic;
using UnityEngine;

public class UsedCards : MonoBehaviour
{
    private List<Cards> usedCards = new List<Cards>(); // Danh sách các lá bài đã sử dụng

    // Hàm để thêm lá bài vào danh sách usedCards
    public void AddCard(Cards card)
    {
        if (card != null)
        {
            usedCards.Add(card);
            Debug.Log("Added card to used cards: " + card.testnum);
        }
        else
        {
            Debug.LogError("Cannot add null card to used cards.");
        }
    }

    // Hàm để lấy tất cả các lá bài đã sử dụng và trả về
    public List<Cards> GetUsedCards()
    {
        if (usedCards.Count > 0)
        {
            Debug.Log("Returning used cards to deck: " + usedCards.Count + " cards.");
            return new List<Cards>(usedCards);  // Trả về bản sao của danh sách
        }
        else
        {
            Debug.LogWarning("Used cards list is empty.");
            return new List<Cards>();  // Trả về danh sách rỗng nếu không có lá bài nào
        }
    }

    // Hàm để xóa sạch danh sách usedCards sau khi các lá bài đã được đưa lại vào deck
    public void ClearUsedCards()
    {
        if (usedCards.Count > 0)
        {
            Debug.Log("Clearing used cards list: " + usedCards.Count + " cards.");
            usedCards.Clear();  // Xóa danh sách các lá bài đã sử dụng
        }
        else
        {
            Debug.LogWarning("No used cards to clear.");
        }
    }

    // Hàm để kiểm tra số lượng bài đã sử dụng
    public int GetUsedCardCount()
    {
        return usedCards.Count; // Trả về số lượng bài trong danh sách usedCards
    }

    // Hàm kiểm tra xem danh sách usedCards có trống không
    public bool HasUsedCards()
    {
        return usedCards.Count > 0; // Trả về true nếu có ít nhất một lá bài trong danh sách
    }
}
