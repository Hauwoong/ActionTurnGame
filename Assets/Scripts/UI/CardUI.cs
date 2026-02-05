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
    private CardData cardData;
    private BattleManager battleManager;
    private Player player;

    public void Setup(CardData data, BattleManager bm)
    {
        cardData = data;
        battleManager = bm;

        UpdateUI();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void UpdateUI()
    {
        if (cardData == null) return;

        cardNameText.text = cardData.cardName;
        cardCostText.text = cardData.cost.ToString();

        cardDamageText.gameObject.SetActive(false); // 기본값으로 비활성화


        if (artworkImage != null && cardData.artwork != null)
        {
            artworkImage.sprite = cardData.artwork;
        }
    }

    private void OnClick()
    {

    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
