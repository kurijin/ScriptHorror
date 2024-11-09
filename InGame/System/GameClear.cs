using UnityEngine.InputSystem;
using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゲームクリア時に呼ばれるもの
/// エンディングムービーが流れ
/// ダイアログが流れて
/// タイトルに戻る.
/// コレクションアイテムはここでPlayerPrefsに送られる
/// </summary>
/// /// 
public class GameClear : MonoBehaviour
{
    //プレイヤー関連
    [SerializeField, Header("プレイヤー")] private GameObject _player;
    [SerializeField, Header("プレイヤーのカメラ")] private Camera _playerCamera;
    [SerializeField, Header("歩行音")] private AudioClip _walkSE;
    [SerializeField, Header("流したいBGM")] private AudioClip _bgm;
    [SerializeField, Header("前進時間（秒）")] private float _forwardDuration = 4f; 
    [SerializeField, Header("回転時間（秒）")] private float _rotationDuration = 2f; 
    [SerializeField, Header("前進速度")] private float _forwardSpeed = 3f; 
    private PlayerInput _playerInputSystem;
    private CharacterController _characterController;

    //敵キャラ関連
    [SerializeField, Header("敵キャラ")] private GameObject _enemy;
    [SerializeField, Header("敵キャラSE")] private AudioClip _enemySE;
    private Animator _animator;

    //UI関連
    [SerializeField, Header("ゲームクリアUI")] private GameObject _gameClearUI;
    [SerializeField, Header("PlayerUI")] private GameObject _playerUI;
    [SerializeField, Header("最後の会話のダイアログ配列")] private Text[] _dialogue;
    [SerializeField, Header("文字の表示速度（秒）")] private float _revealSpeed = 0.1f;
    private string[] messages;
    [SerializeField, Header("ダイアログのSE")] private AudioClip _dialogueSE;


    /// <summary>
    /// プレイヤーの移動制御
    /// ダイアログテキスト取得
    /// </summary>
    private void Start()
    {
        _playerInputSystem = _player.GetComponent<PlayerInput>();
        _characterController = _player.GetComponent<CharacterController>();
        _animator = _enemy.GetComponent<Animator>();

        // _dialogue のテキスト内容をmessageに移してテキストを初期化する(タイトル画面のと同じ)
        messages = new string[_dialogue.Length];
        for (int i = 0; i < _dialogue.Length; i++)
        {
            messages[i] = _dialogue[i].text; 
            _dialogue[i].text = "";          
        }
    }

    /// <summary>
    /// プレイヤーがこのオブジェクトのトリガーに入ったらエンディング開始
    /// ダックアイテムを保存
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InGameFlow.Instance.PauseOUT();
            InGameFlow.Instance.InventoryOUT();
            SoundManager.Instance.StopBGM();
            ItemManager.Instance.SaveCollectDuckItems();
            _playerInputSystem.enabled = false;
            _playerUI.SetActive(false);
            InGameFlow.Instance.PauseOUT();
            PlayClearSequence().Forget(); 
        }
    }

    /// <summary>
    /// プレイヤーの移動,前に直進→後ろ振り返る→敵出現で終了
    /// </summary>
    private async UniTask PlayClearSequence()
    {
        float elapsedTime = 0f;
        SoundManager.Instance.PlaySE2(_walkSE);
        while (elapsedTime < _forwardDuration)
        {
            Vector3 _forward = _player.transform.forward * _forwardSpeed * Time.deltaTime;
            _characterController.Move(_forward);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        SoundManager.Instance.StopSE2();
        Quaternion originalPlayerRotation = _player.transform.rotation;
        Quaternion targetPlayerRotation = originalPlayerRotation * Quaternion.Euler(0f, 180f, 0f); 
        elapsedTime = 0f;

        _enemy.SetActive(true);

        while (elapsedTime < _rotationDuration)
        {
            _player.transform.rotation = Quaternion.Slerp(originalPlayerRotation, targetPlayerRotation, elapsedTime / _rotationDuration);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        _animator.SetTrigger("Attack");
        SoundManager.Instance.PlaySE2(_enemySE);
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        SoundManager.Instance.PlayBGM(_bgm);
        _gameClearUI.SetActive(true);

        //最後のテキスト表示
        await RevealAllTextAsync();
        //ゲームクリア保存
        PlayerPrefs.SetInt("GameClear", 1);
        PlayerPrefs.Save();
        InGameFlow.Instance.End();
    }

    private async UniTask RevealAllTextAsync()
    {
        //表示を待機
        await UniTask.Delay(TimeSpan.FromSeconds(1.5)); 

        for (int i = 0; i < messages.Length; i++)
        {
            if (_dialogue[i] != null)
            {
                if(i == 4) {SoundManager.Instance.StopBGM();}
                await RevealTextAsync(_dialogue[i], messages[i], _revealSpeed); 
            }    
            // 次のダイアログまで2秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(1));  
        }
    }

    /// <summary>
    /// テキストを左から右に流すもの
    /// </summary>
    private async UniTask RevealTextAsync(Text textComponent, string message, float speed)
    {
        SoundManager.Instance.PlaySE3(_dialogueSE);

        int totalCharacters = message.Length;
        for (int i = 0; i <= totalCharacters; i++)
        {
            textComponent.text = message.Substring(0, i);  
            await UniTask.Delay(TimeSpan.FromSeconds(speed));  
        }

        SoundManager.Instance.StopSE3();
    }

}
