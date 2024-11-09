using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 敵キャラの基底クラス
/// EnemyManagerから出た時敵IDも付与され、死んだ時それをEnemyManagerに返す.
/// 敵ID = 1,2,3はトリガースポットのID
/// </summary>
public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private PlayerHealth _playerHealth;
    [SerializeField, Header("敵の移動速度")] private float _speed = 3f;
    [SerializeField, Header("敵の消える時間")] private float _vanishTime;
    [SerializeField, Header("攻撃後の停止時間（秒）")] private float _stopDuration = 2f;  
    [SerializeField, Header("通常BGM")] private AudioClip _normalBGM;
    [SerializeField, Header("敵のBGM")] private AudioClip _bgm;
    [SerializeField, Header("敵のSE")] private AudioClip _se;

    private NavMeshAgent _myAgent;
    private Animator _animator;
    private float _elapsedTime;
    private bool _isAttacking = false; 

    //この敵がどこのトリガースポットから出てきた敵かを管理するもの
    public int ID { get; private set; }

    public void SetID(int id)
    {
        ID = id;
    }

    /// <summary>
    /// 出てきた時にBGMを上書きする。
    /// またプレイヤーを取得する(PrefabであるためEnemyManagerを介して)
    /// </summary>
    private void Start()
    {
        EnemySpawnUI.Instance.OnEnemySpawned();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(_bgm);
        _player = EnemyManager.Instance.GetPlayer();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _myAgent = GetComponent<NavMeshAgent>();
        _myAgent.speed = _speed;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// NavMeshのAIで追いかける
    /// </summary>
    private void Update()
    {
        if (!_isAttacking)
        {
            _animator.SetFloat("Speed",_speed);
            _myAgent.SetDestination(_player.transform.position);
        }

        _elapsedTime += Time.deltaTime;

        // 一定時間経過したら敵を消す
        if (_elapsedTime > _vanishTime)
        {
            Vanish();
        }
    }

    private void Vanish()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(_normalBGM);
        EnemyManager.Instance.EnemyCheckpoint(ID);
        if(ID <= 3)
        {
            EnemyManager.Instance.elapsedTime = 0f;
        }
        EnemySpawnUI.Instance.OnEnemyDisappeared();
        Destroy(gameObject);
    }

    /// <summary>
    /// プレイヤーに当たったら攻撃,攻撃後一時停止
    /// </summary>
    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            await Attack();
        }
    }

    public virtual async UniTask Attack()
    {
        if (!InGameFlow.Instance.isFinish && this != null && _myAgent != null)
        {
            SoundManager.Instance.PlaySE3(_se);
            if (_myAgent != null)
            {
                _myAgent.isStopped = true;
            }
            _isAttacking = true;

            _animator.SetTrigger("Attack");

            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(1);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_stopDuration));

            if (this != null && _myAgent != null)
            {
                _myAgent.isStopped = false;
                _isAttacking = false;
            }
        }
    }
}
