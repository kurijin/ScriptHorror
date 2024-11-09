using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ゲームの大まかな流れを処理するスクリプト
/// クリア、ポーズ、ゲームオーバー、クリア時に読み込まれる
/// </summary>
public class InGameFlow : MonoBehaviour
{
    public static InGameFlow Instance { get; private set; }

    //UI参照
    [SerializeField,Header("PlayerUI")] private GameObject _playerUI;
    [SerializeField,Header("スタートUI")] private GameObject _startUI;
    [SerializeField,Header("遊び方UI")] private GameObject _howToPlayUI;
    [SerializeField,Header("アイテムゲットUI")] private GameObject _itemGetUI;
    [SerializeField,Header("アイテム画像")] private Image _itemImage;
    [SerializeField,Header("アイテム名")] private Text _itemName;
    [SerializeField,Header("アイテムメッセージ")] private Text _itemMessage;
    [SerializeField,Header("メッセージUI")] private GameObject _messageUI;
    [SerializeField,Header("写すメッセージ")] private Text _showMessage;
    [SerializeField,Header("ポーズ画面UI")] private GameObject _pauseUI;
    [SerializeField, Header("アイテムインベントリUI")] private GameObject _ItemInventoryUI;
    [SerializeField,Header("ゲームオーバーUI")] private GameObject _gameOverUI;

    //UI時に使用するシステムの参照
    [SerializeField,Header("プレイヤーのインプットアクション")] private PlayerInput _playerInputSystem;
    [SerializeField,Header("敵管理のゲームオブジェクト")] private GameObject _enemyManager;

    //ポーズメニューのインプットシステム取得
    private PlayerInput _inputActions;

    //ポーズ画面、アイテムインベントリを開くアクションの追加のもの
    private InputAction _pauseAction;
    private InputAction _inventoryAction;
    [SerializeField, Header("パネル開く音")] private AudioClip _panelSE;

    //スタートUIが開かれたかどうかを確認するもの
    private bool _isStart;

    //画面上にアクションにより消せるUIが出た時の確認するもの
    private bool _isOK = false;

    //ポーズ中かアイテムインベントリ中かどうかを確認する
    private bool _isPausing = false;
    private bool _isInventory = false;

    //死亡&チェックポイント処理
    [SerializeField,Header("プレイヤー")] private GameObject _player;
    public bool isFinish = false;
    [SerializeField, Header("第2チェックポイントの後に有効になるもの")] private GameObject _keyLock;

    [SerializeField,Header("通常BGM")] private AudioClip _normalBGM;


    private void Awake()
    {
        Cursor.visible = false;  // カーソルを非表示にする
        Cursor.lockState = CursorLockMode.Locked;
        if (Instance == null)
        {
            Instance = this;
        }

        //プレイヤーの動きを制御
        _playerInputSystem.enabled = false;

        //Pause,ItemInventoryのインプットアクションは手動で追加  
        _inputActions = GetComponent<PlayerInput>();      
        _pauseAction = _inputActions.actions.FindActionMap("UI").FindAction("Pause");
        _pauseAction.performed += OnPause; 
        _pauseAction.Disable();

        _inventoryAction = _inputActions.actions.FindActionMap("UI").FindAction("ItemInventory");
        _inventoryAction.performed += OnInventory;
        _inventoryAction.Disable();
    }

    public Vector3 PlayerPosition()
    {
        return _player.transform.position;
    }

    //時間を空けてStartUIを表示
    private async void Start()
    {
        SoundManager.Instance.PlayBGM(_normalBGM);
        switch (CheckPointManager.Instance.Check)
        {
            //初期
            case 0:
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                ItemSpawnManager.Instance.SpawnRandomItems();
                _startUI.SetActive(true);
                _isStart = true;
                break;
            //チュートリアル~電池取得
            case 1:
                _player.GetComponent<CharacterController>().enabled = false; 
                _player.transform.position = CheckPointManager.Instance.currentPosition; 
                _player.GetComponent<CharacterController>().enabled = true; 
                ItemSpawnManager.Instance.RetryRespawnItem();
                GameStart();
                break;
            //電池取得後~大部屋の鍵取得
            case 2:
                EnemyManager.Instance.DeleteTrigger(1);
                _player.GetComponent<CharacterController>().enabled = false;
                _player.transform.position = CheckPointManager.Instance.currentPosition;
                _player.GetComponent<CharacterController>().enabled = true;
                ItemSpawnManager.Instance.RetryRespawnItem();
                GameStart();
                break;
            //大部屋の鍵取得後~大部屋内
            case 3:
                EnemyManager.Instance.DeleteTrigger(1);
                EnemyManager.Instance.DeleteTrigger(2);
                _keyLock.tag = "Lock";
                _player.GetComponent<CharacterController>().enabled = false;
                _player.transform.position = CheckPointManager.Instance.currentPosition;
                _player.GetComponent<CharacterController>().enabled = true;
                ItemSpawnManager.Instance.RetryRespawnItem();
                GameStart();
                break;
            //大部屋~
            case 4:
                EnemyManager.Instance.DeleteTrigger(1);
                EnemyManager.Instance.DeleteTrigger(2);
                EnemyManager.Instance.DeleteTrigger(3);
                _player.GetComponent<CharacterController>().enabled = false;
                _player.transform.position = CheckPointManager.Instance.currentPosition;
                _player.GetComponent<CharacterController>().enabled = true;
                ItemSpawnManager.Instance.RetryRespawnItem();
                GameStart();
                break;
            default:
                GameStart();
                break;
        }
    }

