using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class S_AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip[] _Clips;


    static bool g_AudioSource;
    static AudioSource _AudioSource;
    static AudioSource m_AudioSource
    {
        get
        {
            if (!g_AudioSource)
            {
                GameObject _AudioManager = GameObject.FindWithTag("S_AudioManager");
                if (_AudioManager) _AudioSource = _AudioManager.GetComponent<AudioSource>();
                else
                {
                    _AudioManager = new GameObject("S_AudioManager", typeof(AudioSource));
                    _AudioManager.tag = "S_AudioManager";
                    _AudioSource = _AudioManager.GetComponent<AudioSource>();
                }
                g_AudioSource = true;
            }
            return _AudioSource;
        }
    }

    internal static void PlayOneShoot(AudioClip _clip, float _volume = 1.0f)
    {
        m_AudioSource.PlayOneShot(_clip, _volume);
    }

    private void Awake()
    {
        StartCoroutine(RandomPlayInfinity());
    }

    IEnumerator RandomPlayInfinity()
    {
        while (true)
        {
            int _RandomIndex = Random.Range(0, _Clips.Length - 1);
            m_AudioSource.clip = _Clips[_RandomIndex];
            m_AudioSource.Play();
            yield return new WaitForSeconds(m_AudioSource.clip.length);
        }
    }
}
