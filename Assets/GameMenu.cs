using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameMenu : MonoBehaviour
{
    public Slider masterAudio;
    public Slider musicAudio;
    public Slider ambientAudio;

    public AudioMixer audio;

    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        float master;
        audio.GetFloat("MasterAttn", out master);
        masterAudio.value = dbToNorm(master);

        float music;
        audio.GetFloat("MusicAttn", out music);
        musicAudio.value = dbToNorm(music);

        float ambient;
        audio.GetFloat("AmbientAttn", out ambient);
        ambientAudio.value = dbToNorm(ambient);

        masterAudio.onValueChanged.AddListener((value) =>
        {
            audio.SetFloat("MasterAttn", normToDb(value));
        });

        musicAudio.onValueChanged.AddListener((value) =>
        {
            audio.SetFloat("MusicAttn", normToDb(value));
        });

        ambientAudio.onValueChanged.AddListener((value) =>
        {
            audio.SetFloat("AmbientAttn", normToDb(value));
        });
    }

        // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (panel.activeSelf)
            {
                Close();
            } else
            {
                Open();
            }
        }
    }

    void Open()
    {
        panel.SetActive(true);
        Pause();
    }

    public void Close()
    {
        panel.SetActive(false);
        Unpause();
    }

    void Pause()
    {
        //Time.timeScale = 0.1f;
    }

    void Unpause()
    {
        //Time.timeScale = 1.0f;
    }

    float normToDb(float norm)
    {
        return 20.0f * Mathf.Log10(norm);
    }

    float dbToNorm(float db)
    {
        return Mathf.Pow(10.0f, db / 20.0f);
    }
}