    private void  Update()
    {
        if(!_startUI.activeSelf && _isStart)
        {
            _isStart = false;
            HowtoUI().Forget();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            CheckPointManager.Instance.Check = 1;
            Retry();
        }
    }

    private async UniTask HowtoUI()
    {
        //遊び方のUI表示
        _howToPlayUI.SetActive(true);
        _isOK = false;
        await WaitForInput();
        _howToPlayUI.SetActive(false);
        CheckPointManager.Instance.Check = 1;
        GameStart();
    }

    private void GameStart()
    {
        // ゲーム開始
        _pauseAction.Enable();
        _inventoryAction.Enable();
        _playerUI.SetActive(true);
        _playerInputSystem.enabled = true;
        _enemyManager.SetActive(true);
        ItemManager.Instance.LoadItemList();
        EnemyManager.Instance.elapsedTime = 0f;
    }

    public async UniTask ShowMessage(string message)
    {
        _isOK = false;
        _playerInputSystem.enabled = false;
        Time.timeScale = 0f;
        _messageUI.SetActive(true);
        _showMessage.text = message;
        await WaitForInput();
        _messageUI.SetActive(false);
        Time.timeScale = 1f;
        _playerInputSystem.enabled = true;
        SoundManager.Instance.StopSE3();
    }
    public async UniTask ItemGet(string ItemName,Sprite ItemImage,string message)
    {
        _isOK = false;
        Time.timeScale = 0f;
        _itemName.text = ItemName;
        _itemMessage.text = message;
        _itemImage.sprite = ItemImage;
        _itemGetUI.SetActive(true);
        await WaitForInput();
        _itemGetUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public async UniTask GameOver()
    {
        isFinish = true;
        _playerInputSystem.enabled = false;
        _playerUI.SetActive(false);
        Quaternion targetRotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 25f);
        _player.transform.rotation = targetRotation;
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        Cursor.visible = true;  
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        _gameOverUI.SetActive(true);
    }

    public void Retry()
    {
        ItemManager.Instance.ClearItemList();
        PauseOUT();
        InventoryOUT();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void End()
    {
        CheckPointManager.Instance.Check = 0;
        ItemSpawnManager.Instance.DeletePlayerPrefs();
        ItemManager.Instance.ClearItemList();  
        Destroy(ItemManager.Instance.gameObject);
        PauseOUT();
        InventoryOUT();
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Title");
    }


    /// <summary>
    /// クリックイベントされたら_isOK=trueにして,OKボタン押されるまで待機するメソッド
    /// </summary>
    private async UniTask WaitForInput()
    {
        while (!_isOK)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);  
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!_isOK && context.phase == InputActionPhase.Performed)  
        {
            _isOK = true;
        }
    }

    /// <summary>
    /// escキーを押すたびにポーズかどうかを切り替える
    /// プレイヤーと敵の動きを止める
    ///  処理が重くならないように,敵はEnemyManagerを確認してもし敵が出現しているならFindをする
    /// </summary>
    public void OnPause(InputAction.CallbackContext context)
    {

        _isPausing = !_isPausing;
        if(_isPausing)
        {
            SoundManager.Instance.PlaySE3(_panelSE);
            Cursor.visible = true; 
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;  
            Cursor.lockState = CursorLockMode.Locked;
        }

        _playerInputSystem.enabled = !_isPausing;
        _pauseUI.SetActive(_isPausing);
        _playerUI.SetActive(!_isPausing);
    }

    //ゲームクリアフラグを取った時のみ呼び出す
    public void PauseOUT()
    {
        _pauseAction.performed -= OnPause;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _isInventory = !_isInventory;
            if (_isInventory)
            {
                SoundManager.Instance.PlaySE3(_panelSE);
                Time.timeScale = 0f;
                Cursor.visible = true;  
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;  
                Cursor.lockState = CursorLockMode.Locked;
            }

            _playerInputSystem.enabled = !_isInventory;
            _ItemInventoryUI.SetActive(_isInventory);
            _playerUI.SetActive(!_isPausing);
        }
    }

    //ゲームクリアフラグを取った時のみ呼び出す
    public void InventoryOUT()
    {
        _inventoryAction.performed -= OnInventory;
    }
}
