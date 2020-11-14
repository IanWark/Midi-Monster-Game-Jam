using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distraction : Interactable
{

    [SerializeField, Tooltip("Amount of time before sound triggers.")]
    private float delay = 5;
    public float Delay { get { return delay; } }

    [SerializeField, Tooltip("Distance from which distraction is audible to Monster.")]
    private float audible_distance;
    public float Audible_Distance { get { return audible_distance; } }

    [SerializeField, Tooltip("Hardcoded reference to the Monster.")]
    private Monster monster;
    public Monster Monster { get { return monster; } }
    

    public override bool IsInteractable()
    {
        return true;
    }

    public override void Interact()
    {
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
        if (dist < audible_distance)
        {
            monster.DetectSound(new Monster.DetectedSound(transform.position, transform.position)); // predicted == current for now
        }

        Debug.Log("Box has been interacted with.");
    }
}
