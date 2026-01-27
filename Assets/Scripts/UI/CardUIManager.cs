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

    private void ClearCards()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayCards()
    {
        if (player == null || player.hand == null) return;

        foreach (var card in player.hand)
        {
            CreateCardUI(card);
        }
    }

    private void CreateCardUI(CardData card)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardParent);
        CardUI cardUI = cardObj.GetComponent<CardUI>();

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
