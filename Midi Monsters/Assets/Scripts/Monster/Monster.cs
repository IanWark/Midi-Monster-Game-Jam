using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public class DetectedSound
    {
        public DetectedSound(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 position;
    }

    private DetectedSound currentSound = null;

    MonsterMovement monsterMovement;

    // Start is called before the first frame update
    void Start()
    {
        monsterMovement = GetComponent<MonsterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetectSound(DetectedSound detectedSound)
    {
        // Detectable things send an event to us

        // I think we do the distance check in here.

        if (currentSound == null || true)
        {
            currentSound = detectedSound;
            monsterMovement.MoveToPosition(currentSound.position, monsterMovement.RunningSpeed);
        }

        // else, we don't care about this sound.
    }
}
