using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Emitter))]
public class Distraction : Interactable
{

    [SerializeField, Tooltip("Amount of time before sound triggers.")]
    private float delay = 5;
    public float Delay { get { return delay; } }

    [SerializeField, Tooltip("Distance from which distraction is audible to Monster.")]
    private float audible_distance;
    public float Audible_Distance { get { return audible_distance; } }

    
    private GameObject monsterObj;
    private Monster monster;
    private Monster Monster { get { return monster; } }

    [SerializeField, Tooltip("Reference to Emitter script")]
    private Emitter emitter;

    public void Start()
    {
        monster = GameObject.FindObjectOfType<Monster>();
        
    }

    public override bool IsInteractable()
    {
        return true;
    }

    public override void Interact(PlayerCharacterController pc)
    {
        monster = GameObject.FindObjectOfType<Monster>();
        if (monster == null)
        {
            return;
        }
        StartCoroutine(TriggerDistraction());
        
    }

    private IEnumerator TriggerDistraction()
    {
   
        // wait for delay
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        // check distance from self to monster
        float dist = Vector3.Distance(transform.position, Monster.transform.position);
        // notify monster if close enough to hear

        emitter.PlaySound();

        if (dist < audible_distance)
        {
            monster.DetectSound(new Monster.DetectedSound(transform.position, transform.position, 1)); // predicted == current for now
        }

        Debug.Log("Box has been interacted with.");
    }
}
