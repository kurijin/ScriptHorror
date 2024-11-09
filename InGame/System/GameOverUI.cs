using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバーUIの処理
/// </summary>

public class GameOverUI : MonoBehaviour
{

    [SerializeField,Header("終了ボタン")] private Button _retireButton;
    [SerializeField,Header("リトライ")] private Button _retryButton;

    [SerializeField,Header("クリック音")] private AudioClip _okSE;

    private void OnEnable()
    {
        _retireButton.onClick.AddListener(Finish);
        _retryButton.onClick.AddListener(Retry);
    }

    private void Finish()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        InGameFlow.Instance.End();
    }
    private void Retry()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        InGameFlow.Instance.Retry();
    }
}
