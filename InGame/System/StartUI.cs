using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// スタートのムービー
/// テキスト表示後、アニメーションする。
/// </summary>
public class StartUI : MonoBehaviour
{
    [SerializeField, Header("プレイヤー")] private GameObject _player;
    [SerializeField, Header("プレイヤーのカメラ")] private GameObject _playerCamera;
    [SerializeField, Header("テキストを出すパネル")] private GameObject _panel;
    [SerializeField, Header("テキストの場所")] private Text _text;
    [SerializeField, Header("表示するメッセージ")] private string[] _message;
    [SerializeField, Header("メッセージ表示時間（秒）")] private float messageDisplayTime = 2f;
    [SerializeField, Header("アニメーション待機時間（秒）")] private float animationWaitTime = 2f;

    [SerializeField, Header("テキストの音")] private AudioClip _textSE;
    [SerializeField, Header("アニメーションの音")] private AudioClip _animationSE;

    private void OnEnable()
    {
        StartSequence().Forget();
    }

    private async UniTask StartSequence()
    {
        await ShowMessagePanel(_message[0], messageDisplayTime);

        await MoveAnimation();

        for (int i = 1; i < _message.Length; i++)
        {
            await ShowMessagePanel(_message[i], messageDisplayTime);
        }

        gameObject.SetActive(false); // 最後にこのオブジェクトを無効にする
    }

    /// <summary>
    /// テキストを表示するメソッド
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    /// <param name="waitTime">メッセージごとの間隔</param>
    /// <returns></returns> <summary>
    private async UniTask ShowMessagePanel(string message, float waitTime)
    {
        _panel.SetActive(true);
        SoundManager.Instance.PlaySE3(_textSE);
        _text.text = message;
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        _panel.SetActive(false);
    }

    // 手動アニメーション
    private async UniTask MoveAnimation()
    {
        Animator _animator = _player.GetComponent<Animator>();

        // プレイヤーを180度回転、カメラを25度回転させてアニメーション発火
        _player.transform.Rotate(0f, 180f, 0f);
        _playerCamera.transform.Rotate(25f, 0f, 0f);
        _animator.SetTrigger("Knock");
        SoundManager.Instance.PlaySE3(_animationSE);
        await UniTask.Delay(TimeSpan.FromSeconds(animationWaitTime)); 
    }

    private void OnDisable()
    {
        // 元の角度に戻す
        _player.transform.Rotate(0f, -180f, 0f);
        _playerCamera.transform.Rotate(-25f, 0f, 0f);
    }
}
