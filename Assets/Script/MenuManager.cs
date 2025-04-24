using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Header("Input")]
    public InputActionReference cancelAction;

    [Header("Rebind")]
    public RebindManager rebindmanager;

    private GameObject currentPanel;

    void Start()
    {
        // Lier les événements des boutons
        buttonJouer.onClick.AddListener(OnClickJouer);
        buttonOptions.onClick.AddListener(OnClickOptions);
        buttonQuitter.onClick.AddListener(OnClickQuitter);
        buttonChangerTouche1.onClick.AddListener(OnClickChangerTouche1);
        buttonChangerTouche2.onClick.AddListener(OnClickChangerTouche2);
        buttonBack.onClick.AddListener(OnClickBack);

        // Lier le bouton Cancel
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;

        // Activer le menu principal au démarrage
        ShowPanel(mainMenuPanel, mainFirstSelect);
    }

    private void OnDestroy()
    {
        // Clean listener
        cancelAction.action.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentPanel != mainMenuPanel)
        {
            OnClickBack();
        }
    }

    void ShowPanel(GameObject panelToShow, GameObject buttonToSelect)
    {
        // Désactive tous les panneaux
        mainMenuPanel.SetActive(false);
        jouerPanel.SetActive(false);
        optionsPanel.SetActive(false);

        // Active le panneau demandé
        panelToShow.SetActive(true);
        currentPanel = panelToShow;

        // Focus UI
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    void PlayClick()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    // --- Actions des boutons principaux ---
    public void OnClickJouer()
    {
        PlayClick();
        // Option 1 : affiche le menu de sélection (jouerPanel)
        ShowPanel(jouerPanel, jouerFirstSelect);

        // Option 2 : Lancer directement une scène (décommente cette ligne si tu veux)
        // LoadSceneByName("NomDeTaScene");
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

    // --- Boutons de remapping ---
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

    // --- Retour au menu principal ---
    public void OnClickBack()
    {
        PlayClick();
        ShowPanel(mainMenuPanel, mainFirstSelect);
    }

    // --- Changer de scène (appelable via OnClick) ---
    public void LoadSceneByName(string sceneName)
    {
        PlayClick();
        SceneManager.LoadScene(sceneName);
    }
}
