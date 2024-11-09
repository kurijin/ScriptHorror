using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 地図を開くためのインベントリにアタッチするUI
/// </summary>
public class InventoryMap : MonoBehaviour
{
    [SerializeField,Header("マップUI")] private GameObject _mapUI;
    [SerializeField, Header("マップボタン")] private Button _mapButton;
    [SerializeField, Header("バックボタン")] private Button _backButton;
    [SerializeField, Header("ボタンSE")] private AudioClip _mapOpenSE;

    void OnEnable()
    {
        _mapUI.SetActive(false);
        _mapButton.onClick.AddListener(OnClickMap);
        _backButton.onClick.AddListener(OnClickBack);
    }

    private void OnClickMap()
    {
        _mapUI.SetActive(true);
        SoundManager.Instance.PlaySE3(_mapOpenSE);
    }
    private void OnClickBack()
    {
        _mapUI.SetActive(false);
    }
}
