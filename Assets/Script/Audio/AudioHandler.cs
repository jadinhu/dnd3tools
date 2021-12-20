/**
* AudioHandler.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 25/10/18 (dd/mm/yy)
* Revised on: 14/12/21 (dd/mm/yy)
*/
using UnityEngine;

/// <summary>
/// List of sounds 
/// </summary>
public enum AudioList { FavoriteSpell, RemoveFavoriteSpell, OpenBook, CloseBook, OpenSpellDescription, CloseSpellDescription }

/// <summary>
/// Manages the the sounds of application
/// </summary>
public class AudioHandler : MonoBehaviour
{
    /// <summary>
    /// Singleton of this class
    /// </summary>
    public static AudioHandler Instance { get; private set; }
    /// <summary>
    /// Sound for put a spell to favorite list
    /// </summary>
    [SerializeField]
    AudioClip favoriteSpell;
    /// <summary>
    /// Sound for remove a spell from favorite list
    /// </summary>
    [SerializeField]
    AudioClip removeFavoriteSpell;
    /// <summary>
    /// Sound for open a spellbook (favorite list)
    /// </summary>
    [SerializeField]
    AudioClip openSpellBook;
    /// <summary>
    /// Sound for close a spellbook (favorite list)
    /// </summary>
    [SerializeField]
    AudioClip closeSpellBook;
    /// <summary>
    /// Sound for open a spell description 
    /// </summary>
    [SerializeField]
    AudioClip openSpellDescription;
    /// <summary>
    /// Sound for close a spell description 
    /// </summary>
    [SerializeField]
    AudioClip closeSpellDescription;
    /// <summary>
    /// The audiosource used in all sounds
    /// </summary>
    AudioSource audioSource;
    /// <summary>
    /// If the audio option is enable by user
    /// </summary>
    bool audioIsOn = true;

    void Awake()
    {
        Setup();
    }

    /// <summary>
    /// Set the singleton <see cref="Instance"/> and get <see cref="audioIsOn"/> user's option
    /// </summary>
    void Setup()
    {
        Instance = Instance != null ? Instance : this;
        audioSource = GetComponent<AudioSource>();
        audioIsOn = PlayerPrefs.GetInt("PanelSettings-sound") > 0;
    }

    /// <summary>
    /// Change <see cref="audioIsOn"/>
    /// </summary>
    public void SetAudioOn(bool on)
    {
        audioIsOn = on;
    }

    /// <summary>
    /// Plays a <see cref="AudioClip"/> from <see cref="AudioList"/>
    /// </summary>
    /// <param name="clip">the sound to play</param>
    public void Play(AudioList clip)
    {
        if (!audioIsOn)
            return;
        switch (clip)
        {
            case AudioList.FavoriteSpell:
                if (favoriteSpell) audioSource.PlayOneShot(favoriteSpell);
                break;
            case AudioList.RemoveFavoriteSpell:
                if (removeFavoriteSpell) audioSource.PlayOneShot(removeFavoriteSpell);
                break;
            case AudioList.OpenBook:
                if (openSpellBook) audioSource.PlayOneShot(openSpellBook);
                break;
            case AudioList.CloseBook:
                if (closeSpellBook) audioSource.PlayOneShot(closeSpellBook);
                break;
            case AudioList.OpenSpellDescription:
                if (openSpellDescription) audioSource.PlayOneShot(openSpellDescription);
                break;
            case AudioList.CloseSpellDescription:
                if (closeSpellDescription) audioSource.PlayOneShot(closeSpellDescription);
                break;
        }
    }
}