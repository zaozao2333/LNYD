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
            Destroy(gameObject); // �����ظ�ʵ��
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
            Debug.LogError("��Ƶ�ļ�����ʧ��: " + fileName);
        }
    }

    // �ṩ��̬���ʵ�
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
            _audioSource.PlayOneShot(clip); // ����ϵ�ǰ����
        }
    }

    public static void PlayOneShotStatic(string fileName)
    {
        if (_instance != null) _instance.PlayOneShot(fileName);
    }
}