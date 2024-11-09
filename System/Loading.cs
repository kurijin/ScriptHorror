using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;

/// <summary>
/// ローディング画面でのスクリプト
/// 非同期で読み込みを開始して、0.9で変異
/// テキストは点滅
/// </summary>
public class Loading : MonoBehaviour
{
    [SerializeField, Header("進行状況を可視化するもの")] private Slider _progressBar; 
    [SerializeField, Header("ローディング中を示すテキスト")] private Text _loadingText; 
    void Awake()
    {
        LoadInGameSceneWithLoadingScreen1().Forget();
        BlinkText().Forget();
    }

    private async UniTask LoadInGameSceneWithLoadingScreen1()
    {
        _progressBar.value = 0f;
        
        AsyncOperation async1 = SceneManager.LoadSceneAsync("InGame");
        async1.allowSceneActivation = false; 

        // シーンの読み込みが完了するまで進行状況を取得
        while (!async1.isDone)
        {
            float progress = Mathf.Clamp01(async1.progress / 0.9f);
            _progressBar.value = progress;

            if (async1.progress >= 0.9f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5)); 
                async1.allowSceneActivation = true;

                // BlinkTextを停止
                _loadingText = null;
                break; // ループを抜ける
            }

            await UniTask.Yield(); 
        }
    }

    private async UniTask BlinkText()
    {
        while (_loadingText != null) 
        {
            _loadingText.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            if (_loadingText == null) break; 

            _loadingText.enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }


}
