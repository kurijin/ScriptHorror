using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// アイテムにより何かイベントが起こるものにアタッチするスクリプト
/// アタッチされたものがPCの場合だけ別のメッセージと、キーロックのタグがLockになり新たな部屋に行けるようになる
/// </summary>



public class Lock : MonoBehaviour
{
    [SerializeField,Header("必要なアイテム名")] private string _needItem;
    [SerializeField,Header("鍵開ける音")] private AudioClip _unlockSE;
    [SerializeField,Header("鍵開けた時のメッセージ")] private string _unlockMessage;
    [SerializeField,Header("鍵開けなかった音")] private AudioClip _lockSE;
    [SerializeField,Header("鍵開けれなかった時のメッセージ")] private string _lockMessage;

    //PC用
    [SerializeField,Header("PCがついた時のマテリアル ---以下PC用---")] private Material _onPC; 
    private Renderer _pcRenderer;
    [SerializeField,Header("PC起動時メッセージ")] private string _pcMessage;
    [SerializeField,Header("PC起動後に有効になるロック")] private GameObject _keyLock;

    //ロック用
    [SerializeField,Header("キーロック解除時にdoorとなるものR--keylock用---")] private GameObject _doorR;
    [SerializeField,Header("キーロック解除時にdoorとなるものL")] private GameObject _doorL;

    private Animator _doorAnima;

    void Start()
    {
        _pcRenderer = GetComponent<Renderer>();
        _doorAnima = GetComponent<Animator>();
    }

    public void ClearLock()
    {
        if(ItemManager.Instance.itemList.Contains(_needItem))
        {
            gameObject.tag = "Untagged";
            SoundManager.Instance.PlaySE3(_unlockSE);
            InGameFlow.Instance.ShowMessage(_unlockMessage).Forget();
            if(gameObject.name == "PC")
            {
                _pcRenderer.material = _onPC;
                InGameFlow.Instance.ShowMessage(_pcMessage).Forget();
                SoundManager.Instance.StopSE3();
                _keyLock.tag = "Lock";
                gameObject.tag = "Lock";
            }
            else if(gameObject.name == "KeyLock")
            {
                _doorR.tag = "Door";
                _doorL.tag = "Door";
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            SoundManager.Instance.PlaySE3(_lockSE);
            InGameFlow.Instance.ShowMessage(_lockMessage).Forget();
        }
    }
}
