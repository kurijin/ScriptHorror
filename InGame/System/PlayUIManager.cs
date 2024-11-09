using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーに関するUIシステムの処理
/// Hpとスタミナを表示する
/// </summary>
public class PlayerUIManager : MonoBehaviour
{
    [SerializeField, Header("プレイヤーのヘルスを取得")] private PlayerHealth _playerHealth;
    [SerializeField, Header("スタミナバー")] private Slider _staminaSlider;
    [SerializeField, Header("HPの画像配列")] private Image[] _hpArrow;
    [SerializeField, Header("フルのHP画像")] private Sprite _fullhpImage;
    [SerializeField, Header("減少時のHP画像")] private Sprite _lesshpImage;

    //最大hp
    private int maxHp;

    void Start()
    {
        // スタミナバーの初期値を設定
        _staminaSlider.value = _playerHealth.maxStamina;

        // 最大HPを取得
        maxHp = _playerHealth.GetHP();

        // HPの画像を初期状態ですべて満タンに設定
        for (int i = 0; i < _hpArrow.Length; i++)
        {
            _hpArrow[i].sprite = _fullhpImage;
        }
    }

    void Update()
    {
        _staminaSlider.value = _playerHealth.GetStamina();

        // HPを監視して減少させる
        UpdateHPImages(_playerHealth.GetHP());
    }

    void UpdateHPImages(int currentHp)
    {
        for (int i = 0; i < _hpArrow.Length; i++)
        {
            if (i < currentHp)
            {
                _hpArrow[i].sprite = _fullhpImage;  // HPが残っている場合
            }
            else
            {
                _hpArrow[i].sprite = _lesshpImage;  // HPが減っている場合
            }
        }
    }

}
