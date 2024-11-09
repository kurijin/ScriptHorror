using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [SerializeField,Header("アイテムのテキスト")] private Text[] inventoryTexts;  

    private void OnEnable()
    {
        UpdateInventoryUI();
    }

    /// <summary>
    /// インベントリのUIを更新
    /// </summary>
    public void UpdateInventoryUI()
    {
        List<string> currentItems = ItemManager.Instance.itemList;
        int itemCount = Mathf.Min(currentItems.Count, inventoryTexts.Length);

        // テキストUIにアイテム名をセット
        for (int i = 0; i < itemCount; i++)
        {
            inventoryTexts[i].text = currentItems[i];
        }

        // 残りのテキストUIを空にする
        for (int i = itemCount; i < inventoryTexts.Length; i++)
        {
            inventoryTexts[i].text = ""; 
        }
    }
}
