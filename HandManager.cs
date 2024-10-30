using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandManager : MonoBehaviour
{
    public List<Image> playerHand; // Danh sách các Image để hiển thị lá bài
    public List<Cards> currentHand; // Danh sách các đối tượng Cards trên tay
    public UsedCards usedCardsManager; // Tham chiếu đến lớp UsedCards
    public Transform handPanel; // Panel để hiển thị lá bài
    public GameObject cardPrefab; // Prefab cho lá bài
    public int maxHandSize = 4; // Kích thước tối đa của tay
    public TextMeshProUGUI handCardCountText; // Tham chiếu tới Text UI để hiển thị số lá bài trên tay
    public Button showHandButton; // Nút hiển thị tất cả lá bài trên tay
    public GameObject showHandPanel; // Panel để hiển thị các lá bài
    public Button closeHandPanelButton; // Nút để tắt handPanel

    private void Awake()
    {
        playerHand = playerHand ?? new List<Image>();
        currentHand = currentHand ?? new List<Cards>();

        // Gán sự kiện cho nút showHandButton
        if (showHandButton != null)
        {
            showHandButton.onClick.AddListener(ShowAllCardsOnScreen);
        }
        else
        {
            Debug.LogWarning("ShowHandButton is not assigned.");
        }

        if (showHandPanel != null)
        {
            showHandPanel.SetActive(false); // Ẩn panel ban đầu
        }
        else
        {
            Debug.LogWarning("ShowHandPanel is not assigned.");
        }

        // Gán sự kiện cho nút để đóng handPanel
        if (closeHandPanelButton != null)
        {
            closeHandPanelButton.onClick.AddListener(HideShowHandPanel);
        }
        else
        {
            Debug.LogWarning("CloseHandPanelButton is not assigned.");
        }
    }

    private void HideShowHandPanel()
    {
        showHandPanel.SetActive(false);
        Debug.Log("Closed show hand panel.");
    }
    // Phương thức hiển thị tất cả các lá bài trong tay lên màn hình
    // Phương thức hiển thị tất cả các lá bài trong tay lên màn hình
    private void ShowAllCardsOnScreen()
    {
        if (showHandPanel == null) return;

        // Xóa các lá bài cũ trên panel
        foreach (Transform child in showHandPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Hiển thị panel và thêm các lá bài
        showHandPanel.SetActive(true);

        foreach (var card in currentHand)
        {
            GameObject cardObject = Instantiate(cardPrefab, showHandPanel.transform);
            Image cardImage = cardObject.GetComponent<Image>();
            cardImage.sprite = card.cardImage;

            // Gán sự kiện khi nhấn vào lá bài để thêm vào usedCards
            Button cardButton = cardObject.GetComponent<Button>();
            if (cardButton == null)
            {
                cardButton = cardObject.AddComponent<Button>();
            }

            // Thêm sự kiện nhấn để chuyển lá bài vào usedCards
            cardButton.onClick.AddListener(() => OnCardClicked(card, cardImage));
        }

        Debug.Log("Displaying all cards on screen.");
    }

    // Phương thức xóa lá bài khỏi Hand và cập nhật UI
    private void RemoveCardFromHand(Cards card, GameObject cardObject)
    {
        if (currentHand.Contains(card))
        {
            currentHand.Remove(card); // Xóa lá bài khỏi danh sách bài trên tay
            playerHand.Remove(cardObject.GetComponent<Image>()); // Xóa hình ảnh lá bài

            Destroy(cardObject); // Hủy GameObject lá bài trên panel

            Debug.Log($"Removed card {card.testnum} from hand. Remaining cards: {currentHand.Count}");
        }
        else
        {
            Debug.LogWarning("Card not found in hand.");
        }
    }


    public void DrawCard(Cards drawnCard)
    {
        if (currentHand.Count < maxHandSize) // Kiểm tra dựa trên currentHand
        {
            GameObject newCard = Instantiate(cardPrefab, handPanel);
            Image cardImage = newCard.GetComponent<Image>();
            cardImage.sprite = drawnCard.cardImage;

            playerHand.Add(cardImage); // Thêm vào UI hiển thị
            currentHand.Add(drawnCard); // Thêm vào danh sách bài trên tay

            // Gán sự kiện xử lý cho lá bài
            CardClickHandler cardClickHandler = newCard.AddComponent<CardClickHandler>();
            cardClickHandler.handManager = this;
            cardClickHandler.card = drawnCard;

            // Thêm Button và gán sự kiện nhấn
            Button cardButton = newCard.GetComponent<Button>();
            if (cardButton == null)
            {
                cardButton = newCard.AddComponent<Button>();
            }
            cardButton.onClick.AddListener(cardClickHandler.OnCardClicked);

            Debug.Log($"Card drawn: {drawnCard.testnum}. Total cards in hand: {currentHand.Count}");
        }
        else
        {
            Debug.LogWarning("Hand is full, moving card to usedCards.");
            usedCardsManager.AddCard(drawnCard);
        }
        UpdateHandCardCount(); // Cập nhật UI sau khi rút bài
    }
    // Phương thức xử lý khi nhấn vào lá bài
    public void OnCardClicked(Cards card, Image cardImage)
    {
        if (currentHand.Contains(card))
        {
            card.isUsed = true; // Đánh dấu lá bài là đã sử dụng
            usedCardsManager.AddCard(card); // Thêm lá bài vào danh sách usedCards

            // Xóa lá bài khỏi UI
            Destroy(cardImage.gameObject); // Hủy GameObject của lá bài trong `showHandPanel`

            // Cập nhật danh sách bài trên tay
            AddCardToUsedCards(card, cardImage.gameObject);

            Debug.Log($"Card used: {card.testnum}. Total used cards: {usedCardsManager.GetUsedCardCount()}");
        }
        else
        {
            Debug.LogError("Card not found in hand!");
        }
    }



    // Hàm cập nhật số lượng lá bài trong tay và hiển thị lên UI
    private void UpdateHandCardCount()
    {
        if (handCardCountText != null)
        {
            handCardCountText.text = currentHand.Count.ToString();
        }
        else
        {
            Debug.LogWarning("handCardCountText is not assigned.");
        }
    }

    private void RemoveCard(Cards card)
    {
        int index = currentHand.IndexOf(card);
        if (index >= 0)
        {
            // Xóa lá bài khỏi danh sách hiện tại
            currentHand.RemoveAt(index);

            // Xóa hình ảnh của lá bài (có thể cần tham chiếu đến playerHand hoặc UI)
            // Giả sử bạn đã lưu hình ảnh trong playerHand tương ứng với currentHand
            Image cardImage = playerHand[index];
            playerHand.RemoveAt(index);
            Destroy(cardImage.gameObject); // Hủy GameObject của lá bài

            // Cập nhật lại số lượng lá bài trên tay
            UpdateHandCardCount();

            Debug.Log($"Removed card {card.testnum} from hand. Remaining cards: {currentHand.Count}");
        }
        else
        {
            Debug.LogError("Card not found in hand for removal!");
        }
    }


    // Phương thức thêm lá bài vào UsedCards và cập nhật UI
    public void AddCardToUsedCards(Cards card, GameObject cardObject)
    {
        if (currentHand.Contains(card))
        {
            usedCardsManager.AddCard(card); // Thêm vào danh sách usedCards
            RemoveCardFromHand(card, cardObject); // Xóa lá bài khỏi tay và UI
            if (currentHand.Count == 0)
            {
                ClearHand(); // Xóa sạch UI nếu tay không còn lá bài nào
            }

            Debug.Log($"Added card {card.testnum} to used cards. Remaining cards: {currentHand.Count}");

            // Cập nhật lại số lượng lá bài trong tay
            UpdateHandCardCount();
        }
        else
        {
            Debug.LogWarning("Card not found in hand.");
        }
    }



    public void ClearHand()
    {
        // Xóa tất cả các hình ảnh của lá bài trong danh sách playerHand
        foreach (Image cardImage in playerHand)
        {
            Destroy(cardImage.gameObject);
        }
        playerHand.Clear(); // Xóa danh sách hình ảnh lá bài
        currentHand.Clear(); // Xóa danh sách đối tượng bài

        // Đảm bảo xóa toàn bộ hình ảnh trên handPanel
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        UpdateHandCardCount(); // Cập nhật lại UI sau khi xóa
        Debug.Log($"Hand cleared. Total cards in hand: {playerHand.Count}");
    }


    public int GetCurrentHandSize()
    {
        return playerHand.Count;
    }

    public List<Cards> cardsInHand = new List<Cards>(); // Danh sách các lá bài hiện có trên tay người chơi

    public void AddCardToHand(Cards card)
    {
        cardsInHand.Add(card); // Thêm lá bài vào danh sách
    }

    public void RemoveCardFromHand(Cards card)
    {
        cardsInHand.Remove(card); // Loại bỏ lá bài khỏi danh sách khi nó đã được sử dụng
    }

}
