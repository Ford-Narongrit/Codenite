using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletDamage;
    [SerializeField] private float destroyTime = 0.4f;
    [SerializeField] private float moveSpeed = 20f;
    private Vector3 shootDir;
    private GameObject owner;
    private PhotonView view;

    private bool isPvpOn = false;

    public void setup(GameObject owner, float bulletDamage, float destroyTime = 0.4f, float moveSpeed = 20f)
    {
        this.owner = owner;
        this.bulletDamage = bulletDamage;
        this.destroyTime = destroyTime;
        this.moveSpeed = moveSpeed;
    }

    void Awake()
    {
        view = GetComponent<PhotonView>();
        isPvpOn = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.Pvp] == 1; //convert int to bool
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (view.IsMine)
        {
            if (!owner.gameObject.Equals(other.gameObject))
            {
                IDamageable damageableObject = other.gameObject.GetComponent<IDamageable>();
                if (damageableObject != null)
                {
                    other.gameObject.GetComponent<PhotonView>().RPC("takeDamage", RpcTarget.All, bulletDamage);
                    if (damageableObject.isDead() && owner.tag == "Player")
                    {
                        string myteam = (string)PhotonNetwork.LocalPlayer.CustomProperties[PlayerProperty.Team];
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        foreach (GameObject _player in players)
                        {
                            string ownerTeam = (string)_player.GetComponent<PhotonView>().Owner.CustomProperties[PlayerProperty.Team];
                            if (myteam == ownerTeam)
                                _player.GetComponent<PhotonView>().RPC("pickItem", RpcTarget.All, damageableObject.dropItem());
                        }

                        if (other.gameObject.tag != "Player")
                            owner.GetComponent<PlayerController>().delayWarp();
                    }
                }
            }
        }
        Destroy(gameObject);
    }
}
