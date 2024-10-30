using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Đảm bảo rằng bạn đã import namespace này

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Singleton để dễ dàng truy cập
    public Pathfinding pathfinding;  // Tham chiếu đến đối tượng Pathfinding
    public GameObject player;  // Tham chiếu đến nhân vật (player)
    public float moveSpeed = 3f;  // Tốc độ di chuyển
    public TextMeshProUGUI diceRes;  // Tham chiếu đến TextMeshPro để hiển thị kết quả tung xúc xắc
    
    public Tile currentTile;
    private List<Tile> possibleDestinations = new List<Tile>();  // Danh sách các ô có thể đến


    public GameObject pointerPrefab;  // Prefab của con trỏ
    private List<GameObject> activePointers = new List<GameObject>(); // Danh sách các con trỏ đang hiển thị
    private SpecialTileEventHandler specialTileEventHandler = new SpecialTileEventHandler();





    public Deck deck; // Tham chiếu đến đối tượng Deck
    public Button drawCardButton; //nút draw
    public int maxmaxHandSize = 6;
    public HandManager handManager;
    public UsedCards usedCardsManager;

    private void Start() {

        currentTile = allTiles[0];
        if (deck == null)
        {
            Debug.LogError("Deck is not assigned in the Inspector.");
            return; // Ngăn không cho thực hiện nếu deck không được gán
        }
        // Thêm listener cho nút
        if (drawCardButton != null)
        {
            drawCardButton.onClick.AddListener(PlayerDrawCard);
        }
        else
        {
            Debug.LogError("drawCardButton is not assigned in the Inspector.");
        }

        DrawStartingHand();

    }// Ô hiện tại của nhân vật
    private void Update()
    {
        // Cập nhật ô hiện tại dựa vào vị trí nhân vật
        currentTile = GetCurrentTile();
    }

    public List<Tile> allTiles;
    private Tile GetCurrentTile()
    {
        float minDistance = Mathf.Infinity;
        Tile nearestTile = null;

        foreach (Tile tile in allTiles)
        {
            float distance = Vector3.Distance(player.transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        return nearestTile;  // Trả về ô gần nhất với nhân vật
    }
    private void Awake()
    {
        // Đảm bảo chỉ có một GameManager duy nhất
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int diceValue = 0;
    public void OnRollDiceButtonClicked()
    {

        diceValue = Random.Range(1, 7);
        diceRes.text = "" + diceValue;
        Debug.Log("You rolled: " + diceValue);

        currentTile = GetCurrentTile();

        if (currentTile.GetSpecialTileType() == SpecialTileType.AtSchool)
        {
            if (diceValue == 5 || diceValue == 6) // Nếu tung ra 5 hoặc 6
            {
                // Di chuyển đến ô Graduate
                Tile targetTile = currentTile.targetTile; // Hàm để tìm ô Graduate
                if (targetTile != null)
                {
                    // Cập nhật vị trí của người chơi khi đến ô mục tiêu
                    player.transform.position = targetTile.transform.position + new Vector3(0f, 1f / 2, 0f);
                    GameManager.Instance.currentTile = targetTile;

                    // Tạm dừng một khoảng thời gian ngắn nếu cần
                    // (Có thể dùng phương pháp khác như gọi một hàm khác để xử lý tạm dừng nếu cần)
                }
                return; // Kết thúc phương thức nếu đã di chuyển đến Graduate
            }

        }

        else { 
    
        
        currentTile = GetCurrentTile();

        ClearActivePointers();
        possibleDestinations = pathfinding.GetPossibleDestinations(currentTile, diceValue);

        foreach (Tile destination in possibleDestinations)
                    {

            // Tạo một con trỏ tại mỗi ô đích
            GameObject newPointer = Instantiate(pointerPrefab, destination.transform.position, Quaternion.identity);
            newPointer.GetComponent<PointerBounce>().StartBouncing();  // Bắt đầu hiệu ứng nhấp nhô
            activePointers.Add(newPointer);  // Thêm con trỏ vào danh sách
        }
        }
    }

    private void ClearActivePointers()
    {
        foreach (GameObject pointer in activePointers)
        {
            Destroy(pointer);  // Hủy con trỏ
        }
        activePointers.Clear();  // Xóa danh sách con trỏ
    }




    public void SelectDestinationTile(Tile destinationTile)
    {
        if (possibleDestinations.Contains(destinationTile))
        {
            MoveToTile(destinationTile); // Di chuyển tới ô đã chọn
            ClearActivePointers(); // Xóa các con trỏ khi ô đã chọn
            ResetDestinationColors(); // Đặt lại màu cho ô
        }
        else
        {
            Debug.Log("Selected tile is not a valid destination.");
        }
    }


    public void MoveToTile(Tile destinationTile)
    {
        if (destinationTile != currentTile)  // Kiểm tra nếu ô đích khác ô hiện tại
        {
            // Đảm bảo rằng bạn có đường đi hợp lệ
            List<Tile> path = pathfinding.GetPath(currentTile, destinationTile, diceValue);
            if (path.Count > 0)
            {
                player.GetComponent<PlayerMovement>().SetPath(path);  // Đặt đường đi trong PlayerMovement
                currentTile = destinationTile;  // Cập nhật currentTile sau khi di chuyển
            }
            else
            {
                Debug.Log("No path found to the destination.");
            }
        }
    }


    private void ResetDestinationColors()
    {
        foreach (Tile tile in possibleDestinations)
        {
            tile.GetComponent<Renderer>().material.color = Color.white; // Đặt lại màu ô
        }
        possibleDestinations.Clear(); // Xóa danh sách ô có thể đến sau khi chọn
    }


    private IEnumerator UpdateCurrentTileAfterMove(Tile destinationTile)
    {
        // Đợi cho đến khi nhân vật di chuyển xong
        while (player.GetComponent<PlayerMovement>().IsMoving)
        {
            yield return null;  // Đợi một frame
        }

        // Cập nhật currentTile sau khi di chuyển xong
        currentTile = destinationTile;
    }

    public void PlayerDrawCard()
    {
        if (deck == null)
        {
            Debug.LogError("Deck is null when trying to draw a card!");
            return;
        }

        Cards drawnCard = deck.DrawCard();

        if (usedCardsManager == null)
        {
            Debug.LogError("UsedCardsManager is null!");
            return;
        }

        if (drawnCard == null && usedCardsManager.GetUsedCardCount() > 0)
        {
            RefillDeckFromUsedCards();
            drawnCard = deck.DrawCard(); // Rút lại bài sau khi bổ sung từ usedCards
        }

        if (drawnCard != null)
        {
            Debug.Log("Drawn card: " + drawnCard.testnum);
            handManager.DrawCard(drawnCard); // Gọi hàm DrawCard của HandManager
        }
        else
        {
            Debug.LogError("No card was drawn, deck and usedCards are empty.");
        }
    }

    private void RefillDeckFromUsedCards()
    {
        Debug.Log("Refilling deck from used cards...");
        List<Cards> usedCards = usedCardsManager.GetUsedCards();  // Lấy các lá bài đã sử dụng
        if (usedCards.Count > 0)
        {
            deck.cardsInDeck.AddRange(usedCards);  // Thêm vào deck
            usedCardsManager.ClearUsedCards();  // Xóa danh sách usedCards sau khi bổ sung vào deck
            deck.ShuffleDeck();  // Xáo trộn lại deck
            Debug.Log("Deck refilled with " + usedCards.Count + " cards.");
        }
        else
        {
            Debug.LogWarning("No used cards to refill the deck.");
        }
    }
    private void DrawStartingHand()
    {
        for (int i = 0; i < 6; i++)
        {
            Cards drawnCard = deck.DrawCard(); // Rút một lá bài từ deck
            if (drawnCard != null)
            {
                handManager.DrawCard(drawnCard); // Thêm lá bài vào tay
                Debug.Log("Drawn starting card: " + drawnCard.testnum);
            }
            else
            {
                Debug.LogWarning("No more cards to draw for starting hand.");
            }
        }

    }





}
