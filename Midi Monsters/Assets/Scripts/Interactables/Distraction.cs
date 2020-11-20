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

    bool active = false;
    float timer = 0;

    //[SerializeField, Tooltip("Reference to Emitter script")]
    private Emitter emitter;

    public void Start()
    {
        monster = FindObjectOfType<Monster>();
        emitter = GetComponent<Emitter>();
    }

    public override bool IsInteractable()
    {
        return true;
    }

    public override void Interact(PlayerCharacterController pc)
    {
        monster = FindObjectOfType<Monster>();
        timer = delay;
        active = true;
    }

    private void Update()
    {
        if (active)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TriggerDistraction();
            }
        }
    }

    private void TriggerDistraction()
    {
        emitter.PlaySound();

        if (monster != null)
        {
            // check distance from self to monster
            float dist = Vector3.Distance(transform.position, Monster.transform.position);
            // notify monster if close enough to hear
            if (dist < audible_distance)
            {
                monster.DetectDistractionSound(new Monster.DetectedSound(transform.position, transform.position, 1)); // predicted == current for now
            }
        }

        active = false;
    }

    public override string GetInteractionPrompt()
    {
        if (active)
        {
            return timer.ToString("0.0") + "s";
        }
        else
        {
            return interactionPrompt;
        }
    }
}
