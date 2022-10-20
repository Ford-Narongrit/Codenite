using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Photon.Pun;

public class MonsterController : MonoBehaviour, IDamageable
{
    [Header("Object Info")]
    protected NavMeshAgent agent;
    [SerializeField] protected Transform body;
    [SerializeField] private Transform aim;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem hitEf;

    [Header("Monster UI")]
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private GaugeBar healthBar;

    [Header("Monster info")]
    [SerializeField] private string monsterName = "monster";
    [SerializeField] public float chaseRadius = 3f;
    [SerializeField] public float moveRadius = 5f;
    [SerializeField] public float attackRadius = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private float attackSpeed = 4f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float respawnTime = 3f;
    public string carryItem { get; set; }
    private float currentHealth;
    private float currentSpeed;
    private float nextAttack;
    private bool reset = false;
    private Vector2 spawnPosition;
    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        transform.rotation = Quaternion.identity;
        //Monster info
        currentHealth = maxHealth;
        currentSpeed = moveSpeed;
        spawnPosition = transform.position;

        //NavMesh
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = currentSpeed;

        //set UI
        healthBar.SetMaxValue(maxHealth);
        monsterNameText.text = monsterName;
    }

    private void Update()
    {
        if (!isDead())
        {
            GameObject target = FindClosestPlayer();
            if (target && isInMoveRange() && !reset)
            {
                chasing(target.transform.position);
                if (isInAttackRange(target.transform.position))
                {
                    stop();
                    if (view.IsMine)
                        attack();
                }
            }
            else
            {
                if (isInSpawnPoint())
                {
                    patrol();
                    reset = false;
                }
                else
                {
                    goToSpawnPoint();
                    reset = true;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (isDead())
        {
            gameObject.SetActive(false);
            Destroy(gameObject, respawnTime);
        }
    }

    public virtual void moveTo(Vector2 target)
    {
        agent.SetDestination(target);
    }

    public virtual void rotation(Vector2 target)
    {
        Vector2 lookDir = target - (Vector2)body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        body.eulerAngles = Vector3.forward * angle;
    }
    public virtual void stop()
    {
        agent.SetDestination(transform.position);
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
    public void attack()
    {
        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackSpeed;
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, aim.position, aim.rotation);
            bullet.GetComponent<Bullet>().setup(this.gameObject, damage);
        }
    }

    //action stage
    public virtual void chasing(Vector2 target)
    {
        moveTo(target);
        rotation(target);
    }
    public virtual void goToSpawnPoint()
    {
        moveTo(spawnPosition);
        rotation(spawnPosition);
    }
    public virtual void patrol()
    {
        // Debug.Log("movearound");
    }

    //check
    public virtual bool isDead()
    {
        return currentHealth <= 0;
    }
    [PunRPC]
    public void setItem(string item)
    {
        carryItem = item;
    }
    public string dropItem()
    {
        return carryItem;
    }
    public bool isInMoveRange()
    {
        return Vector2.Distance(spawnPosition, transform.position) <= moveRadius;
    }
    public bool isInAttackRange(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) <= attackRadius;
    }
    public bool isInSpawnPoint()
    {
        return Vector2.Distance(spawnPosition, transform.position) <= 1;
    }
    public GameObject FindClosestPlayer()
    {
        float distanceToClosestTarget = Mathf.Infinity;
        GameObject closestTarget = null;
        GameObject[] allTarget = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject target in allTarget)
        {
            float distance = Vector2.Distance(target.transform.position, transform.position);
            if (distance < chaseRadius && distance < distanceToClosestTarget && !target.GetComponent<PlayerController>().isDead())
            {
                distanceToClosestTarget = distance;
                closestTarget = target;
            }
        }
        return closestTarget;
    }
}
