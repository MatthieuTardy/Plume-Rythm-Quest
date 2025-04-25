using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Panneaux principaux")]
    public GameObject mainMenuPanel;
    public GameObject jouerPanel;
    public GameObject optionsPanel;

    [Header("Panneaux Mondes")]
    public GameObject monde1Panel;
    public GameObject monde2Panel;

    [Header("Boutons Principaux")]
    public Button buttonJouer;
    public Button buttonOptions;
    public Button buttonQuitter;

    [Header("Boutons Options")]
    public Button buttonChangerTouche1;
    public Button buttonChangerTouche2;
    public Button buttonBack;

    [Header("Boutons Mondes")]
    public Button buttonMonde1;
    public Button buttonMonde2;

    [Header("Sélections par défaut")]
    public GameObject mainFirstSelect;
    public GameObject jouerFirstSelect;
    public GameObject optionsFirstSelect;
    public GameObject monde1FirstSelect;
    public GameObject monde2FirstSelect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Input")]
    public InputActionReference cancelAction;

    [Header("Rebind")]
    public RebindManager rebindmanager;

    private GameObject currentPanel;

    // ?? Nouvelle pile pour garder l'historique
    private Stack<(GameObject panel, GameObject firstSelect)> panelHistory = new Stack<(GameObject, GameObject)>();

    void Start()
    {
        // Boutons principaux
        buttonJouer.onClick.AddListener(OnClickJouer);
        buttonOptions.onClick.AddListener(OnClickOptions);
        buttonQuitter.onClick.AddListener(OnClickQuitter);
        buttonChangerTouche1.onClick.AddListener(OnClickChangerTouche1);
        buttonChangerTouche2.onClick.AddListener(OnClickChangerTouche2);
        buttonBack.onClick.AddListener(OnClickBack);

        // Boutons Monde
        buttonMonde1.onClick.AddListener(OnClickMonde1);
        buttonMonde2.onClick.AddListener(OnClickMonde2);

        // Cancel action
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;

        // Panel de départ
        ShowPanel(mainMenuPanel, mainFirstSelect, false);
    }

    private void OnDestroy()
    {
        cancelAction.action.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentPanel != mainMenuPanel)
        {
            OnClickBack();
        }
    }

    // ?? ShowPanel avec option pour empiler ou pas
    void ShowPanel(GameObject panelToShow, GameObject buttonToSelect, bool addToHistory = true)
    {
        if (addToHistory && currentPanel != null && currentPanel != panelToShow)
        {
            panelHistory.Push((currentPanel, EventSystem.current.currentSelectedGameObject));
        }

        CloseAllPanels();
        panelToShow.SetActive(true);
        currentPanel = panelToShow;

        EventSystem.current.SetSelectedGameObject(null);
        if (buttonToSelect != null)
            EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    void PlayClick()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    void CloseAllPanels()
    {
        mainMenuPanel.SetActive(false);
        jouerPanel.SetActive(false);
        optionsPanel.SetActive(false);
        monde1Panel.SetActive(false);
        monde2Panel.SetActive(false);
    }

    // --- Boutons principaux ---
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

    // --- Rebinding ---
    public void OnClickChangerTouche1()
    {
        PlayClick();
        rebindmanager.StartRebindAction1();
    }

    public void OnClickChangerTouche2()
    {
        PlayClick();
        rebindmanager.StartRebindAction2();
    }

    // --- Retour contextuel ---
    public void OnClickBack()
    {
        PlayClick();

        if (panelHistory.Count > 0)
        {
            var (previousPanel, previousSelect) = panelHistory.Pop();
            ShowPanel(previousPanel, previousSelect, false);
        }
        else
        {
            ShowPanel(mainMenuPanel, mainFirstSelect, false);
        }
    }

    // --- Navigation vers Mondes ---
    public void OnClickMonde1()
    {
        PlayClick();
        ShowPanel(monde1Panel, monde1FirstSelect);
    }

    public void OnClickMonde2()
    {
        PlayClick();
        ShowPanel(monde2Panel, monde2FirstSelect);
    }

    // --- Changement de scène ---
    public void LoadSceneByName(string sceneName)
    {
        PlayClick();
        SceneManager.LoadScene(sceneName);
    }
}
