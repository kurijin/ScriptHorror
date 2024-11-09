using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定画面での難易度処理
/// 設定画面を開いた時に現在のレベルを取得
/// 現在のレベルの周りに枠組みを表示
/// ランダムがデフォルトだが、難易度を選択された方にはそのレベルのみ提供する 
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [SerializeField, Header("ランダムレベル")] private Button _level0;
    [SerializeField, Header("level1")] private Button _level1;
    [SerializeField, Header("level2")] private Button _level2;
    [SerializeField, Header("level3")] private Button _level3;

    [SerializeField, Header("ランダムレベルの枠組み")] private GameObject _level0Frame;
    [SerializeField, Header("level1の枠組み")] private GameObject _level1Frame;
    [SerializeField, Header("level2の枠組み")] private GameObject _level2Frame;
    [SerializeField, Header("level3の枠組み")] private GameObject _level3Frame;
    private int _currentLevel;

    private void OnEnable()
    {
        DifficultyManager.Instance.isLevelLocked = false;
        // 各ボタンにイベントを追加
        _level0.onClick.AddListener(() => SetLevel(0));
        _level1.onClick.AddListener(() => SetLevel(1));
        _level2.onClick.AddListener(() => SetLevel(2));
        _level3.onClick.AddListener(() => SetLevel(3));

        // 現在のレベルを取得する
        _currentLevel = DifficultyManager.Instance.Level;
        DisPlayFrameWork();
    }

    private void SetLevel(int level)
    {
        // ボタンを押したらインスタンスのレベルを変える.
        DifficultyManager.Instance.Level = level;
        _currentLevel = level;

        DisPlayFrameWork();
    }

    private void DisPlayFrameWork()
    {
        // 現在のレベルに基づいてフレームを表示
        _level0Frame.SetActive(_currentLevel == 0);
        _level1Frame.SetActive(_currentLevel == 1);
        _level2Frame.SetActive(_currentLevel == 2);
        _level3Frame.SetActive(_currentLevel == 3);
    }

    //ランダム以外を選ばれたユーザーはそれで難易度変更はなし
    private void OnDisable()
    {
        if (_currentLevel != 0)
        {
            DifficultyManager.Instance.isLevelLocked = true;
        }
    }
}
