using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
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

    [Header("Bouton de chargement de scène")]
    public Button loadSceneButton;
    public int sceneIndexToLoad;

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

        // **Lier le bouton de chargement de scène**
        if (loadSceneButton != null)
            loadSceneButton.onClick.AddListener(OnClickLoadScene);
        else
            Debug.LogError("MenuManager : loadSceneButton non assigné dans l'Inspector !");

        // Activer le menu principal au démarrage
        ShowPanel(mainMenuPanel, mainFirstSelect);
    }

    private void OnDestroy()
    {
        cancelAction.action.performed -= OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentPanel != mainMenuPanel)
            OnClickBack();
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

    // --- Actions des boutons principaux ---
    public void OnClickJouer() { PlayClick(); ShowPanel(jouerPanel, jouerFirstSelect); }
    public void OnClickOptions() { PlayClick(); ShowPanel(optionsPanel, optionsFirstSelect); }
    public void OnClickQuitter() { PlayClick(); Application.Quit(); }

    // --- Boutons de remapping ---
    public void OnClickChangerTouche1() { PlayClick(); rebindmanager.StartRebindAction1(); }
    public void OnClickChangerTouche2() { PlayClick(); rebindmanager.StartRebindAction2(); }

    // --- Retour au menu principal ---
    public void OnClickBack() { PlayClick(); ShowPanel(mainMenuPanel, mainFirstSelect); }

    // --- Charge la scène via l’index exposé dans l’Inspector ---
    public void OnClickLoadScene()
    {
        PlayClick();
        if (sceneIndexToLoad >= 0 && sceneIndexToLoad < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(sceneIndexToLoad);
        else
            Debug.LogError($"MenuManager : index de scène invalide ({sceneIndexToLoad}).");
    }
}
