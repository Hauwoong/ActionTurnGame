using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [Header("Card UI Elements")]
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text cardCostText;
    [SerializeField] TMP_Text cardDamageText;
    [SerializeField] Image artworkImage;
    [SerializeField] Button button;

    public CardData card;
    public PlayerActionInput input;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector3 originalPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(CardData data, PlayerActionInput input)
    {
        this.card = data;
        this.input = input;
        
        UpdateUI();
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!input.HasSelectedSlot()) return;

        originalPos = rect.position;
        canvasGroup.blocksRaycasts = false;
        input.StartDraggingCard(card);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rect.position = originalPos;

        input.EndDraggingCard();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
