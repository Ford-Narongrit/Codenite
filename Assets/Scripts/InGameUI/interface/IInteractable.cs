using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class IInteractable : MonoBehaviour
{
    public KeyCode interactKey;
    protected GameObject interactor;
    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    public abstract void interact();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                interactor = other.gameObject;
                interactor.GetComponent<PlayerController>().setInteractable(this);
                HelperBox.Instance.showAlert("press " + interactKey + " to interact");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                interactor.GetComponent<PlayerController>().setInteractable(null);
                interactor = null;
                HelperBox.Instance.hide();
            }
        }
    }
}

