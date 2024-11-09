using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// PlayerのHPとスタミナ管理
/// スタミナがあれば走れるので、Playerの動きのスクリプトで監視
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    // プレイヤーのhpと無敵時間
    [SerializeField, Header("プレイヤーのHP")] private int _hp = 3;
    private Player _player;
    [SerializeField, Header("プレイヤーの無敵時間")] private float _waitTime = 1f; 

    // プレイヤーのスタミナとフレーム当たりの増加量
    [SerializeField, Header("プレイヤーのMAXスタミナ")] public float maxStamina = 100f;
    [SerializeField, Header("スタミナの増加率")] private float _increaseStamina = 0.05f;
    private float _stamina;

    // 無敵状態のフラグ
    public bool isInvincible = false;

    // 音声
    [SerializeField, Header("攻撃受けた音")] private AudioClip _damageSE;
    [SerializeField, Header("ゲームオーバー")] private AudioClip _gameOverSE;
    public void Start()
    {
        _player = gameObject.GetComponent<Player>();
        _stamina = maxStamina;
    }

    /// <summary>
    /// 敵から受けるダメージをここで処理
    /// 無敵時間中はダメージを受けない
    /// </summary>
    /// <param name="power">敵キャラのパワー</param>
    public async void TakeDamage(int power)
    {
        // 無敵状態であればダメージを受けない
        if (isInvincible) return;
        EnemySpawnUI.Instance.OnDamageTaken();

        SoundManager.Instance.PlaySE2(_damageSE);
        _hp -= power;
        Debug.Log(_hp);

        if (_hp <= 0)
        {
            Die().Forget();
        }
        else
        {
            isInvincible = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTime));
            isInvincible = false;
        }
    }

    private async UniTask Die()
    {
        SoundManager.Instance.PlaySE2(_gameOverSE);
        await InGameFlow.Instance.GameOver();  
    }


    public int GetHP()
    {
        return _hp;
    }

    public float GetStamina()
    {
        return _stamina;
    }

    /// <summary>
    /// 走ったらスタミナを消費させる
    /// </summary>
    /// <param name="runpower">減少率</param>
    public void ConsumeStamina(float runpower)
    {
        _stamina -= runpower;
        // スタミナが最低値を下回らないようにする
        _stamina = Mathf.Clamp(_stamina, 0, maxStamina);
    }

    void Update()
    {
        if(!_player.isRunnig)
        {
            // 走っていない状態ならスタミナ増加
            _stamina += _increaseStamina;
        }
        // スタミナが最大値を超えないようにする
        _stamina = Mathf.Clamp(_stamina, 0, maxStamina);
    }
}
