using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    SFX,
    Ambient,
    Music
}

[System.Serializable]
public struct SFXData
{
    public string id;
    public AudioClip[] clips;
    public float cooldown;
}

public class AudioSystem : MonoBehaviour, IGameSystem
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Setup")]
    [SerializeField] private List<SFXData> sfxDatabase = new List<SFXData>();

    private Dictionary<string, SFXData> sfxDictionary = new Dictionary<string, SFXData>();
    private Dictionary<string, float> lastPlayTime = new Dictionary<string, float>();


    public bool MuteSFX { get; set; }
    public bool MuteAmbient { get; set; }
    public bool MuteMusic { get; set; }

    public float SFXVolume { get; set; } = 1.0f;
    public float AmbientVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 1.0f;

    private void Awake()
    {
        foreach (var sfx in sfxDatabase)
        {
            if (!string.IsNullOrEmpty(sfx.id) && !sfxDictionary.ContainsKey(sfx.id))
            {
                sfxDictionary.Add(sfx.id, sfx);
            }
        }
    }
    
    public void PlaySFX(string sfxId)
    {
        if (MuteSFX) return;

        if (sfxDictionary.TryGetValue(sfxId, out var sfxData))
        {
            if (CanPlaySFX(sfxData))
            {
                if (sfxData.clips != null && sfxData.clips.Length > 0)
                {
                    var clip = sfxData.clips[Random.Range(0, sfxData.clips.Length)];
                    
                    if (clip != null)
                    {
                        Vector3 playPosition = Camera.main.transform.position + Vector3.forward;
                        AudioSource.PlayClipAtPoint(clip, playPosition, SFXVolume);
                        
                        lastPlayTime[sfxId] = Time.time;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"SFX с ID '{sfxId}' не найден в базе данных AudioSystem.");
        }
    }

    private bool CanPlaySFX(SFXData sfxData)
    {
        if (!lastPlayTime.TryGetValue(sfxData.id, out float lastTimePlayed))
            return true;

        return (Time.time - lastTimePlayed >= sfxData.cooldown);
    }
    
    public void PlayAmbient(AudioClip ambient)
    {
        if (ambient == null) return;

        ambientSource.clip = ambient;
        ambientSource.loop = true;
        ambientSource.volume = AmbientVolume;
        
        if (!MuteAmbient)
        {
            ambientSource.Play();
        }
    }
    
    public void PlayMusic(AudioClip music)
    {
        if (music == null) return;

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.volume = MusicVolume;

        if (!MuteMusic)
        {
            musicSource.Play();
        }
    }

    public void SetVolume(AudioType type, float volume)
    {
        volume = Mathf.Clamp01(volume); 

        switch (type)
        {
            case AudioType.SFX:
                SFXVolume = volume;
                break;
            case AudioType.Ambient:
                AmbientVolume = volume;
                if (ambientSource != null) ambientSource.volume = AmbientVolume;
                break;
            case AudioType.Music:
                MusicVolume = volume;
                if (musicSource != null) musicSource.volume = MusicVolume;
                break;
        }
    }

    public void Mute(AudioType type, bool mute)
    {
        switch (type)
        {
            case AudioType.SFX:
                MuteSFX = mute;
                break;
            case AudioType.Ambient:
                MuteAmbient = mute;
                if (ambientSource != null) ambientSource.enabled = !mute;
                break;
            case AudioType.Music:
                MuteMusic = mute;
                if (musicSource != null) musicSource.enabled = !mute;
                break;
        }
    }

    public void OnAdded() { }

    public void OnGameStarted() { }
}