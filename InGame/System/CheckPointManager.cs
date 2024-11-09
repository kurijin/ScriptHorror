using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーがチェックポイントに訪れた時の管理をするもの
/// 進行度0:初期
/// 進行度1:チュートリアル終了
/// 進行度2:電池拾った後の敵消えた後
/// 進行度3:大部屋鍵取得後の敵消えた後
/// これいらない？→進行度4:大部屋での敵が消えた後
/// </summary>
public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; }

    [SerializeField, Header("ゲームの進行度")]
    private int checkPoint = 0;
    public Vector3 currentPosition;

    /// <summary>
    /// チェックポイントが更新されるたびに呼ばれるメソッド
    /// 更新時、その時点でのアイテムリストを取得する。
    /// </summary>
    public int Check
    {
        get { return checkPoint; }
        set
        {
            checkPoint = value;
            currentPosition = InGameFlow.Instance.PlayerPosition();
            ItemManager.Instance.SaveItemList();
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
