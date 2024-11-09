using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトルにボタンイベント追加
/// </summary>

public class TitleUI : MonoBehaviour
{
    [SerializeField,Header("ゲームスタートボタン")] private Button _playButton;
    [SerializeField,Header("設定ボタン")] private Button _settingsButton;   
    [SerializeField,Header("コレクションボタン")] private Button _collectionButton;    
    [SerializeField,Header("クレジットボタン")] private Button _creditButton;  
    [SerializeField,Header("ボタンマネージャーの取得")] private ButtonManager _buttonSelect;   
    [SerializeField,Header("BGM")] private AudioClip _bgm;  

    void Awake()
    {
        _playButton.onClick.AddListener(_buttonSelect.OnClickPlay);
        _settingsButton.onClick.AddListener(_buttonSelect.OnClickSettings);
        _collectionButton.onClick.AddListener(_buttonSelect.OnClickCollection);
        _creditButton.onClick.AddListener(_buttonSelect.OnClickCredit);
    }

    void Start()
    {
        SoundManager.Instance.PlayBGM(_bgm);
    }
}
