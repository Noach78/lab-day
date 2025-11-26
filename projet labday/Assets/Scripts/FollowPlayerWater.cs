using UnityEngine;

public class FollowPlayerWater : MonoBehaviour
{
    public Transform playerTarget;

    private float waterHeightY;

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        waterHeightY = transform.position.y;
    }

    void LateUpdate()
    {
        if (playerTarget != null)
        {
            Vector3 newPosition = new Vector3(
                playerTarget.position.x, 
                waterHeightY,            
                playerTarget.position.z  
            );

            transform.position = newPosition;
        }
    }
}