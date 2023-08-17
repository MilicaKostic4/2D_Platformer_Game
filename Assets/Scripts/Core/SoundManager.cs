using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        //Muzika u pozadini se nastavlja kada se predje na novu scenu
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //Unistavanje novih zvukova ukoliko jedan vec postoji
        else if(instance != null && instance != this)
            Destroy(gameObject);
    }

    public void PlaySound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }

}
