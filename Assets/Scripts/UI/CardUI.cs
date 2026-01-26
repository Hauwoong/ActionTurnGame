using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Card UI Elements")]
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text cardCostText;
    [SerializeField] TMP_Text cardDamageText;
    [SerializeField] Button button;

    [Header("Reference")]
    private CardData card;
    private Player player;  
    private Enemy enemy;

    public void Setup(CardData data, Player p, Enemy e)
    {
        card = data;
        player = p;
        enemy = e;

        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();
        cardDamageText.text = card.damage.ToString();

        button.onClick.AddListener(UseCard);
    }

    void UseCard()
    {
        card.Use(player, enemy);
        Destroy(gameObject);
    }
}
