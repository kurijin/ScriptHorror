using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

/// <summary>
/// アイテムの所持状況を考えるもの
/// リストでアイテム管理
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }  

    public List<string> itemList = new List<string>();  

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    /// <summary>
    /// アイテムを取得した時Itemのスクリプトからここにアイテム名を引数にして飛ばす
    /// </summary>
    /// <param name="_item">取得するアイテム名</param>
    public void GetItem(string _item)
    {
        itemList.Add(_item);
        Debug.Log(_item + " を取得しました。");
    }

    /// <summary>
    /// チェックポイントが更新されるたびにその時点でのPlayerPrefsにアイテム情報を保存
    /// </summary>
    public void SaveItemList()
    {
        string items = string.Join(",", itemList);  
        PlayerPrefs.SetString("ItemList", items);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// チェックポイントから呼ばれた時その時時点で、以前保存してたアイテムリストから取得
    /// もしこの時インベントリにアイテムを持ってるならシーン内のアイテムを削除
    /// </summary>
    public void LoadItemList()
    {
        if (PlayerPrefs.HasKey("ItemList"))
        {
            string savedItems = PlayerPrefs.GetString("ItemList");
            // 空のエントリーを除外してリストに変換
            itemList = new List<string>(savedItems.Split(',').Where(item => !string.IsNullOrEmpty(item)).ToList());
        }
        else
        {
            itemList = new List<string>();
        }

        GameObject[] itemsInScene = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in itemsInScene)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent != null && itemList.Contains(itemComponent.itemName))
            {
                Destroy(item);
            }
        }
    }


    /// <summary>
    /// アイテムリストを消す
    /// 必要性?????
    /// </summary>
    public void ClearItemList()
    {
        itemList.Clear();
    }

    /// <summary>
    /// クリア時のみ呼び出す.
    /// コレクションアイテムであるダックシリーズだけフィルタリング取得
    /// </summary>

    public void SaveCollectDuckItems()
    {
        List<string> filteredItems = itemList.FindAll(item => item.Contains("ダック"));
        string existingItems = PlayerPrefs.GetString("CollectList", "");
        List<string> collectList = new List<string>();
        if (!string.IsNullOrEmpty(existingItems))
        {
            collectList.AddRange(existingItems.Split(','));
        }
        foreach (string item in filteredItems)
        {
            if (!collectList.Contains(item))
            {
                collectList.Add(item);
            }
        }
        // リストを文字列に戻して保存
        string items = string.Join(",", collectList);
        PlayerPrefs.SetString("CollectList", items);
        PlayerPrefs.Save();
    }

}
