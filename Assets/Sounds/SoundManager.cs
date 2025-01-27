using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            // attempt to locate the singleton
            if (_instance == null)
            {
                _instance = (SoundManager)FindObjectOfType(typeof(SoundManager));
            }

            // create a new singleton
            if (_instance == null)
            {
                _instance = (new GameObject("SoundManager")).AddComponent<SoundManager>();
            }

            // return singleton
            return _instance;
        }
    }

    public AudioSource _theme;
    public float _themeVolume = 0.5f;

    public AudioSource _whoosh;
    public float _whooshVolume = 0.5f;

    public float _volume = 0.2f;
    public bool _mute = false;
    public bool _initialized = false;

    public IEnumerator Start()
    {
        UpdateVolume(_volume);
        Mute(_mute);

        yield return new WaitForSecondsRealtime(0.5f);
        // play theme in loop
        _theme.loop = true;
        InvokeRepeating(nameof(PlayTheme), 1f, _theme.clip.length + 0.5f);
        if (_whoosh != null) _whooshVolume = _whoosh.volume;
        _initialized = true;
    }

    public void PlayTheme()
    {
        AudioSource.PlayClipAtPoint(_theme.clip, transform.position);
    }

    public void UpdateVolume(float volume)
    {
        _volume = volume;
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            audioSource.volume = volume;
        }
    }

    public void PlayWhoosh(GameEvent opts)
    {
        AudioSource.PlayClipAtPoint(_whoosh.clip, transform.position);
    }

    public void Mute(bool m)
    {
        _theme.mute = m;
        if (_whoosh != null) _whoosh.mute = m;
    }
}