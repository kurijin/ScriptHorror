using UnityEngine;

/// <summary>
/// このスポットに訪れたとき敵が出現する
/// 事前にこのトリガースポットと敵キャラが出現する場所に
/// </summary>
public class TriggerSpot : MonoBehaviour
{ 
    [SerializeField,Header("このトリガーのID")] public int triggerID;  
    [SerializeField,Header("このトリガーに関連付けされている出現スポット")] public GameObject respawnPlace;  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            // プレイヤーがこのトリガーに入ったらEnemyManagerに関連づけてる出現場所を引き渡す
            EnemyManager.Instance.OnPlayerEnterTriggerPlace(respawnPlace,triggerID);
            Destroy(this.gameObject);
        }
    }
}
