using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Modal;
using Cysharp.Threading.Tasks;

/// <summary>
/// コレクションページでのアイテム処理
/// ダックはモーダルでより詳細に確認することができる
/// コンプリート特典もある
/// 1.Red 2.Pink 3.Blue 4.White 5.Black 6.Green 7.Purple 8.Yinyan
/// 9.Gold
/// </summary>
public class Collection : MonoBehaviour
{
    public static Collection Instance { get; private set; }
    [SerializeField, Header("ボタンのクリック音")] private AudioClip _buttonSE;
    [SerializeField, Header("シーン管理のCanvasのスクリプト参照(Modal)")] private ModalContainer _modalContainer;
    [SerializeField, Header("クリック時のパーティクル")] private GameObject _clickParticle;
    [SerializeField, Header("コレクションのImageComponent8つ")] private Image[] _collectImage;
    [SerializeField, Header("コレクションのボタンコンポーネント8つ")] private Button[] _collectButton;
    [SerializeField, Header("コレクション収集前の画像")] private Sprite _normalImage;
    [SerializeField, Header("コレクション収集後の画像")] private Sprite[] _getImage;

    //コンプリート用
    [SerializeField, Header("コンプリート画像")] private Image _completeImage;
    [SerializeField, Header("コンプリートボタン")] private Button _completeButton;
    [SerializeField, Header("コンプリート前画像")] private Sprite _normalcompleteImage;
    [SerializeField, Header("コンプリート後画像")] private Sprite _getcompleteImage;
    [SerializeField, Header("コレクションアイテムリスト")] private CollectionItem[] _collectionItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// アイテムを取得済みかを確認して、もし所持しているのならクリックもできて,画像も変更される。
    /// </summary>
    void OnEnable()
    {
        // PlayerPrefsからアイテムを取得する
        string existingItems = PlayerPrefs.GetString("CollectList", "");
        string[] collectedItems = existingItems.Split(',');

        for (int i = 0; i < _collectButton.Length; i++)
        {
            int index = i;

            // ボタンの初期設定
            _collectImage[i].sprite = _normalImage;
            _collectButton[i].interactable = false;

            // アイテムが取得済みの場合画像を変更してボタンを有効化
            if (System.Array.Exists(collectedItems, item => item == _collectionItems[i].itemName))
            {
                _collectImage[i].sprite = _getImage[i];
                _collectButton[i].interactable = true;
            }

            // ボタンイベントの追加
            _collectButton[i].onClick.AddListener(() => _ClickButton(index));
        }

        // コンプリート用
        _completeImage.sprite = _normalcompleteImage;
        _completeButton.interactable = false;
        _completeButton.onClick.RemoveAllListeners();
        _completeButton.onClick.AddListener(() => _ClickButton(8));

        // すべてのアイテムを収集しているか確認
        bool isComplete = true;
        foreach (var item in _collectionItems)
        {
            if (!System.Array.Exists(collectedItems, collectedItem => collectedItem == item.itemName))
            {
                isComplete = false;
                break;
            }
        }

        if (isComplete)
        {
            _completeImage.sprite = _getcompleteImage;
            _completeButton.interactable = true;
        }
    }

    private void _ClickButton(int index)
    {
        ShowItemDetail(index).Forget();
    }

    // アイテム詳細をモーダルで表示するメソッド
    private async UniTask ShowItemDetail(int index)
    {
        var selectedItem = _collectionItems[index];
        await PlaySoundAndPushModal("DescriptionModal");
        ModalManager.Instance.SetItemDetails(selectedItem.itemName, selectedItem.itemImage, selectedItem.itemDescription);
    }

    // 共通のコルーチンメソッド(モーダル)
    private async UniTask PlaySoundAndPushModal(string pageName)
    {
        SoundManager.Instance.PlaySE3(_buttonSE);
        ClickParticle(_clickParticle);
        await _modalContainer.Push(pageName, true);
    }

    // モーダルを閉じる処理
    public void OnClickBack()
    {
        CloseModalCoroutine().Forget();
    }

    private async UniTask CloseModalCoroutine()
    {
        SoundManager.Instance.PlaySE3(_buttonSE);
        ClickParticle(_clickParticle);
        await _modalContainer.Pop(true);
    }

    /// <summary>
    /// クリックして流したいParicleを流す.引数は流したいparicle
    /// <param name="_particleName"></param> 
    /// </summary>
    private void ClickParticle(GameObject _particleName)
    {
        Vector3 clickPosition = Input.mousePosition;
        clickPosition.z = 10f;
        clickPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        Instantiate(_particleName, clickPosition, Quaternion.identity);
    }
}
