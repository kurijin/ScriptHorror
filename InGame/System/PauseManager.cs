using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ポーズ画面のマネージャー
/// Escapeボタンでポーズ画面を起動する
/// </summary>

public class PauseManager : MonoBehaviour
{
    [SerializeField,Header("遊び方ボタン")] private Button _howtoButton;
    [SerializeField,Header("設定ボタン")] private Button _settingsButton;
    [SerializeField,Header("終了ボタン")] private Button _retireButton;

    [SerializeField,Header("遊び方UI")] private GameObject _howToPlayUI;
    [SerializeField,Header("設定UI")] private GameObject _settingsUI;
    [SerializeField,Header("バックボタン(遊び方)")] private Button _backButton;
    [SerializeField,Header("バックボタン(設定)")] private Button _backButton2;

    [SerializeField,Header("クリック音")] private AudioClip _okSE;

    private void OnEnable()
    {
        _howtoButton.onClick.AddListener(DisPlayHowto);
        _settingsButton.onClick.AddListener(DisplaySettings);
        _backButton.onClick.AddListener(Back);
        _backButton2.onClick.AddListener(Back);

        _retireButton.onClick.AddListener(Finish);
    }

    private void DisPlayHowto()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        _howToPlayUI.SetActive(true);
    }

    private void DisplaySettings()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        _settingsUI.SetActive(true);
    }

    private void Back()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        _howToPlayUI.SetActive(false);
        _settingsUI.SetActive(false);
    }

    private void Finish()
    {
        SoundManager.Instance.PlaySE3(_okSE);
        InGameFlow.Instance.End();
    }
}
