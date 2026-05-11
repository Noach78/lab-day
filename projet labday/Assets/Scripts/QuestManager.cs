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
    public bool swordBought = false;
    
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

    private void SaveQuestData()
    {
        PlayerPrefs.SetInt("QuestIndex", currentQuestIndex);
        PlayerPrefs.SetInt("QuestState", (int)currentState);
        PlayerPrefs.SetInt("SwordBought", swordBought ? 1 : 0); 
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

    public void OnSwordBought()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 0)
        {
            swordBought = true;
            SaveQuestData();
            SetQuestReady();
        }
    }

    public void AddSkeletonKill()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 1) 
        {
            currentSkeletons++;
            SaveQuestData(); 
            if (currentSkeletons >= skeletonsToKill) SetQuestReady();
        }
    }

    public void OnSapCollected()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 2) 
        {
            currentSap++;
            SaveQuestData(); 
            if (currentSap >= sapToCollect) SetQuestReady();
        }
    }

    public void GolemDefeated()
    {
        if (currentState == QuestState.InProgress && currentQuestIndex == 3)
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

        if (currentQuestIndex == 0)
        {
            title = "Quest 1: Equip to Survive";
            loreText = "It is too dangerous to go out empty-handed. Go see the merchant and buy a sword.";
            objectiveText = $"- Sword bought: {(swordBought ? "Yes" : "No")}";
        }
        else if (currentQuestIndex == 1)
        {
            title = "Quest 2: The Bone Threat";
            loreText = "Now that you are armed, clear the area of skeletons.";
            objectiveText = $"- Skeletons killed: {currentSkeletons}/{skeletonsToKill}";
        }
        else if (currentQuestIndex == 2)
        {
            title = "Quest 3: The Forest's Gold";
            loreText = "I need magic sap to decipher the rest. Search the trees.";
            objectiveText = $"- Sap collected: {currentSap}/{sapToCollect}";
        }
        else if (currentQuestIndex == 3)
        {
            title = "Quest 4: The Stone Guardian";
            loreText = "The next fragment is guarded by a Golem. Show what you can do!";
            objectiveText = $"- Golem defeated: {(golemKilled ? "Yes" : "No")}";
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