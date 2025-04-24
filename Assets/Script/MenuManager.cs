using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject mainMenuPanel;
    public GameObject jouerPanel;
    public GameObject optionsPanel;

    [Header("Boutons Principaux")]
    public Button buttonJouer;
    public Button buttonOptions;
    public Button buttonQuitter;

    [Header("Boutons Options")]
    public Button buttonChangerTouche1;
    public Button buttonChangerTouche2;
    public Button buttonBack;

    [Header("Sélections par défaut")]
    public GameObject mainFirstSelect;
    public GameObject jouerFirstSelect;
    public GameObject optionsFirstSelect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private GameObject currentPanel;

    void Start()
    {
        // Lier manuellement les events
        buttonJouer.onClick.AddListener(OnClickJouer);
        buttonOptions.onClick.AddListener(OnClickOptions);
        buttonQuitter.onClick.AddListener(OnClickQuitter);
        buttonChangerTouche1.onClick.AddListener(OnClickChangerTouche1);
        buttonChangerTouche2.onClick.AddListener(OnClickChangerTouche2);
        buttonBack.onClick.AddListener(OnClickBack);

        ShowPanel(mainMenuPanel, mainFirstSelect);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current?.startButton.wasPressedThisFrame == true)
        {
            if (currentPanel != mainMenuPanel)
            {
                OnClickBack();
            }
        }
    }

    void ShowPanel(GameObject panelToShow, GameObject buttonToSelect)
    {
        mainMenuPanel.SetActive(false);
        jouerPanel.SetActive(false);
        optionsPanel.SetActive(false);

        panelToShow.SetActive(true);
        currentPanel = panelToShow;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    void PlayClick()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    // --- Actions de boutons ---
    public void OnClickJouer()
    {
        PlayClick();
        ShowPanel(jouerPanel, jouerFirstSelect);
    }

    public void OnClickOptions()
    {
        PlayClick();
        ShowPanel(optionsPanel, optionsFirstSelect);
    }

    public void OnClickQuitter()
    {
        PlayClick();
        Application.Quit();
    }

    public void OnClickChangerTouche1()
    {
        PlayClick();
        Debug.Log("Changer Touche 1");
        // À relier au système de rebind
    }

    public void OnClickChangerTouche2()
    {
        PlayClick();
        Debug.Log("Changer Touche 2");
        // À relier au système de rebind
    }

    public void OnClickBack()
    {
        PlayClick();
        ShowPanel(mainMenuPanel, mainFirstSelect);
    }
}
