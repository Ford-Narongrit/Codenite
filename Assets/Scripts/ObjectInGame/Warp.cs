using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : IInteractable
{
    public Transform endpoint;
    public override void interact()
    {
        interactor.transform.position = endpoint.position;
    }
}
