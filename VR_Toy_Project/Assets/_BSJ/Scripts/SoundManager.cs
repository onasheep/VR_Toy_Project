using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Sound[] effectSounds;    // 효과음 오디오 클립들
    public Sound[] bgmSounds;       // BGM 오디오 클립들

    public AudioSource audioSource_BGMPlayer; // BGM 재생기.
    public AudioSource[] audioSource_MP3Player; // MP3 재생기. 효과음은 동시에 여러개 재생될 수 있으므로 배열로 선언

    public string[] playSoundName;  // 재생중인 효과음 사운드 이름 배열

    private void Start()
    {
        playSoundName = new string[audioSource_MP3Player.Length];
    }

    // 효과음을 재생하는 함수
    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            // 동일한 효과음이 있다면
            if (_name == effectSounds[i].name)
            {
                for (int j = 0; j < audioSource_MP3Player.Length; j++)
                {
                    // 실행되고 있지 않은 mp3 재생기
                    if (audioSource_MP3Player[j].isPlaying == false)
                    {
                        audioSource_MP3Player[j].clip = effectSounds[i].clip;
                        audioSource_MP3Player[j].Play();
                        playSoundName[j] = effectSounds[i].name;
                        return;
                    }
                }
            }
        }
    }

    // BGM을 재생하는 함수
    public void PlayBGM(string _name)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (_name == bgmSounds[i].name)
            {
                audioSource_BGMPlayer.clip = bgmSounds[i].clip;
                audioSource_BGMPlayer.Play();
                return;
            }
        }
    }

    // BGM 정지
    public void StopBGM()
    {
        audioSource_BGMPlayer.Stop();
    }

    // 모든 효과음 정지
    public void StopAllSE()
    {
        for (int i = 0; i < audioSource_MP3Player.Length; i++)
        {
            audioSource_MP3Player[i].Stop();
        }
    }

    // 효과음 정지
    public void StopSE(string _name)
    {
        // 동일한 효과음이 있다면 정지
        for (int i = 0; i < audioSource_MP3Player.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSource_MP3Player[i].Stop();
                break;
            }
        }
    }

    // 효과음 볼륨 조절
    public void SetSEVolume(string _name, float _volume)
    {
        // 동일한 효과음이 있다면 볼륨 조절
        for (int i = 0; i < audioSource_MP3Player.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSource_MP3Player[i].volume = _volume;
                break;
            }
        }
    }

    // 배경음 볼륨 조절
    public void SetBGMVolume(float _volume)
    {
        audioSource_BGMPlayer.volume = _volume;
    }
}
