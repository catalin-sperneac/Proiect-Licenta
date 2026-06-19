using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    private bool playerInRange = false;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIManager.instance.ShowInteractText("Press E to start boss fight");
            playerInRange = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !UIManager.instance.isGameOver && !InventoryManager.instance.isOpen && playerInRange)
        {
            UIManager.instance.DeactivateInteractText();
            BattleManager.instance.StartBossBattle(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.instance.DeactivateInteractText();
        }
    }
}
