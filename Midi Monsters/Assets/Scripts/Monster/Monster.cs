using UnityEngine;

public class Monster : MonoBehaviour
{
    public class DetectedSound
    {
        public DetectedSound(Vector3 position, Vector3 predictedPosition)
        {
            this.position = position;
            this.predictedPosition = predictedPosition;
        }

        public Vector3 position;
        public Vector3 predictedPosition;
    }

    private DetectedSound currentSound = null;

    MonsterMovement monsterMovement;

    // Start is called before the first frame update
    void Start()
    {
        monsterMovement = GetComponent<MonsterMovement>();
    }

    public void DetectSound(DetectedSound detectedSound)
    {
        // Detectable things send an event to us

        // I think we do the distance check in here.
        currentSound = detectedSound;
        monsterMovement.MoveToPosition(currentSound.predictedPosition, monsterMovement.RunningSpeed);

        /*
        if (currentSound == null)
        {
            
        }
        */

        // else, we don't care about this sound.
    }
}
