using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private AudioSource _audioSource;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // 避免重复实例
        }
    }

    public void PlayAudio(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + fileName);
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
        else
        {
            Debug.LogError("音频文件加载失败: " + fileName);
        }
    }

    // 提供静态访问点
    public static void Play(string fileName)
    {
        if (_instance != null)
            _instance.PlayAudio(fileName);
    }

    public void PlayOneShot(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + fileName);
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip); // 不打断当前播放
        }
    }

    public static void PlayOneShotStatic(string fileName)
    {
        if (_instance != null) _instance.PlayOneShot(fileName);
    }
}