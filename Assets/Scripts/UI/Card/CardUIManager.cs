using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardParent;
    [SerializeField] BattleManager battleManager;

    private void Start()
    {
        ValidateReferences();

        if (player != null)
        {
            player.OnHandChanged += RefreshUI;
            RefreshUI();
        }
    }

    private void ValidateReferences()
    {
        if (player == null)
            Debug.LogError("Player reference is missing in CardUIManager.");
        if (enemy == null)
            Debug.LogError("Enemy reference is missing in CardUIManager.");
        if (cardPrefab == null)
            Debug.LogError("Card Prefab reference is missing in CardUIManager.");
        if (cardParent == null)
            Debug.LogError("Card Parent reference is missing in CardUIManager.");
    }

    private void RefreshUI()
    {
        ClearCards();
        DisplayCards();
    }

    private void ClearCards() // 카드 UI 초기화
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayCards() // 초기화된 카드 패널에 카드 UI 다시 생성
    {
        if (player == null || player.hand == null) return;

        foreach (var card in player.hand)
        {
            CreateCardUI(card);
        }
    }

    private void CreateCardUI(CardData card) // 카드 UI 정보 설정 요청 -----> CardUI.cs의 Setup() 호출
    {
        GameObject cardObj = Instantiate(cardPrefab, cardParent); // cardParent 아래에 카드 프리팹 복제 생성
        CardUI cardUI = cardObj.GetComponent<CardUI>(); // 카드 프리팹에 붙어있는 CardUI 컴포넌트 가져오기

        if (cardUI != null)
        {
            cardUI.Setup(card, battleManager);
        }
        else
        {
            Debug.LogError("CardUI component is missing on the card prefab.");
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHandChanged -= RefreshUI;
        }
    }

}
