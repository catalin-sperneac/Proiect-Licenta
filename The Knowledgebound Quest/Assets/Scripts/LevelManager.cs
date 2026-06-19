using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private GameObject gate;
    private Transform player;
    public int level;
    private bool cleanedUp = false;
    private bool showObjective = false;
    private bool teleportPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        gate = GameObject.FindWithTag("Gate");
        player = GameObject.Find("Player").transform;
        if (!PlayerDataManager.instance.stats.isEndlessMode)
        {
            level = FindLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!showObjective && PlayerDataManager.instance.stats.isEndlessMode)
        {
            StartCoroutine(ShowMessageText("Objective: Defeat the bosses.", 5f));
            showObjective = true;
        }
        if (!PlayerDataManager.instance.stats.isEndlessMode)
        {
            if (level < PlayerDataManager.instance.stats.currentLevel)
            {
                if (!cleanedUp)
                {
                    RemoveAllWithTag("Upgrade");
                    RemoveAllWithTag("Boss");
                    RemoveAllWithTag("Castle Key");
                    RemoveKeyFromChests();
                    cleanedUp = true;
                }
                if (!teleportPlayer && level != 6)
                {
                    if (gate.transform.eulerAngles.y == 180)
                    {
                        player.position = gate.transform.position + new Vector3(0f, 3f, -3f);
                    }
                    else if (gate.transform.eulerAngles.y == 0)
                    {
                        player.position = gate.transform.position + new Vector3(0f, 3f, 3f);
                    }
                    else if (gate.transform.eulerAngles.y == 270)
                    {
                        player.position = gate.transform.position + new Vector3(-3f, 3f, 0f);
                    }
                    else if (gate.transform.eulerAngles.y == 90)
                    {
                        player.position = gate.transform.position + new Vector3(3f, 3f, 0f);
                    }
                    teleportPlayer = true;
                }
            }
            if (!showObjective && level >= PlayerDataManager.instance.stats.currentLevel)
            {
                LevelCompleteConditions lcc;
                if (level != 6)
                {
                    lcc = gate.GetComponent<LevelCompleteConditions>();
                }
                else
                {
                    lcc = GameObject.FindWithTag("Castle Key").GetComponent<LevelCompleteConditions>();
                }
                if (lcc.CheckKeyCondition() && lcc.CheckBossCondition())
                {
                    StartCoroutine(ShowMessageText("Objective: Collect the key and defeat the bosses.", 5f));
                }
                else if (lcc.CheckKeyCondition() && !lcc.CheckBossCondition())
                {
                    StartCoroutine(ShowMessageText("Objective: Collect the key.", 5f));
                }
                else if (!lcc.CheckKeyCondition() && lcc.CheckBossCondition())
                {
                    StartCoroutine(ShowMessageText("Objective: Defeat the bosses.", 5f));
                }
                else if (!lcc.CheckKeyCondition() && !lcc.CheckBossCondition())
                {
                    StartCoroutine(ShowMessageText("Objective: Finish the level.", 5f));
                }
                showObjective = true;
            }
        }
    }

    private void RemoveAllWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }

    private void RemoveKeyFromChests()
    {
        ChestInventoryManager[] chests = GameObject.FindObjectsByType<ChestInventoryManager>(FindObjectsSortMode.None);

        foreach (ChestInventoryManager chest in chests)
        {
            for (int i = chest.chestInventory.items.Count - 1; i >= 0; i--)
            {
                var item = chest.chestInventory.items[i];

                if (item.itemName == "Key")
                {
                    chest.chestInventory.RemoveItem(item);
                }
            }
        }
    }

    private int FindLevel()
    {
        int level;
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Tutorial":
                level = 1;
                break;
            case "Level 1":
                level = 2;
                break;
            case "Level 2":
                level = 3;
                break;
            case "Level 3":
                level = 4;
                break;
            case "Level 4":
                level = 5;
                break;
            case "Castle Interior":
                level = 6;
                break;
            case "Final Level":
                level = 8;
                break;
            default:
                level = -1;
                break;
        }
        return level;
    }

    private IEnumerator ShowMessageText(string text, float seconds)
    {
        UIManager.instance.ShowMessageText(text);
        yield return new WaitForSeconds(seconds);
        UIManager.instance.DeactivateMessageText();
    }
}
