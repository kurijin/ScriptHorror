using UnityEngine;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// スタートボタンを押した時のダイアログ
/// </summary>
public class FirstFlow : MonoBehaviour
{
    [SerializeField, Header("最初のダイアログの会話配列")] private Text[] _dialogue;  
    [SerializeField, Header("文字の表示速度（秒）")] private float _revealSpeed = 0.1f;  
    private string[] messages; // 各テキストの内容を保存する配列
    [SerializeField, Header("ダイアログのSE")] private AudioClip _dialogueSE; 

    private async void OnEnable()
    {
        // _dialogue のテキスト内容をmessageに移してテキストを初期化する
        messages = new string[_dialogue.Length];
        for (int i = 0; i < _dialogue.Length; i++)
        {
            messages[i] = _dialogue[i].text; 
            _dialogue[i].text = "";          
        }

        await RevealAllTextAsync();
        await LoadLoadingSceneWithTransition();
    }

    private async UniTask RevealAllTextAsync()
    {
        //表示を待機
        await UniTask.Delay(TimeSpan.FromSeconds(1.5)); 

        for (int i = 0; i < messages.Length; i++)
        {
            if (_dialogue[i] != null)
            {
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
        SoundManager.Instance.PlaySE(_dialogueSE);

        int totalCharacters = message.Length;
        for (int i = 0; i <= totalCharacters; i++)
        {
            textComponent.text = message.Substring(0, i);  
            await UniTask.Delay(TimeSpan.FromSeconds(speed));  
        }

        SoundManager.Instance.StopSE();
    }

    private async UniTask LoadLoadingSceneWithTransition()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("LoadingScene");
        async.allowSceneActivation = true; 
        await UniTask.Yield();
    }
}
