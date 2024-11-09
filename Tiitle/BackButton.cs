using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戻るボタンにつけるもの
/// </summary>

public class BackButton : MonoBehaviour
{
    //バックボタンの参照
    private Button _backButton; 
    private ButtonManager _buttonSelect; 

    private void OnEnable()
    {
        GameObject buttonManager = GameObject.Find("ButtonManager");
        _buttonSelect = buttonManager.GetComponent<ButtonManager>();
        _backButton = GetComponent<Button>();
        _backButton.onClick.AddListener(_buttonSelect.OnClickBacktoTitle);
    }
}
