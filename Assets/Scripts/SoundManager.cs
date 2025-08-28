using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Clips")]
    public List<AudioClip> musicClips;
    public List<AudioClip> sfxClips;
    public List<AudioClip> uiClips;

    private Dictionary<string, AudioClip> musicDict = new();
    private Dictionary<string, AudioClip> sfxDict = new();
    private Dictionary<string, AudioClip> uiDict = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadDictionaries();
        LoadSavedVolumes();
        UpdateVolumes();
    }

    private void LoadDictionaries()
    {
        foreach (var clip in musicClips)
            if (clip != null) musicDict[clip.name] = clip;

        foreach (var clip in sfxClips)
            if (clip != null) sfxDict[clip.name] = clip;

        foreach (var clip in uiClips)
            if (clip != null) uiDict[clip.name] = clip;
    }

    public void PlayMusic(string clipName, bool loop = true)
    {
        if(ToggleButtonHandler.IsMusicOn() == false)
        {
            StopMusic();
            return;
        }
        if (musicDict.TryGetValue(clipName, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + clipName);
        }
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if(ToggleButtonHandler.IsSoundOn() == false)
            return;

        if (sfxDict.TryGetValue(clipName, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + clipName);
        }
    }

    public void PlayUI(string clipName)
    {
        if (uiDict.TryGetValue(clipName, out var clip))
        {
            uiSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("UI clip not found: " + clipName);
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        UpdateVolumes();
    }

    private void LoadSavedVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    private void UpdateVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        uiSource.volume = sfxVolume; // UI shares sfx volume
    }

    public void MuteAll(bool isMuted)
    {
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;
        uiSource.mute = isMuted;
    }
}
