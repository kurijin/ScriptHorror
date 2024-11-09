using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コレクトアイテムのランダムリスポーン
/// </summary>
public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [SerializeField, Header("出現させるアイテムのプレハブ")] private GameObject[] itemPrefabs;
    [SerializeField, Header("アイテムが出現するポイント")] private Transform[] spawnPoints;
    private GameObject selectedItem1;
    private GameObject selectedItem2;
    private Transform selectedPoint1;
    private Transform selectedPoint2;

    /// <summary>
    /// アイテムと出現ポイントをランダムに2つずつ選んで生成する
    /// </summary>
    public void SpawnRandomItems()
    {
        // アイテムプレハブからランダムに2つ選択
        List<GameObject> availableItems = new List<GameObject>(itemPrefabs);
        selectedItem1 = GetRandomItem(availableItems);
        selectedItem2 = GetRandomItem(availableItems);

        // 出現ポイントからランダムに2つ選択
        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        selectedPoint1 = GetRandomSpawnPoint(availablePoints);
        selectedPoint2 = GetRandomSpawnPoint(availablePoints);

        // アイテムと出現地点を保存
        SaveSelectedItemsAndPoints();

        // 選ばれた地点にアイテムを生成
        Instantiate(selectedItem1, selectedPoint1.position, Quaternion.Euler(-90f, 0f, 0f));
        Instantiate(selectedItem2, selectedPoint2.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    private GameObject GetRandomItem(List<GameObject> availableItems)
    {
        int randomIndex = Random.Range(0, availableItems.Count);
        GameObject selectedItem = availableItems[randomIndex];
        availableItems.RemoveAt(randomIndex); // 選んだアイテムをリストから削除
        return selectedItem;
    }

    private Transform GetRandomSpawnPoint(List<Transform> availablePoints)
    {
        int randomIndex = Random.Range(0, availablePoints.Count);
        Transform selectedPoint = availablePoints[randomIndex];
        availablePoints.RemoveAt(randomIndex); // 選んだ地点をリストから削除
        return selectedPoint;
    }

    /// <summary>
    /// 選択されたアイテムとポイントを PlayerPrefs に保存(リトライ時にそのデータを受け取るため）
    /// </summary>
    private void SaveSelectedItemsAndPoints()
    {
        // アイテムのインデックスを保存
        PlayerPrefs.SetInt("SelectedItem1", System.Array.IndexOf(itemPrefabs, selectedItem1));
        PlayerPrefs.SetInt("SelectedItem2", System.Array.IndexOf(itemPrefabs, selectedItem2));

        // 出現ポイントのインデックスを保存
        PlayerPrefs.SetInt("SelectedPoint1", System.Array.IndexOf(spawnPoints, selectedPoint1));
        PlayerPrefs.SetInt("SelectedPoint2", System.Array.IndexOf(spawnPoints, selectedPoint2));

        PlayerPrefs.Save();
    }

    /// <summary>
    /// PlayerPrefs からアイテムとポイントを読み込んで再生成する
    /// </summary>
    public void RetryRespawnItem()
    {
        // 保存されたアイテムとポイントのインデックスをロード
        int itemIndex1 = PlayerPrefs.GetInt("SelectedItem1", -1);
        int itemIndex2 = PlayerPrefs.GetInt("SelectedItem2", -1);
        int pointIndex1 = PlayerPrefs.GetInt("SelectedPoint1", -1);
        int pointIndex2 = PlayerPrefs.GetInt("SelectedPoint2", -1);

        if (itemIndex1 != -1 && itemIndex2 != -1 && pointIndex1 != -1 && pointIndex2 != -1)
        {
            // アイテムと出現地点をロード
            selectedItem1 = itemPrefabs[itemIndex1];
            selectedItem2 = itemPrefabs[itemIndex2];
            selectedPoint1 = spawnPoints[pointIndex1];
            selectedPoint2 = spawnPoints[pointIndex2];

            // 選ばれた地点にアイテムを生成
            Instantiate(selectedItem1, selectedPoint1.position, Quaternion.Euler(-90f, 0f, 0f));
            Instantiate(selectedItem2, selectedPoint2.position, Quaternion.Euler(-90f, 0f, 0f));
        }
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteKey("SelectedItem1");
        PlayerPrefs.DeleteKey("SelectedItem2");
        PlayerPrefs.DeleteKey("SelectedPoint1");
        PlayerPrefs.DeleteKey("SelectedPoint2");
    }
}
