using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームの設定画面での音量の変更
/// </summary>
public class SoundSlider : MonoBehaviour
{
    [SerializeField, Header("BGMのスライダー")] private Slider _bgmSlider;
    [SerializeField, Header("SEのスライダー")] private Slider _seSlider;

    void OnEnable()
    {
        // スライダーの初期値をAudioSourceの現在の音量に設定
        _bgmSlider.value = SoundManager.Instance.audioSourceBGM.volume;
        _seSlider.value = SoundManager.Instance.audioSourceSE.volume;
        SoundManager.Instance.audioSourceSE2.volume = _seSlider.value;
        SoundManager.Instance.audioSourceSE3.volume = _seSlider.value;
    }

    void Update()
    {
        // BGMの音量をスライダーに応じて変更
        SoundManager.Instance.audioSourceBGM.volume = _bgmSlider.value;

        // SEの音量をスライダーに応じて、全てのSE用AudioSourceに適用
        SoundManager.Instance.audioSourceSE.volume = _seSlider.value;
        SoundManager.Instance.audioSourceSE2.volume = _seSlider.value;  
        SoundManager.Instance.audioSourceSE3.volume = _seSlider.value;  
    }
}
