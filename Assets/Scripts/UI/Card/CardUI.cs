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

    public CardData card;
    public PlayerActionInput input;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPos;
    private Transform originalParent;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
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

        input.StartDraggingCard(card);

        originalPos = rect.position;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(originalParent);
        rect.position = originalPos;

        input.EndDraggingCard();
    }
}
