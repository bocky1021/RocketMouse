using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Animator startButton;
    public Animator settingsButton;
    public Animator dialog;
    public Animator contentPanel;
    public Animator gearImage;

    public Slider volumeSdr;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        LoadVolume();
    }

    public void ToggleMenu()
    {
        bool isHidden = contentPanel.GetBool("isHidden");
        contentPanel.SetBool("isHidden", !isHidden);
        gearImage.SetBool("isHidden", !isHidden);
    }

    public void OpenCloseSettings(bool isOpen)
    {
        startButton.SetBool("isHidden", isOpen);
        settingsButton.SetBool("isHidden", isOpen);
        dialog.SetBool("isHidden", !isOpen);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SaveVolume()
    {
        float volume = volumeSdr.value;
        PlayerPrefs.SetFloat("bgVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("bgVolume", 1);
        volumeSdr.value = volume;
    }
}
