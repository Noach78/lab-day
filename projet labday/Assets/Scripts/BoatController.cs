using UnityEngine;

public class BoatController : MonoBehaviour
{
    CharacterController cc;
    PlayerMovement playerMovement;
    GameObject player;
    Transform defaultPlayerTransform;

    float startY;

    bool isDriving = false;

    void Start()
    {
        cc = GameObject.FindObjectOfType<CharacterController>();
        playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        
        if (playerMovement != null)
        {
            player = playerMovement.gameObject;
            defaultPlayerTransform = player.transform.parent;
        }
        startY = gameObject.transform.position.y;
    }

    bool IsPlayerCloseToBoat()
    {
        if (player == null)
        {
            return false;
        }
        
        float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        return distance < 5; 
    }

    void SetDriving(bool isDriving)
    {
        this.isDriving = isDriving;
        playerMovement.canMove = !isDriving;
        cc.enabled = !isDriving;
        
        if (isDriving)
        {
            player.transform.parent = gameObject.transform;
            player.transform.localPosition = Vector3.zero;
        }
        else
        {
            player.transform.parent = defaultPlayerTransform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPlayerCloseToBoat())
            {
                SetDriving(!isDriving);
            }
        }
        if (isDriving)
        {
            float forwardThrust = 0;
            if (Input.GetKey(KeyCode.W))
            {
                forwardThrust = 3;
            }
            if (Input.GetKey(KeyCode.S))
            {
                forwardThrust = -1;
            }

            GetComponent<Rigidbody>().AddForce(gameObject.transform.forward*forwardThrust);

            float turnThrust = 0;
            if (Input.GetKey(KeyCode.A))
            {
                turnThrust = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                turnThrust = 1;
            }
            GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up*turnThrust);
        }
        GetComponent<Rigidbody>().linearVelocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().linearVelocity,5);

        Vector3 newPosition = gameObject.transform.position;
        newPosition.y = startY + Mathf.Sin(Time.timeSinceLevelLoad*2)/8;
        gameObject.transform.position = newPosition;
    }
}