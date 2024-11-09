using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// アイテムにつけるスクリプト
/// アイテム拾われるメソッドはPlayerのPickUpから読み込まれる。読み込まれたらItemManagerに渡して
/// アイテム取得UIを表示して消える
/// </summary>
public class Item : MonoBehaviour
{
    [SerializeField, Header("アイテム名")] public string itemName;
    [SerializeField, Header("アイテム画像")] public Sprite itemImage;
    [SerializeField, Header("取得メッセージ")] public string itemMessage;
    [SerializeField,Header("アイテム取得音")] private AudioClip _itemGetSE;
    [SerializeField,Header("関連してるスポットID")] private int _spotID;

    public void OnItemPickedUp()
    {
        ItemManager.Instance.GetItem(itemName);
        SoundManager.Instance.PlaySE3(_itemGetSE);
        InGameFlow.Instance.ItemGet(itemName,itemImage,itemMessage).Forget();
        EnemyManager.Instance.ActiveTrigger(_spotID);
        Destroy(this.gameObject);
    }
}
