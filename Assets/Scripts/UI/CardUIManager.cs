using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardParent;

    private void Start()
    {
        if (player == null || enemy == null || cardPrefab == null || cardParent == null)
        {
            Debug.LogError("CardUIManager: Missing References.");
        }

        player.OnHandChanged += RefreshUI;
    }

    void RefreshUI()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in player.hand)
        {
            GameObject obj = Instantiate(cardPrefab, cardParent);

            CardUI ui = obj.GetComponent<CardUI>();
            ui.Setup(card, player, enemy);
        }
    }
}
