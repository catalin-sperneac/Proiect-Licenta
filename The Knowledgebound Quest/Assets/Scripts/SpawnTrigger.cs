using UnityEngine;
using System.Collections;

public class SpawnTrigger : MonoBehaviour
{
    private Coroutine spawnRoutine;
    public static int playerInsideCount = 0;

    void Awake()
    {
        playerInsideCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCount++;
            if (spawnRoutine == null)
            {
                spawnRoutine = StartCoroutine(SpawnCheck());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCount--;
            if(playerInsideCount <= 0)
            {
                playerInsideCount = 0;
                if(spawnRoutine != null)
                {
                    StopCoroutine(spawnRoutine);
                    spawnRoutine = null;
                }
            }
        }
    }

    private IEnumerator SpawnCheck()
    {
        while (playerInsideCount > 0)
        {
            if (!BattleManager.instance.isBattleActive && !BattleManager.instance.battleCooldown && !UIManager.instance.isGameOver && !InventoryManager.instance.isOpen)
            {
                BattleManager.instance.ActivateEnemySpawn();
            }
            yield return new WaitForSeconds(1f);
        }
        spawnRoutine = null;
    }
}
