using System.Collections.Generic;
using UnityEngine;

public class Key : Interactable
{
    [SerializeField]
    List<Transform> possibleSpawnPoints = null;

    public enum KeyType
    {
        Normal,
        Red,
    }

    [SerializeField]
    private KeyType keyType = KeyType.Normal;

    public override string GetInteractionPrompt() { return interactionPrompt; }

    private void Start()
    {
        if (possibleSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleSpawnPoints.Count);
            transform.position = possibleSpawnPoints[randomIndex].position;
            transform.rotation = possibleSpawnPoints[randomIndex].rotation;
        }
    }

    public override void Interact(PlayerCharacterController pc)
    {
        pc.AddKey(keyType);
        Destroy(this.gameObject);
    }

    public override bool IsInteractable()
    {
        return true;
    }
}