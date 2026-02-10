using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Card UI Elements")]
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text cardCostText;
    [SerializeField] TMP_Text cardDamageText;
    [SerializeField] Image artworkImage;
    [SerializeField] Button button;

    [Header("Reference")]
    public CardData card;
    public SpeedSlot targetSlot;
    public PlayerActionInput input;
    //private BattleManager battleManager;
    //private Player player;

    public void Setup(CardData data, BattleManager bm)
    {
        card = data;
        //battleManager = bm;

        UpdateUI();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void UpdateUI()
    {
        if (card == null) return;

        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();

        cardDamageText.gameObject.SetActive(false); // 기본값으로 비활성화


        if (artworkImage != null && card.artwork != null)
        {
            artworkImage.sprite = card.artwork;
        }
    }

    private void OnClick()
    {
        input.SelectCard(targetSlot, card);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
