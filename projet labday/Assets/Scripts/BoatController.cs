using UnityEngine;
using UnityEngine.SceneManagement;
using InventoryFramework;

public class BoatController : MonoBehaviour
{
    CharacterController cc;
    PlayerMovement playerMovement;
    GameObject player;
    Transform defaultPlayerTransform;

    Rigidbody rb;
    float startY;
    bool isDriving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        cc = GameObject.FindObjectOfType<CharacterController>();
        playerMovement = GameObject.FindObjectOfType<PlayerMovement>();

        if (playerMovement != null)
        {
            player = playerMovement.gameObject;
            defaultPlayerTransform = player.transform.parent;
        }

        startY = gameObject.transform.position.y;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Scene")
        {
            Vector3 savedBoatPos = transform.position; 

            // Charger le Bateau
            if (PlayerPrefs.HasKey("BoatPosX_Scene"))
            {
                float bx = PlayerPrefs.GetFloat("BoatPosX_Scene");
                float by = PlayerPrefs.GetFloat("BoatPosY_Scene");
                float bz = PlayerPrefs.GetFloat("BoatPosZ_Scene");
                float bRotY = PlayerPrefs.GetFloat("BoatRotY_Scene");

                savedBoatPos = new Vector3(bx, by, bz);

                transform.position = savedBoatPos;
                transform.rotation = Quaternion.Euler(0, bRotY, 0);

                if (rb != null) rb.position = savedBoatPos;

                startY = by;
            }

            // Charger le Joueur
            if (PlayerPrefs.HasKey("PlayerPosX_Scene") && player != null)
            {
                float px = PlayerPrefs.GetFloat("PlayerPosX_Scene");
                float py = PlayerPrefs.GetFloat("PlayerPosY_Scene");
                float pz = PlayerPrefs.GetFloat("PlayerPosZ_Scene");
                float pRotY = PlayerPrefs.GetFloat("PlayerRotY_Scene");

                Vector3 savedPlayerPos = new Vector3(px, py + 0.5f, pz); 

                if (cc != null) cc.enabled = false;

                player.transform.position = savedPlayerPos;
                player.transform.rotation = Quaternion.Euler(0, pRotY, 0);

                if (cc != null) cc.enabled = true;

                Physics.SyncTransforms();

                if (Vector3.Distance(savedPlayerPos, savedBoatPos) < 2f)
                {
                    SetDriving(true);
                }
            }
        }
    }

    bool IsPlayerCloseToBoat()
    {
        if (player == null) return false;
        return Vector3.Distance(gameObject.transform.position, player.transform.position) < 5;
    }

    void SetDriving(bool isDriving)
    {
        this.isDriving = isDriving;
        playerMovement.canMove = !isDriving;

        if (cc != null) cc.enabled = !isDriving;

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
            if (Input.GetKeyDown(KeyCode.B))
            {
                string currentScene = SceneManager.GetActiveScene().name;

                SaveManager saveManager = FindAnyObjectByType<SaveManager>();
                if (saveManager != null) saveManager.SaveGame();

                if (currentScene == "Scene")
                {
                    PlayerPrefs.SetFloat("BoatPosX_Scene", transform.position.x);
                    PlayerPrefs.SetFloat("BoatPosY_Scene", transform.position.y);
                    PlayerPrefs.SetFloat("BoatPosZ_Scene", transform.position.z);
                    PlayerPrefs.SetFloat("BoatRotY_Scene", transform.rotation.eulerAngles.y);

                    if (player != null)
                    {
                        PlayerPrefs.SetFloat("PlayerPosX_Scene", player.transform.position.x);
                        PlayerPrefs.SetFloat("PlayerPosY_Scene", player.transform.position.y);
                        PlayerPrefs.SetFloat("PlayerPosZ_Scene", player.transform.position.z);
                        PlayerPrefs.SetFloat("PlayerRotY_Scene", player.transform.rotation.eulerAngles.y);
                    }

                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Demo_Island_1");
                }
                else if (currentScene == "Demo_Island_1")
                {
                    SceneManager.LoadScene("Scene");
                }
            }

            if (rb != null)
            {
                float forwardThrust = 0;
                if (Input.GetKey(KeyCode.W)) forwardThrust = 3;
                if (Input.GetKey(KeyCode.S)) forwardThrust = -1;

                rb.AddForce(gameObject.transform.forward * forwardThrust);

                float turnThrust = 0;
                if (Input.GetKey(KeyCode.A)) turnThrust = -1;
                if (Input.GetKey(KeyCode.D)) turnThrust = 1;

                rb.AddRelativeTorque(Vector3.up * turnThrust);
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 5);
        }

        Vector3 newPosition = gameObject.transform.position;
        newPosition.y = startY + Mathf.Sin(Time.timeSinceLevelLoad * 2) / 8;
        gameObject.transform.position = newPosition;
    }
}