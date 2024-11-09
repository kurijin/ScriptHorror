using UnityEngine;

/// <summary>
/// SoundManager全体を支えるもの、SoundManagerにはSEとBGM用にSourceをわけている
/// SEを流すものは3個用意している,
/// 1個目は主に多くの時間プレイヤーの足音など長く流れるもの.
/// 2個目は主に単発のもの,喰らった音やアイテムゲットなどのシステム音
/// 3個目は敵に関するもの
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSourceBGM; 
    public AudioSource audioSourceSE;  
    public AudioSource audioSourceSE2;  
    public AudioSource audioSourceSE3; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); 

            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSourceBGM = audioSources[0];
            audioSourceSE = audioSources[1];
            audioSourceSE2 = audioSources[2];
            audioSourceSE3 = audioSources[3];
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// BGMに関するメソッド
    /// </summary>
    /// <param name="_bgmClip">BGMの名前(他スクリプトから渡す)</param>
    public void PlayBGM(AudioClip _bgmClip)
    {
        audioSourceBGM.clip = _bgmClip;  
        audioSourceBGM.Play();         
    }
    public void StopBGM()
    {
        audioSourceBGM.Stop();         
    }

    /// <summary>
    ///  SEに関するメソッド
    /// </summary>
    /// <param name="_seClip">SEの名前(他スクリプトから渡す)</param>
    public void PlaySE(AudioClip _seClip)
    {
        audioSourceSE.PlayOneShot(_seClip);
    }
    public void StopSE()
    {
         audioSourceSE.Stop();
    }
    public void PlaySE2(AudioClip _seClip)
    {
         audioSourceSE2.PlayOneShot(_seClip);
    }
    public void StopSE2()
    {
         audioSourceSE2.Stop();
    }
    public void PlaySE3(AudioClip _seClip)
    {
         audioSourceSE3.PlayOneShot(_seClip);
    }
    public void StopSE3()
    {
         audioSourceSE3.Stop();
    }
}
