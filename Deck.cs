using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Cards> cardsInDeck = new List<Cards>(); // Danh sách chứa các lá bài trong deck
    private List<Cards> usedCards = new List<Cards>(); // Danh sách chứa các lá bài đã sử dụng

    private void Start()
    {
        ShuffleDeck(); // Gọi hàm xáo trộn bộ bài khi bắt đầu
    }

    // Xáo trộn deck
    public void ShuffleDeck()
    {
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            Cards temp = cardsInDeck[i];
            int randomIndex = Random.Range(i, cardsInDeck.Count);
            cardsInDeck[i] = cardsInDeck[randomIndex];
            cardsInDeck[randomIndex] = temp;
        }
    }

    // Rút một lá bài từ deck
    public Cards DrawCard()
    {
        if (cardsInDeck.Count > 0)
        {
            Cards drawnCard = cardsInDeck[0];  // Rút lá bài đầu tiên
            drawnCard.isUsed = true; // Đánh dấu lá bài là đã sử dụng
            cardsInDeck.RemoveAt(0);  // Loại bỏ lá bài khỏi bộ bài
            usedCards.Add(drawnCard); // Thêm lá bài vào danh sách đã sử dụng
            return drawnCard;
        }
        else
        {
            Debug.LogWarning("Deck is empty!");
            return null;  // Trả về null nếu bộ bài rỗng
        }
    }

    // Trả tất cả bài đã sử dụng trở lại deck và xáo trộn
    public void ReturnUsedCardsToDeck()
    {
        if (usedCards.Count > 0)
        {
            foreach (Cards card in usedCards)
            {
                card.isUsed = false; // Đặt lại trạng thái là chưa sử dụng
                cardsInDeck.Add(card); // Thêm lá bài đã sử dụng trở lại deck
            }
            usedCards.Clear(); // Xóa danh sách các lá bài đã sử dụng
            ShuffleDeck(); // Xáo trộn lại deck
        }
        else
        {
            Debug.Log("No used cards to return.");
        }
    }

    // Thêm một lá bài mới vào deck
    public void AddCard(Cards newCard)
    {
        cardsInDeck.Add(newCard);
    }

    // Lấy số lượng bài còn lại trong deck
    public int GetRemainingCards()
    {
        return cardsInDeck.Count;
    }

    // Lấy số lượng bài đã sử dụng
    public int GetUsedCardCount()
    {
        return usedCards.Count;
    }

    // Lấy danh sách các lá bài đã sử dụng
    public List<Cards> GetUsedCards()
    {
        return new List<Cards>(usedCards); // Trả về bản sao danh sách usedCards
    }
}
