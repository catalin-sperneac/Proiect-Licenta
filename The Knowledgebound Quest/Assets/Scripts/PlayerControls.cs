using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerControls : MonoBehaviour
{
    private float walkSpeed = 4.0f;
    private float sprintSpeed = 7.5f;
    public float maxStamina = 100;
    private float staminaRegen = 15;
    private float staminaDrain = 20;
    public float currentStamina;
    private bool canSprint;
    public Transform cameraTransform;
    private Coroutine regenCoroutine;
    private Rigidbody playerRb;
    private float currentSpeed;
    private Animator playerAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();
        currentStamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!BattleManager.instance.isBattleActive && !UIManager.instance.isGameOver)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (!ChestTrigger.isChestQuestionActive && !UpgradeItemTrigger.isUpgradeQuestionActive)
        {
            if (currentStamina > 0)
            {
                canSprint = true;
            }
            else
            {
                canSprint = false;
            }
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            Vector3 movement = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
            if(movement.magnitude > 1f)
            {
                movement.Normalize();
            }
            Vector3 newPos = playerRb.position + movement * currentSpeed * Time.deltaTime;
            playerRb.MovePosition(newPos);
            float moveMagnitude = movement.magnitude;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && canSprint;
            if (moveMagnitude < 0.1f)
            {
                playerAnim.SetFloat("Run", 0f);
            }
            else if (moveMagnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                if(!isSprinting)
                {
                    playerAnim.SetFloat("Run", 0.5f);
                }
                else
                {
                    playerAnim.SetFloat("Run", 1f);
                }
            }
            if (isSprinting)
            {
                currentSpeed = sprintSpeed;
                currentStamina -= staminaDrain * Time.deltaTime;
                if (regenCoroutine != null)
                {
                    StopCoroutine(regenCoroutine);
                    regenCoroutine = null;
                }
            }
            else
            {
                currentSpeed = walkSpeed;
                if (currentStamina < maxStamina && regenCoroutine == null)
                {
                    regenCoroutine = StartCoroutine(RegenerateStamina());
                }
            }
        }
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(5);
        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            yield return null;
        }
        regenCoroutine = null;
    }
}
