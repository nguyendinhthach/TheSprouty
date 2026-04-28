using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private UnityEngine.UI.Image itemIcon;
    [SerializeField] private TMPro.TextMeshProUGUI quantityText;
    [SerializeField] private UnityEngine.UI.Image background;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0, 0, 0, 0.3f);
    [SerializeField] private Color hoverColor = new Color(0, 0, 0, 0.7f);

    [Header("Drag")]
    [SerializeField] private CanvasGroup canvasGroup;

    private int _slotIndex;
    private GameObject _dragIcon;
    private RectTransform _dragIconRT;
    private Canvas _rootCanvas;

    private void Awake()
    {
        _rootCanvas = transform.root.GetComponent<Canvas>();
    }

    private void Update()
    {
        if (_dragIcon == null) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rootCanvas.transform as RectTransform,
            Input.mousePosition,
            _rootCanvas.worldCamera,
            out Vector3 worldPoint
        );
        _dragIconRT.position = worldPoint;
    }

    public void Setup(int index)
    {
        _slotIndex = index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = normalColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Không drag nếu slot trống
        if (InventoryManager.Instance.GetSlots()[_slotIndex].IsEmpty)
        {
            eventData.pointerDrag = null;
            return;
        }

        // Dim icon gốc trong slot để biết đang bị drag
        itemIcon.color = new Color(1, 1, 1, 0.4f);
        quantityText.gameObject.SetActive(false);

        // Tạo drag visual theo cursor
        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(transform.root);
        _dragIcon.transform.SetAsLastSibling(); // nằm trên cùng

        var image = _dragIcon.AddComponent<UnityEngine.UI.Image>();
        image.sprite = itemIcon.sprite;
        image.raycastTarget = false;

        _dragIconRT = _dragIcon.GetComponent<RectTransform>();
        _dragIconRT.sizeDelta = itemIcon.rectTransform.sizeDelta;

        canvasGroup.blocksRaycasts = false;
        CursorManager.Instance.SetDrag();
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon == null) return;

        Destroy(_dragIcon);
        _dragIcon = null;
        itemIcon.color = Color.white;
        canvasGroup.blocksRaycasts = true;
        CursorManager.Instance.SetDefault();

        ItemSlotUI dropSlot = eventData.pointerEnter?.GetComponent<ItemSlotUI>();
        if (dropSlot != null && dropSlot != this)
            InventoryManager.Instance.SwapSlots(_slotIndex, dropSlot._slotIndex);
        else
            RefreshSelf();
    }

    public void Bind(ItemSlot itemSlot)
    {
        if (itemSlot.IsEmpty)
        {
            itemIcon.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
            return;
        }

        itemIcon.sprite = itemSlot.GetItemSO().icon;
        itemIcon.gameObject.SetActive(true);

        quantityText.text = itemSlot.GetQuantity().ToString();
        quantityText.gameObject.SetActive(itemSlot.GetQuantity() > 1);
    }

    private void RefreshSelf()
    {
        Bind(InventoryManager.Instance.GetSlots()[_slotIndex]);
    }
}
