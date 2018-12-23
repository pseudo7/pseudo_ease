using UnityEngine;

[System.Serializable]
public class Sound
{
    public bool isMusic;

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = .8f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    [Range(0f, 1f)]
    public float spatialBlend = .7f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;

}
