using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 敵が出ているときの演出
/// Enemyから直接呼び出す
/// </summary>
public class EnemySpawnUI : MonoBehaviour
{
    public static EnemySpawnUI Instance { get; private set; }

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // 重複した場合は削除
        }
    }

    [SerializeField, Header("敵が出てきた時のパネル")] private Image panelImage;
    [SerializeField, Header("Imageのアルファ値")] private float alphaIncreaseAmount = 0.2f;
    [SerializeField, Header("シェイクの長さ")] private float shakeDuration = 0.5f;
    [SerializeField, Header("シェイクの強さ")] private float shakeMagnitude = 0.1f;
    [SerializeField, Header("揺らすカメラ")] private Transform cameraTransform;

    private Vector3 initialCameraPosition;
    private Color initialPanelColor;
    private float maxAlpha = 1.0f;

    void Start()
    {
        // カメラの初期位置を記録
        initialCameraPosition = cameraTransform.localPosition;

        // パネルの初期色を記録
        initialPanelColor = panelImage.color;
    }

    /// <summary>
    /// 敵が出現した時のパネルのα値変更
    /// </summary>
    public void OnEnemySpawned()
    {
        ChangePanelAlpha(true).Forget();
    }

    /// <summary>
    /// 敵が消えた時のパネルのα値リセット
    /// </summary>
    public void OnEnemyDisappeared()
    {
        ChangePanelAlpha(false).Forget();
    }

    /// <summary>
    /// パネルのα値を変更する
    /// </summary>
    /// <param name="increaseAlpha"></param>
    /// <returns></returns>
    private async UniTask ChangePanelAlpha(bool increaseAlpha)
    {
        Color panelColor = panelImage.color;

        if (increaseAlpha)
        {
            panelColor.a = Mathf.Min(panelColor.a + alphaIncreaseAmount, maxAlpha);
        }
        else
        {
            panelColor.a = initialPanelColor.a;
        }

        panelImage.color = panelColor;

        // アニメーション的な場合はここで遅延を追加可能
        await UniTask.Yield();
    }

    /// <summary>
    /// ダメージを受けた時にカメラを揺らす
    /// </summary>
    public void OnDamageTaken()
    {
        ShakeCamera().Forget();
    }

    /// <summary>
    /// カメラシェイクの処理
    /// </summary>
    /// <returns></returns>
    private async UniTask ShakeCamera()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // ランダムなオフセットを計算
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            cameraTransform.localPosition = initialCameraPosition + randomOffset;

            // シェイクの持続時間を増やす
            elapsed += Time.deltaTime;

            // 1フレーム待機
            await UniTask.Yield();
        }

        // シェイクが終わったらカメラを元の位置に戻す
        cameraTransform.localPosition = initialCameraPosition;
    }
}
