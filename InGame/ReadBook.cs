using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 大きい地図に貼るスクリプト
/// 地図には脱出のヒントと出口が書かれていて
/// このスクリプトを読み込んだ時出口が有効になる(TagがUnTagからLockになる)
/// </summary>
public class ReadBook : MonoBehaviour
{
    [SerializeField,Header("脱出ヒントUI")] private GameObject _hintPanel;
    [SerializeField,Header("メッセージパネル")] private GameObject _messagePanel;
    [SerializeField,Header("メッセージテキスト")] private Text _messageText;
    [SerializeField,Header("テキスト内容")] private string _message;
    [SerializeField,Header("タグを変えるオブジェクト最終脱出床")] private GameObject _changeTagObject;
    [SerializeField, Header("関連してるスポットID")] private int _spotID;
    

    public async UniTask OnBookRead()
    {
        _messageText.text = _message;
        _hintPanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(5f));
        _hintPanel.SetActive(false);

        _messagePanel.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        _messagePanel.SetActive(false); 

        _changeTagObject.tag = "Lock";
        EnemyManager.Instance.ActiveTrigger(_spotID);
    }
}
