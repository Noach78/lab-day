using UnityEngine;
using UnityEngine.UI;
using InventoryFramework;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Paramètres de l'Interface (UI)")]
    public GameObject questUI; 
    public Text questProgressText;
    
    [Header("Boutons (UI)")]
    public GameObject acceptButton;
    public GameObject claimButton;
    public GameObject closeButton;

    [Header("Récompenses (Les fragments)")]
    public Item[] mapFragments = new Item[6]; 
    public ItemPickupHandler pickupHandler;

    [Header("Progression des Quêtes")]
    public int currentQuestIndex = 0; 
    public enum QuestState { Available, InProgress, CanComplete, AllFinished }
    public QuestState currentState = QuestState.Available;

    [Header("Objectifs (Variables)")]
    public bool swordBought = false; // NOUVEAU : Pour la quête 1
    
    public int skeletonsToKill = 5;
    public int currentSkeletons = 0;
    
    public int sapToCollect = 3;
    public int currentSap = 0;
    
    public bool golemKilled = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadQuestData();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (questUI == null) questUI = GameObject.Find("QuestCanvas/QuestPanel"); 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) ToggleQuestUI();
        if (Input.GetKeyDown(KeyCode.F12)) 
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Sauvegarde effacée ! Redémarre le jeu.");
        }
    }

    public void ToggleQuestUI()
    {
        if (questUI == null) return;
        bool isOpening = !questUI.activeSelf;
        questUI.SetActive(isOpening);
        if (isOpening) RefreshUI();

        Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpening;
    }

    // --- SYSTÈME DE SAUVEGARDE ---
    private void SaveQuestData()
    {
        PlayerPrefs.SetInt("QuestIndex", currentQuestIndex);
        PlayerPrefs.SetInt("QuestState", (int)currentState);
        PlayerPrefs.SetInt("SwordBought", swordBought ? 1 : 0); // Sauvegarde de l'épée
        PlayerPrefs.SetInt("SkeletonsKilled", currentSkeletons);
        PlayerPrefs.SetInt("SapCollected", currentSap);
        PlayerPrefs.SetInt("GolemKilled", golemKilled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadQuestData()
    {
        currentQuestIndex = PlayerPrefs.GetInt("QuestIndex", 0);
        currentState = (QuestState)PlayerPrefs.GetInt("QuestState", 0);
        swordBought = PlayerPrefs.GetInt("SwordBought", 0) == 1;
        currentSkeletons = PlayerPrefs.GetInt("SkeletonsKilled", 0);
        currentSap = PlayerPrefs.GetInt("SapCollected", 0);
        golemKilled = PlayerPrefs.GetInt("GolemKilled", 0) == 1; 
    }

    // --- LES SIGNAUX DES OBJECTIFS ---

    // NOUVELLE MÉTHODE : Appelée par la boutique
    public void OnSwordBought()
    {
        // On vérifie qu'on est bien à la quête 0
        if (currentState == QuestState.InProgress && currentQuestIndex == 0)
        {
            swordBought = true;
            SaveQuestData();
            SetQuestReady();
        }
    }

    public void AddSkeletonKill()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 1) // Décalé à l'index 1
        {
            currentSkeletons++;
            SaveQuestData(); 
            if (currentSkeletons >= skeletonsToKill) SetQuestReady();
        }
    }

    public void OnSapCollected()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 2) // Décalé à l'index 2
        {
            currentSap++;
            SaveQuestData(); 
            if (currentSap >= sapToCollect) SetQuestReady();
        }
    }

    public void GolemDefeated()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 3) // Décalé à l'index 3
        {
            golemKilled = true;
            SaveQuestData();
            SetQuestReady();
        }
    }

    private void SetQuestReady()
    {
        currentState = QuestState.CanComplete;
        SaveQuestData(); 
        if (questUI.activeSelf) RefreshUI(); 
    }

    // --- BOUTONS UI ---
    public void StartQuest()
    {
        if (currentState == QuestState.Available)
        {
            currentState = QuestState.InProgress;
            SaveQuestData(); 
            RefreshUI(); 
        }
    }

    public void ClaimReward()
    {
        if (currentState == QuestState.CanComplete)
        {
            if (pickupHandler != null && mapFragments.Length > currentQuestIndex && mapFragments[currentQuestIndex] != null)
            {
                pickupHandler.PickupItem(mapFragments[currentQuestIndex], 1);
            }
            
            currentQuestIndex++;
            currentState = (currentQuestIndex >= 6) ? QuestState.AllFinished : QuestState.Available;

            // Réinitialisation de tout
            swordBought = false;
            currentSkeletons = 0;
            currentSap = 0;
            golemKilled = false;

            SaveQuestData(); 
            RefreshUI(); 
        }
    }

    public void ClosePanel()
    {
        if (questUI.activeSelf) ToggleQuestUI();
    }

    // --- GESTION DE L'AFFICHAGE ---
    private void RefreshUI()
    {
        if (questProgressText == null) return;

        if (acceptButton != null) acceptButton.SetActive(false);
        if (claimButton != null) claimButton.SetActive(false);
        if (closeButton != null) closeButton.SetActive(true);

        if (currentState == QuestState.AllFinished)
        {
            questProgressText.text = "Tu as récupéré tous les fragments ! L'aventure t'attend.";
            return;
        }

        string title = "";
        string objectiveText = "";
        string loreText = "";

        if (currentQuestIndex == 0) // NOUVELLE QUÊTE 1
        {
            title = "Quête 1 : S'équiper pour survivre";
            loreText = "Il est trop dangereux de sortir les mains vides. Va voir le marchand et achète une épée.";
            objectiveText = $"- Épée achetée : {(swordBought ? "Oui" : "Non")}";
        }
        else if (currentQuestIndex == 1) // SQUELETTES (devenue Quête 2)
        {
            title = "Quête 2 : La menace des os";
            loreText = "Maintenant que tu es armé, nettoie la zone des squelettes.";
            objectiveText = $"- Squelettes tués : {currentSkeletons}/{skeletonsToKill}";
        }
        else if (currentQuestIndex == 2) // SÈVE (devenue Quête 3)
        {
            title = "Quête 3 : L'or de la forêt";
            loreText = "J'ai besoin de sève magique pour déchiffrer la suite. Fouille les arbres.";
            objectiveText = $"- Sève récoltée : {currentSap}/{sapToCollect}";
        }
        else if (currentQuestIndex == 3) // GOLEM (devenue Quête 4)
        {
            title = "Quête 4 : Le Gardien de Pierre";
            loreText = "Le prochain fragment est gardé par un Golem. Montre ce que tu sais faire !";
            objectiveText = $"- Golem vaincu : {(golemKilled ? "Oui" : "Non")}";
        }

        switch (currentState)
        {
            case QuestState.Available:
                questProgressText.text = $"{title}\n\n{loreText}";
                if (acceptButton != null) acceptButton.SetActive(true);
                break;

            case QuestState.InProgress:
                questProgressText.text = $"{title}\n\nObjectifs :\n{objectiveText}";
                break;

            case QuestState.CanComplete:
                questProgressText.text = $"{title}\n\nBravo ! Reviens me voir pour obtenir ton fragment.";
                if (claimButton != null) claimButton.SetActive(true);
                if (closeButton != null) closeButton.SetActive(false); 
                break;
        }
    }
}