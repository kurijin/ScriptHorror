using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 敵の出現を管理するマネージャー
/// 敵の出現トリガーをONにするのは色々なスクリプトから呼ぶ(→フラグが立たないとトリガーは出現しない.)
/// spotID == EnemyIDで敵が死んだ時このスクリプトのチェックポイントEnemyが呼び出されてそれとともにid(spotID,enemyID)が渡される
/// それによってどこの敵かを考え、セーブポイントを作る。
/// 
/// 
/// ゲーム一回クリア時であるならば
/// エネミースポーン間の移動時間によって敵の難易度が難しくなる
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [SerializeField, Header("敵A")] private GameObject[] _easyEnemy;
    [SerializeField, Header("敵B")] private GameObject[] _normalEnemy;
    [SerializeField, Header("敵C")] private GameObject[] _hardEnemy;
    [SerializeField, Header("プレイヤー")] private GameObject _player;
    [SerializeField, Header("敵の出現トリガー")] private GameObject[] _enemyTrigger;

    private int _currentLevel;

    /// <summary> スポーンスポット間でかかったタイムを計測するstatic変数</summary>
    public float elapsedTime;
    private int _gameClear;

    [Header("スポットid毎に考えられるハードの敵、ノーマルの敵の閾値")]
    [SerializeField, Header("ハードの敵のタイムの閾値")]private float[] _hardTimeBox;
    [SerializeField, Header("ノーマルの敵のタイムの閾値")]private float[] _normalTimeBox;

    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 難易度を取得
        _currentLevel = DifficultyManager.Instance.Level;
        ///ゲームクリアかどうかをちぇっく
        _gameClear = PlayerPrefs.GetInt("GameClear", 0);

    }

    /// <summary>
    /// spotによってはあるフラグを通過したらトリガーを有効にする
    /// リスタート時,そのトリガーで一回出てたら削除する
    /// </summary>
    /// <param name="spotnumber">トリガースポットの地点</param>

    public void ActiveTrigger(int spotnumber)
    {
        _enemyTrigger[spotnumber].SetActive(true);
    }

    public void DeleteTrigger(int spotnumber)
    {
        Destroy(_enemyTrigger[spotnumber]);
    }

    //時間経過やアクションにより難易度を変更さしたい場合に使用
    private void Update()
    {
        if(_gameClear == 1)
        {
            _currentLevel = DifficultyManager.Instance.Level;
            elapsedTime += Time.deltaTime;
        }
    }




    /// <summary>
    /// プレイヤーがトリガーポイントに到達したらそこからこのスクリプトが呼ばれる。
    /// </summary>
    /// <param name="respawnPlace">敵のリスポーンする場所</param>

    public void OnPlayerEnterTriggerPlace(GameObject respawnPlace,int id)
    {
        // トリガーポイントに対応するリスポーン場所から敵を出現させる
        SpawnEnemy(respawnPlace.transform.position, id);
    }


    /// <summary>
    /// 敵を出現させるメソッド
    /// </summary>
    /// <param name="spawnPosition">敵のリスポーン場所</param>
    /// <param name="id">スポットごとのID</param>
    private void SpawnEnemy(Vector3 spawnPosition, int id)
    {
        ///idが３以下（ストーリーに関連するスポットのみ適応）
        if (_gameClear == 1 && id <= 3)
        {
            EnemySpawnTimeCheck(id);
        }
        GameObject[] respawnEnemies;
        // 難易度に応じた敵のグループを選択
        switch (_currentLevel)
        {
            case 0:
                respawnEnemies = _easyEnemy
                                .Concat(_normalEnemy)
                                .Concat(_hardEnemy)
                                .ToArray();
                break;
            case 1:
                respawnEnemies = _easyEnemy;
                break;
            case 2:
                respawnEnemies = _normalEnemy;
                break;
            case 3:
                respawnEnemies = _hardEnemy;
                break;
            default:
                respawnEnemies = _normalEnemy;
                break;
        }

        // 各敵グループに格納されている敵からランダムに選んで出現させる
        int randomIndex = Random.Range(0, respawnEnemies.Length);  
        GameObject spawnedEnemy = Instantiate(respawnEnemies[randomIndex], spawnPosition, Quaternion.identity);

        Enemy enemyComponent = spawnedEnemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.SetID(id);
        }
    }

    /// <summary>
    /// スポーンエネミーのごとにその時点の経過時間を考えて、ある閾値以上だと敵を出現させる
    /// </summary>
    /// <param name="id"> スポットのID (idは1から始まっているため配列では-1する)</param>
    private void EnemySpawnTimeCheck(int id)
    {
        float _hardTime = _hardTimeBox[id - 1];
        float _normalTime = _normalTimeBox[id - 1];

        if(elapsedTime > _hardTime)
        {
            DifficultyManager.Instance.Level = 3;
            _currentLevel = 3;
        }
        else if(elapsedTime > _normalTime)
        {
            DifficultyManager.Instance.Level = 2;
            _currentLevel = 2;
        }
        else if(elapsedTime >= 0)
        {
            DifficultyManager.Instance.Level = 1;
            _currentLevel = 1;
        }
        else
        {
            DifficultyManager.Instance.Level = 0;
            _currentLevel = 0;
        }
    }

    /// <summary>
    /// プレイヤーを渡すもの
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer()
    {
        return _player;
    }

    /// <summary>
    /// どこの敵が消えたかによってチェックポイントを更新
    /// </summary>
    /// <param name="ID">トリガーポイントのid ,敵id</param>
    public void EnemyCheckpoint(int ID)
    {
        switch(ID)
        {
            case 1:
                CheckPointManager.Instance.Check = 2;
                break;
            case 2:
                CheckPointManager.Instance.Check = 3;
                break;
            ///<summary>
            ///最後の避難経路を見た後のチェックポイントいらない　??
            ///case 3:
            /// CheckPointManager.Instance.Check = 4;
            ///break;
            ///</summary>
            default:
                break;
        }
    }
}
