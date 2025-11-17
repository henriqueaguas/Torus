using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    private const string LOG_FILE_BASENAME = "playtesting.log";
    [HideInInspector] public static GlobalManager Instance;
    [HideInInspector] public List<Ability.AbilityType> playerAbilities = new List<Ability.AbilityType>();
    [HideInInspector] public Dictionary<Ability.AbilityType, string> abilities = new Dictionary<Ability.AbilityType, string>();
    [HideInInspector] public Dictionary<string, Ability.AbilityType> houseAbilities = new Dictionary<string, Ability.AbilityType>();
    [HideInInspector] public Ability.AbilityType currentAbility = Ability.AbilityType.None;
    [HideInInspector] public int lastCompletedHouse = 0;
    [HideInInspector] public int nextHouseID = 1;
    private string nextHouse = "house 1";
    private GameObject player;
    [HideInInspector] public int currentHouseTimerMs = 0;
    [HideInInspector] public bool isTimerRunning = false;
    [HideInInspector] public int instrDeaths = 0;
    [HideInInspector] public int instrCheckpoints = 0;
    [HideInInspector] public int instrJumpAbilityUsed = 0;
    [HideInInspector] public int instrGrabAbilityUsed = 0;
    [HideInInspector] public int instrFlashedEntity = 0;
    [HideInInspector] public string nextTarget;

    [HideInInspector] public event Action OnHotkeyAbilityListChanged;
    private string logFilePath;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            abilities.Add(Ability.AbilityType.Jump, "Jump");
            abilities.Add(Ability.AbilityType.Grab, "Grab");
            houseAbilities.Add("house 1", Ability.AbilityType.Jump);
            houseAbilities.Add("house 2", Ability.AbilityType.Grab);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "[" + GetTimestamp() + "] " + LOG_FILE_BASENAME);
    }

    void Update()
    {
        if (isTimerRunning)
            currentHouseTimerMs += Mathf.FloorToInt(Time.deltaTime * 1000);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public bool HasCompletedAllHouses()
    {
        return lastCompletedHouse == abilities.Count;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool TryEnterHouse(string houseID)
    {
        if (houseAbilities.TryGetValue(houseID, out Ability.AbilityType ability))
        {
            if (playerAbilities.Contains(ability))
            {
                StartTimer();
                return true;
            }
        }

        return false;
    }

    public void CompleteHouse(string house)
    {
        int houseID = GetIDByHouse(house);

        if (houseID >= lastCompletedHouse)
        {
            StopTimer();
            GetPlayerIfNotExists();
            lastCompletedHouse = houseID;
            nextHouseID = houseID + 1;
            nextHouse = GetHouseByID(nextHouseID);
            houseAbilities.TryGetValue(nextHouse, out Ability.AbilityType ability);
            abilities.TryGetValue(ability, out string abilityTag);
            nextTarget = abilityTag;
        }
    }

    public void CaptureAbility(GameObject ability)
    {
        GetPlayerIfNotExists();
        Ability.AbilityType abilityType = ability.GetComponent<Ability>().GetAbilityType();
        playerAbilities.Add(abilityType);
        OnHotkeyAbilityListChanged?.Invoke();
        nextTarget = nextHouse;
    }

    public void CaptureAllAbilitiesHACK()
    {
        GetPlayerIfNotExists();
        playerAbilities.AddRange(abilities.Keys);
        OnHotkeyAbilityListChanged?.Invoke();
        nextTarget = nextHouse;
    }

    private int GetIDByHouse(string houseID)
    {
        return int.Parse(houseID.Split(' ')[1]);
    }

    private string GetHouseByID(int houseID)
    {
        return "house " + houseID;
    }

    void GetPlayerIfNotExists()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");

            if (player == null)
            {
                Debug.LogWarning("Player not found");
            }
        }
    }

    public void StartTimer()
    {
        currentHouseTimerMs = 0;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // private void ResetTimer()
    // {
    //     currentHouseTimerMs = 0;
    //     isTimerRunning = false;
    // }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentAbility = Ability.AbilityType.None;
        if (scene.name == "Village")
        {
            StopTimer();

            // if (UI_InGame.prevTimerString.Length != 0)
            //     WriteToLog(UI_InGame.prevTimerString);
        }

        // Just entered a new house
        if (scene.name.EndsWith("-1"))
        {
            instrCheckpoints = 0;
            instrJumpAbilityUsed = 0;
            instrGrabAbilityUsed = 0;
            instrDeaths = 0;
            instrFlashedEntity = 0;
        }
    }

    public void SetAbility(Ability.AbilityType abilityType)
    {
        currentAbility = abilityType;
    }

    private void WriteToLog(string logText)
    {
        Debug.Log("Writing to log file: " + logFilePath + " | Content: " + logText);

        if (!File.Exists(logFilePath))
        {
            // Create + write
            using (StreamWriter sw = File.CreateText(logFilePath))
                sw.WriteLine(GetTimestamp() + "\n" + logText + "\n");
        }
        else
        {
            // Append
            using (StreamWriter sw = File.AppendText(logFilePath))
                sw.WriteLine(GetTimestamp() + "\n" + logText + "\n");
        }
    }

    string GetTimestamp()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    }
}
