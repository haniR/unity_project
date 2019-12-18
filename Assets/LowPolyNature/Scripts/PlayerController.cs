using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Private Members

    private Animator _animator;

    private CharacterController _characterController;

    private float Gravity = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private InventoryItemBase mCurrentItem = null;

    private HealthBar mHealthBar;

    private HealthBar mFoodBar;

    private int startHealth;

    private int startFood;

    #endregion

    #region Public Members

    public float Speed ;

    public float RotationSpeed ;

    public Inventory Inventory;

    public GameObject Hand;

    public HUD Hud;

    public float JumpSpeed;

    #endregion
    public AudioClip healthClip; 
    public AudioClip unhealthClip;
    public AudioClip gameOverClip;
    public AudioSource healthSource; 
    public AudioSource unhealthSource;
    public AudioSource gameOverSource;
    public GameObject losePanel;
    public GameObject winPanel;


    bool speedflage; 
    bool jumpflage; 
    float speedTime ;
    float jumbTime;
    float slowTime ; 
    float notJumbTime; 
    public Text timeText;
    float time;
    float time2;
    // Use this for initialization
    void Start()
    {
        losePanel.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(false);

        healthSource.clip = healthClip;
        unhealthSource.clip = unhealthClip;
        gameOverSource.clip = gameOverClip;
        Speed = 5.0f;
        RotationSpeed = 90f;
        JumpSpeed = 9.0f;
        speedflage = false;
             jumpflage = false; 

        time = 15;
        speedTime = 0;
        jumbTime = 0;
        slowTime = 0;
        time2 = 0;
        notJumbTime = 0;
        setText();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        Inventory.ItemUsed += Inventory_ItemUsed;
        Inventory.ItemRemoved += Inventory_ItemRemoved;

        mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
        mHealthBar.Min = 0;
        mHealthBar.Max = Health;
        startHealth = Health;
        mHealthBar.SetValue(Health);

        mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
        mFoodBar.Min = 0;
        mFoodBar.Max = Food;
        startFood = Food;
        mFoodBar.SetValue(Food);

        InvokeRepeating("IncreaseHunger", 0, HungerRate);
    }

    #region Inventory

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;

    }

    private void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        if (e.Item.ItemType != EItemType.Consumable)
        {
            // If the player carries an item, un-use it (remove from player's hand)
            if (mCurrentItem != null)
            {
                SetItemActive(mCurrentItem, false);
            }

            InventoryItemBase item = e.Item;

            // Use item (put it to hand of the player)
            SetItemActive(item, true);

            mCurrentItem = e.Item;
        }

    }

    private int Attack_1_Hash = Animator.StringToHash("Base Layer.Attack_1");

    public bool IsAttacking
    {
        get
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Attack_1_Hash)
            {
                return true;
            }
            return false;
        }
    }

    public void DropCurrentItem()
    {
        _animator.SetTrigger("tr_drop");

        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        Inventory.RemoveItem(mCurrentItem);

        // Throw animation
        Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
        if (rbItem != null)
        {
            rbItem.AddForce(transform.forward * 2.0f, ForceMode.Impulse);

            Invoke("DoDropItem", 0.25f);
        }

    }

    public void DoDropItem()
    {

        // Remove Rigidbody
        Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());

        mCurrentItem = null;
    }

    #endregion

    #region Health & Hunger

    [Tooltip("Amount of health")]
    public int Health = 100;

    [Tooltip("Amount of food")]
    public int Food = 100;

    [Tooltip("Rate in seconds in which the hunger increases")]
    public float HungerRate = 0.5f;

    public void IncreaseHunger()
    {
        Food--;
        if (Food < 0)
            Food = 0;

        mFoodBar.SetValue(Food);

        if (IsDead)
        {
            CancelInvoke();
            _animator.SetTrigger("death");
        }
    }

    public bool IsDead
    {
        get
        {
            return Health == 0 || Food == 0;
        }
    }

    public bool IsArmed
    {
        get
        {
            if (mCurrentItem == null)
                return false;

            return mCurrentItem.ItemType == EItemType.Weapon;
        }
    }


    public void Eat(int amount)
    {
        Food += amount;
        if (Food > startFood)
        {
            Food = startFood;
        }

        mFoodBar.SetValue(Food);

    }

    public void Rehab(int amount)
    {
        Health += amount;
        if (Health > startHealth)
        {
            Health = startHealth;
        }

        mHealthBar.SetValue(Health);
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0)
            Health = 0;

        mHealthBar.SetValue(Health);

        if (IsDead)
        {
            _animator.SetTrigger("death");
        }

    }

    #endregion


    void FixedUpdate()
    {
        if (!IsDead)
        {
            // Drop item
            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }
        }
    }

    private bool mIsControlEnabled = true;

    public void EnableControl()
    {
        mIsControlEnabled = true;
    }

    public void DisableControl()
    {
        mIsControlEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        time2++;

        if (time > 0)
        {
            time -= 0.01f;
            setText();
        }
       
        if (speedTime > 0)
        {
            speedTime -= 0.01f;
            Debug.Log(speedTime);
        }
        else
        {
            Speed = 5f;
        }
        if (jumbTime > 0)
        {
           jumbTime -= 0.01f;
            Debug.Log("jumpTime "+JumpSpeed);
        }
        else
        {
            JumpSpeed = 9.0f;
        }
        if (notJumbTime > 0)
        {
            notJumbTime -= 0.01f;
            jumpflage = true; 
        }
       if(notJumbTime < 0 && jumpflage ==true)
        {
          JumpSpeed = 9.0f;
            jumpflage = false;
        }
        if (slowTime > 0)
        {
            slowTime -= 0.001f;
            speedflage = true; 
        }
        if (slowTime < 0 && speedflage == true)
        {
                Speed = 5f;
            speedflage = false; 
        }

        if (!IsDead && mIsControlEnabled)
        {
            // Interact with the item
            if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
            {
                // Interact animation
                mInteractItem.OnInteractAnimation(_animator);
            }

            // Execute action with item
            if (mCurrentItem != null && Input.GetMouseButtonDown(0))
            {
                // Dont execute click if mouse pointer is over uGUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // TODO: Logic which action to execute has to come from the particular item
                    _animator.SetTrigger("attack_1");
                }
            }

            // Get Input for axis
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Calculate the forward vector
            Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

            if (move.magnitude > 1f) move.Normalize();

            // Calculate the rotation for the player
            move = transform.InverseTransformDirection(move);

            // Get Euler angles
            float turnAmount = Mathf.Atan2(move.x, move.z);

            transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);

            if (_characterController.isGrounded)
            {
                _moveDirection = transform.forward * move.magnitude;

                _moveDirection *= Speed;

                if (Input.GetButton("Jump"))
                {
                    _animator.SetBool("is_in_air", true);
                    _moveDirection.y = JumpSpeed;

                }
                else
                {
                    _animator.SetBool("is_in_air", false);
                    _animator.SetBool("run", move.magnitude > 0);
                }
            }

            _moveDirection.y -= Gravity * Time.deltaTime;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }
    }

    public void InteractWithItem()
    {
        if (mInteractItem != null)
        {
            mInteractItem.OnInteract();

            if (mInteractItem is InventoryItemBase)
            {
                InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
                Inventory.AddItem(inventoryItem);
                inventoryItem.OnPickup();

                if (inventoryItem.UseItemAfterPickup)
                {
                    Inventory.UseItem(inventoryItem);
                }
            }
        }

        Hud.CloseMessagePanel();

        mInteractItem = null;
    }

    private InteractableItemBase mInteractItem = null;

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("healthy food"))
        {
            time = time + 5f;
            other.gameObject.SetActive(false);
            setText();
            healthSource.Play();
        }
        if (other.gameObject.CompareTag("unhealthy food"))
        {
            time = time - 5f;
            other.gameObject.SetActive(false);
            setText();
            unhealthSource.Play();
        }
        if (other.gameObject.CompareTag("banana"))
        {
            other.gameObject.SetActive(false);
            jumbTime =  3f;
            JumpSpeed = 14f; 
            setText();
            healthSource.Play();

        }
        if (other.gameObject.CompareTag("hotdog"))
        {
            other.gameObject.SetActive(false);
            notJumbTime =  3f;
            JumpSpeed = 2f; 
            setText();
            unhealthSource.Play();

        }

        if (other.gameObject.CompareTag("pepper"))
        {
            other.gameObject.SetActive(false);
            speedTime =  3f;
            Speed = 10f; 
            setText();
            healthSource.Play();

        }
        if (other.gameObject.CompareTag("pizza"))
        {
            other.gameObject.SetActive(false);
            slowTime =  4f;
            Speed = 2f; 
            setText();
            unhealthSource.Play();

        }
        if (other.gameObject.CompareTag("pineapple"))
        {
            time = time + 10f;
            other.gameObject.SetActive(false);
            setText();
            healthSource.Play();

        }
        if (other.gameObject.CompareTag("burger"))
        {
            time = time - 10f;
            other.gameObject.SetActive(false);
            setText();
            unhealthSource.Play();

        }
        if (other.gameObject.CompareTag("final") && time>0)
        {
            // win and go to next scene
            winPanel.gameObject.SetActive(true);

        }
        if ( time < 0 && time2>3)
        {
            // win and go to next scene
            losePanel.gameObject.SetActive(true);
            gameOverSource.Play();
            //  Restart();
            

        }
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void setText()
    {
        timeText.text = "Time : " + time;
    }
    private void OnTriggerEnter(Collision collision)
    {
      
    }
}
