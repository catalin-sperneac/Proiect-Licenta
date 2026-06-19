using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float height = 40f;
    [SerializeField] private float minHeight = 10f;
    [SerializeField] private float maxHeight = 100f;
    private float scrollSpeed = 10f;

    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    
    void Update()
    {
        if (gameObject.activeSelf)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll == 0)
            {
                return;
            }
            height -= scroll * scrollSpeed;
            height = Mathf.Clamp(height, minHeight, maxHeight);
        }
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            if (player == null)
            {
                return;
            }
            Vector3 pos = transform.position;
            pos.x = player.position.x;
            pos.z = player.position.z;
            pos.y = height;
            transform.position = pos;
        }
    }
}
