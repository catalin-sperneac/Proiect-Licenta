using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStatsData
{
    public int currentLevel;
    public bool isHardMode;
    public bool isEndlessMode;
    public int maxHP;
    public int playerDamage;
    public int coins;
}

[System.Serializable]
public class InventoryNames
{
    public int capacity;
    public List<string> itemNames = new List<string>();
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    public Inventory tempInventory;
    public Inventory savedInventory;
    public PlayerStatsData stats = new PlayerStatsData();
    public List<ItemData> knownItems = new List<ItemData>();

    private string inventoryFilePath;
    private string statsFilePath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            inventoryFilePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
            statsFilePath = Path.Combine(Application.persistentDataPath, "playerStats.json");

            LoadPlayerStats();
            LoadInventory();

            if (PlayerInventoryManager.Instance != null)
            {
                PlayerInventoryManager.Instance.playerInventory = tempInventory;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private ItemData GetItemByName(string name)
    {
        for(int i = 0; i < knownItems.Count; i++)
        {
            if(name == knownItems[i].itemName && knownItems[i] != null)
            {
                ItemData item = Instantiate(knownItems[i]);
                return item;
            }
        }
        return null;
    }

    public void SavePlayerStats()
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(statsFilePath, json);
    }

    public void LoadPlayerStats()
    {
        if(File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            stats = JsonUtility.FromJson<PlayerStatsData>(json);
        }
        else
        {
            stats = new PlayerStatsData()
            {
                currentLevel = 0,
                isHardMode = false,
                isEndlessMode = false,
                maxHP = 5,
                playerDamage = 1,
                coins = 0
            };
        }
    }

    public void ResetPlayerStats()
    {
        stats.currentLevel = 0;
        stats.isHardMode = false;
        stats.isEndlessMode = false;
        stats.maxHP = 5;
        stats.playerDamage = 1;
        stats.coins = 0;
        SavePlayerStats();
    }

    public void SaveInventory()
    {
        InventoryNames names = new InventoryNames();
        names.capacity = savedInventory.capacity;
        foreach (var item in savedInventory.items)
        {
            if (item != null)
                names.itemNames.Add(item.itemName);
        }
        string json = JsonUtility.ToJson(names, true);
        File.WriteAllText(inventoryFilePath, json);
    }

    public void LoadInventory()
    {
        InventoryNames names = new InventoryNames();
        if (File.Exists(inventoryFilePath))
        {
            string json = File.ReadAllText(inventoryFilePath);
            names = JsonUtility.FromJson<InventoryNames>(json);
            if (names.itemNames == null)
                names.itemNames = new List<string>();
        }

        savedInventory = new Inventory(8);
        foreach (var itemName in names.itemNames)
        {
            ItemData item = GetItemByName(itemName);
            if (item != null)
                savedInventory.AddItem(item);
        }

        tempInventory = new Inventory(8);
        foreach (var item in savedInventory.items)
            tempInventory.AddItem(item);

        if (PlayerInventoryManager.Instance != null)
            PlayerInventoryManager.Instance.playerInventory = tempInventory;
    }

    public void ResetTempInventory()
    {
        tempInventory = new Inventory(8);
        foreach (var item in savedInventory.items)
            tempInventory.AddItem(item);

        if (PlayerInventoryManager.Instance != null)
            PlayerInventoryManager.Instance.playerInventory = tempInventory;
    }

    public void ResetSavedInventory()
    {
        tempInventory = new Inventory(8);
        savedInventory = new Inventory(8);
        SaveInventory();
    }

    public void LevelComplete()
    {
        savedInventory = new Inventory(8);
        foreach (var item in tempInventory.items)
        {
            if (item != null && item.itemName != "Key")
                savedInventory.AddItem(item);
        }
        SaveInventory();
        ResetTempInventory();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Menu")
        {
            if (PlayerInventoryManager.Instance != null)
            {
                PlayerInventoryManager.Instance.playerInventory = tempInventory;
            }
        }
    }
}

