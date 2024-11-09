using UnityEngine;

/// <summary>
/// ゲーム内の変動する怖さ(以降難易度)について常に保持しておくもの
/// 設定画面の変更のみisLevelLocked=trueになり保存は永続する
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }
    //ゲームの難易度 0:ランダム 1:怖くない　2:普通　3:怖い
    [SerializeField,Header("ゲームの難易度")] public int gameLevel = 0;
    public bool isLevelLocked = false;
    public int Level
    {
        get { return gameLevel; }
        set
        {
            if (!isLevelLocked)  
            {
                gameLevel = value;
            }
            else
            {
                Debug.LogWarning("Level is locked");
            }
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
