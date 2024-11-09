using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// モーダルが呼ばれた時Collectionからもらった情報を引き渡す
/// </summary>
public class ModalManager : MonoBehaviour
{
    public static ModalManager Instance { get; private set; }

    [SerializeField, Header("アイテム名のUI")] private Text _itemNameText;
    [SerializeField, Header("アイテム画像のUI")] private Image _itemImage;
    [SerializeField, Header("アイテム説明のUI")] private Text _itemDescriptionText;
    [SerializeField, Header("バックボタン")] private Button _backButton;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        _backButton.onClick.AddListener(() => Collection.Instance.OnClickBack());
    }


    public void SetItemDetails(string itemName, Sprite itemImage, string itemDescription)
    {
        _itemNameText.text = itemName;
        _itemImage.sprite = itemImage;
        _itemDescriptionText.text = itemDescription;
    }
}
