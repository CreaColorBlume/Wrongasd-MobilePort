using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovementScript : MonoBehaviour
{
    Vector3 
        up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination, direction;

    public float speed = 5f;
    float rayLength = 1f;

    bool canMove;

    public GameObject player;
    public GameObject playerDeath;

    public Vector3 startPosition;  // GameObject current position at start is the start position

    public List<string> wasdKeys;

    // mostly copied from tutorial cuz I don't have time to fully understand what I'm doing
    [FMODUnity.EventRef]
    public string moveSound;
    FMOD.Studio.EventInstance playerMoveSound;

    public List<KeyCode> pressForSound;

    void Start()
    {
        playerMoveSound = FMODUnity.RuntimeManager.CreateInstance(moveSound);   // playerMoveSound instance is equal to moveSound FMOD Event
        
        currentDirection = up;
        nextPos = Vector3.forward;
        destination = transform.position;

        startPosition = transform.position;
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerMoveSound, GetComponent<Transform>(), GetComponent<Rigidbody>());     // Necessary for 3D sound
        Move();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Application Quit");
        }
    }

    void MoveDirection(int index)
    {
        if (index.Equals(0))
        {
            moveUp();
        }
        if (index.Equals(1))
        {
            moveLeft();
        }
        if (index.Equals(2))
        {
            moveDown();
        }
        if (index.Equals(3))
        {
            moveRight();
        }
        shuffleList();

        void moveUp()
        {
            nextPos = Vector3.forward;
            currentDirection = up;
            canMove = true;
            playerMoveSound.start();    // Play move sound

        }

        void moveDown()
        {
            nextPos = Vector3.back;
            currentDirection = down;
            canMove = true;
            playerMoveSound.start();    // Play move sound

        }

        void moveRight()
        {
            nextPos = Vector3.right;
            currentDirection = right;
            canMove = true;
            playerMoveSound.start();    // Play move sound

        }

        void moveLeft()
        {
            nextPos = Vector3.left;
            currentDirection = left;
            canMove = true;
            playerMoveSound.start();    // Play move sound

        }


        void shuffleList()  // Shuffles list of wasdKeys... How does it work? Who knows, it might as well be magic
        {
            for (int i = 0; i < wasdKeys.Count; i++)
            {
                string temp = wasdKeys[i];
                int randomIndex = Random.Range(i, wasdKeys.Count);
                wasdKeys[i] = wasdKeys[randomIndex];
                wasdKeys[randomIndex] = temp;
            }
        }

        if (Vector3.Distance(destination, transform.position) <= 0.1f)
        {
            transform.localEulerAngles = currentDirection;   // Rotation

            if (canMove)   // Movement
            {
                if (Valid())
                {
                    destination = transform.position + nextPos;
                    direction = nextPos;
                    canMove = false;
                }
            }
        }
    }

    void Move() 
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveDirection(wasdKeys.IndexOf("W"));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveDirection(wasdKeys.IndexOf("A"));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDirection(wasdKeys.IndexOf("S"));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveDirection(wasdKeys.IndexOf("D"));
        }
    }

    bool Valid()
    {
        Ray myRay = new Ray(transform.position + new Vector3(0, 0.2f, 0), transform.forward);
        RaycastHit hit;

        Debug.DrawRay(myRay.origin, myRay.direction, Color.cyan);

        if(Physics.Raycast(myRay,out hit,rayLength))
        {
            if(hit.collider.tag == "Wall")      // If ray hits wall with tag "wall" then...
            {
                Instantiate(player, startPosition, Quaternion.identity);  // ...instantiate new player at startposition and destroy this player
                Instantiate(playerDeath, transform.position, transform.rotation);
                Destroy(gameObject);

                // canMove = false;
                // transform.position = startPosition;
                // SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Restart Level
                // return false;
            }

            if (hit.collider.tag == "Boundary") // cannot move if hit level boundary
            {
                // Instantiate(player, startPosition, Quaternion.identity);
                // Destroy(gameObject);

                return false;
            }

        }
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet") // if bullet hits player then make new player, destroy bullet and destroy player
        {
            Instantiate(player, startPosition, Quaternion.identity);  // ...instantiate new player at startposition and destroy this player
            Destroy(other.gameObject);
            Instantiate(playerDeath, transform.position, transform.rotation);

            // FMODUnity.RuntimeManager.PlayOneShot(Death, transform.position);

            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Wall")      // If player hits wall with tag "wall" then... This is so player dies if he falls down level and touches the BG object
        {
            Instantiate(player, startPosition, Quaternion.identity);  // ...instantiate new player at startposition and destroy this player
            Instantiate(playerDeath, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
