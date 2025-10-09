using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public class MenuUIManager : MonoBehaviour
{
    [Header("UI - Información del Jugador")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerIdText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private Image expFillImage;
    [SerializeField] private TMP_Text skillPointsText;

    [Header("UI - Estadísticas")]
    [SerializeField] private TMP_Text fightText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text magicText;
    [SerializeField] private TMP_Text itemText;

    [Header("Paneles")]
    [SerializeField] private GameObject editNamePanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject assignStatsPanel;

    [Header("Botones")]
    [SerializeField] private Button editNameButton;
    [SerializeField] private Button saveNameButton;
    [SerializeField] private Button cancelNameButton;

    [SerializeField] private Button assignStatsButton;
    [SerializeField] private Button saveStatsButton;

    [Header("Botones de Stats")]
    [SerializeField] private Button addFightBtn, subFightBtn;
    [SerializeField] private Button addDefenseBtn, subDefenseBtn;
    [SerializeField] private Button addMagicBtn, subMagicBtn;
    [SerializeField] private Button addItemBtn, subItemBtn;

    [Header("Sistema")]
    [SerializeField] private PlayerStatsSystem playerStatsSystem;
    private PlayerData player;

    private void Start()
    {
        player = PlayerDataHandler.Instance.CurrentData;
        UpdateUI();
        SetupButtons();

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!editNamePanel.activeSelf && !assignStatsPanel.activeSelf)
            {
                GainExpFromClick();
            }
        }
    }
    private async void GainExpFromClick()
    {
        int gainedExp = 10; 
        playerStatsSystem.GainExperience(gainedExp);
        UpdateUI();
    }
    private void SetupButtons()
    {
        //  nombre
        editNameButton.onClick.AddListener(() => editNamePanel.SetActive(true));
        cancelNameButton.onClick.AddListener(() => editNamePanel.SetActive(false));
        saveNameButton.onClick.AddListener(async () => await SaveNewName());

        //  stats
        assignStatsButton.onClick.AddListener(OpenAssignStats);
        saveStatsButton.onClick.AddListener(async () => await SaveStats());

        // Control  stats
        addFightBtn.onClick.AddListener(() => ModifyStat(ref player.fight, 1));
        subFightBtn.onClick.AddListener(() => ModifyStat(ref player.fight, -1));

        addDefenseBtn.onClick.AddListener(() => ModifyStat(ref player.defense, 1));
        subDefenseBtn.onClick.AddListener(() => ModifyStat(ref player.defense, -1));

        addMagicBtn.onClick.AddListener(() => ModifyStat(ref player.magic, 1));
        subMagicBtn.onClick.AddListener(() => ModifyStat(ref player.magic, -1));

        addItemBtn.onClick.AddListener(() => ModifyStat(ref player.item, 1));
        subItemBtn.onClick.AddListener(() => ModifyStat(ref player.item, -1));
    }

    private void ModifyStat(ref int stat, int amount)
    {
        if (amount > 0 && player.skillPoints > 0)
        {
            stat += 1;
            player.skillPoints -= 1;
        }
        else if (amount < 0)
        {
            stat = Mathf.Max(1, stat - 1);
            player.skillPoints += 1;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        playerNameText.text = player.playerName;
        playerIdText.text = "ID: " + AuthenticationService.Instance.PlayerId;

        levelText.text = $"Nivel {player.level}";
        skillPointsText.text = $"Points: {player.skillPoints}";

        int requiredExp = GetRequiredExpForNextLevel();
        float fillAmount = Mathf.Clamp01((float)player.experience / requiredExp);
        expFillImage.fillAmount = fillAmount;
        expText.text = $"{player.experience} / {requiredExp}";

        fightText.text = player.fight.ToString();
        defenseText.text = player.defense.ToString();
        magicText.text = player.magic.ToString();
        itemText.text = player.item.ToString();

        if (player.skillPoints <= 0)
        {
            ToggleStatButtons(true);
        }
    }

    private int GetRequiredExpForNextLevel()
    {
        int baseExp = 10;          
        float growthFactor = 1.25f; 
        return Mathf.RoundToInt(baseExp * Mathf.Pow(growthFactor, player.level - 1) * player.level);
    }


    private async Task SaveNewName()
    {
        if (string.IsNullOrWhiteSpace(nameInputField.text)) return;

        await AuthManager.Instance.UpdatePlayerName(nameInputField.text);
        player.playerName = nameInputField.text;

        await PlayerDataHandler.Instance.SavePlayerData();
        UpdateUI();

        editNamePanel.SetActive(false);
    }

    private void OpenAssignStats()
    {
        assignStatsPanel.SetActive(true);

        if (player.skillPoints > 0)
            ToggleStatButtons(true);
        else
            ToggleStatButtons(false, showSaveButton: true);

    }

    private async Task SaveStats()
    {
        assignStatsPanel.SetActive(false);

        ToggleStatButtons(false, showSaveButton: false);

        await PlayerDataHandler.Instance.SavePlayerData();
        UpdateUI();
    }

    private void ToggleStatButtons(bool state, bool showSaveButton = false)
    {
        addFightBtn.gameObject.SetActive(state);
        subFightBtn.gameObject.SetActive(state);
        addDefenseBtn.gameObject.SetActive(state);
        subDefenseBtn.gameObject.SetActive(state);
        addMagicBtn.gameObject.SetActive(state);
        subMagicBtn.gameObject.SetActive(state);
        addItemBtn.gameObject.SetActive(state);
        subItemBtn.gameObject.SetActive(state);
        saveStatsButton.gameObject.SetActive(showSaveButton || state);
    }
}
