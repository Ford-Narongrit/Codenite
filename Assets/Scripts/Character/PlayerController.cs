using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun, IDamageable
{
    [Header("Object info")]
    [SerializeField] private Transform playerHand;
    [SerializeField] private SpriteRenderer playerHair;
    [SerializeField] private Transform aim;
    [SerializeField] private ParticleSystem walkEf;
    [SerializeField] private ParticleSystem hitEf;
    [SerializeField] private GameObject bulletPrefab;
    private Animator animator;
    private Rigidbody2D rigi;

    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GaugeBar healthBar;

    [Header("Player info")]
    public float moveSpeed = 5.0f;
    public float damage = 10;
    public float maxHealth = 100;
    public float respawnTime = 10f;
    public float delayWarpTime = 1.5f;

    public bool interact { get; set; }
    public float currentChargeTime = 0f;
    public float chargeToFireTime = 5f;
    private Vector2 movement;
    private Vector2 mousePos;
    private Vector2 lookDir;
    private Vector2 spawnPoint;
    private float currentHealth;
    private float currentSpeed;
    private IInteractable interactableObject = null;
    public List<string> itemList = new List<string>();
    private PhotonView view;
    public void setInteractable(IInteractable interactable)
    {
        this.interactableObject = interactable;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        currentSpeed = moveSpeed;

        playerNameText.text = view.Owner.NickName;
        healthBar.SetMaxValue(maxHealth);
        spawnPoint = transform.position;
        interact = true;

        if (view.Owner.CustomProperties[PlayerProperty.Color] != null)
        {
            Color teamColor = ColorString.GetColorFromString((string)view.Owner.CustomProperties[PlayerProperty.Color]);
            setTeam(teamColor);
        }
    }

    public void setTeam(Color _colorTeam)
    {
        playerHair.color = _colorTeam;
    }

    private void Update()
    {
        if (interact)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetButton("Fire1") && view.IsMine && !ENVConfig.useSmile)
            {
                currentChargeTime += Time.deltaTime;
                if (currentChargeTime >= chargeToFireTime)
                    currentChargeTime = chargeToFireTime;
            }
            if (Input.GetButtonUp("Fire1") && view.IsMine && !ENVConfig.useSmile)
            {
                if (currentChargeTime >= chargeToFireTime)
                    shoot();
                currentChargeTime = 0f;
            }

            if (interactableObject != null)
            {
                if (Input.GetKeyDown(interactableObject.interactKey) && view.IsMine)
                {
                    interactableObject.interact();
                }
            }
        }
        else
        {
            movement = Vector2.zero;
        }
    }
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            move();
            rotation();
            updateAnimation();
        }
    }

    private void LateUpdate()
    {
        if (isDead())
        {
            gameObject.SetActive(false);
            if (view.IsMine)
                view.RPC("respawn", RpcTarget.All);
        }
    }

    public void move()
    {
        rigi.MovePosition(rigi.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    public void rotation()
    {
        lookDir = mousePos - (Vector2)playerHand.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 270f;
        playerHand.eulerAngles = Vector3.forward * angle;
    }

    public void shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, aim.position, aim.rotation);
        bullet.GetComponent<Bullet>().setup(this.gameObject, damage);
    }
    [PunRPC]
    public void takeDamage(float damage)
    {
        hitEf.Play();
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        healthBar.SetValue(currentHealth);
    }
    public void delayWarp()
    {
        Invoke("goSpawnPoint", delayWarpTime);
    }
    public void goSpawnPoint()
    {
        transform.position = spawnPoint;
    }

    [PunRPC]
    public void respawn()
    {
        currentHealth = maxHealth;
        currentSpeed = moveSpeed;
        transform.position = spawnPoint;
        gameObject.SetActive(true);
        healthBar.SetValue(currentHealth);
    }
    public bool isDead()
    {
        return currentHealth <= 0;
    }
    public string dropItem()
    {
        return null;
    }

    public void pickItemTeam(string itemname, string teamname)
    {
        string myteam = (string)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Team];
        if (myteam == teamname)
        {
            pickItem(itemname);
        }
    }

    [PunRPC]
    public void pickItem(string itemName)
    {
        CodePanelController code = FindObjectOfType<CodePanelController>();
        int maxInventory = code ? code.maxInventory : 3;
        if (itemList.Count < maxInventory)
        {
            if (!itemList.Contains(itemName) && !(itemName == "" || itemName == null))
            {
                itemList.Add(itemName);
            }
        }
    }

    [PunRPC]
    public void removeItem(string itemName)
    {
        if (itemList.Contains(itemName))
        {
            itemList.Remove(itemName);
        }
    }
    [PunRPC]
    public void destroy()
    {
        Destroy(gameObject);
    }
    public void updateAnimation()
    {
        animator.SetFloat("Horizontal", lookDir.normalized.x);
        animator.SetFloat("Vertical", lookDir.normalized.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude >= 1)
        {
            walkEf.Play();
        }
    }
}
