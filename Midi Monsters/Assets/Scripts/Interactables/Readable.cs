using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class Readable : Interactable
{

    [SerializeField, Tooltip("Image to display.")]
    private Texture displayTexture;
    public Texture DisplayTexture{ get { return displayTexture; } }


    [SerializeField, Tooltip("Coordinates")]
    public Vector2 screen_pos = Vector2.zero;

    private GameObject playerObj;
    private GameObject readableObj;
// private Canvas canvas;
    private Image image;
    private Vector3 initialLookDirection = Vector3.zero;

    [SerializeField, Tooltip("Main Camera")]
    private Camera camera;

    private bool isReading;

    public void Start()
    {
        playerObj = GameObject.Find("Player");
        readableObj = GameObject.Find("Player/Canvas/Readable");
        if (!readableObj)
        {
            return;
        }
        image = readableObj.GetComponent<Image>();
        
    }

    public void Update()
    {
        if (isReading)
        {
            if (Vector3.Angle(camera.ScreenPointToRay(Input.mousePosition).direction, initialLookDirection) > 10)
            {
                isReading = false;
                image.enabled = false;
                Debug.Log("Stop read");
            }
        }
    }
    public override bool IsInteractable() { return true;  }

    public override void Interact()
    {
        isReading = true;
        initialLookDirection = camera.ScreenPointToRay(Input.mousePosition).direction;
        Rect dimensions = new Rect(screen_pos.x, screen_pos.y, displayTexture.width, displayTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        image.sprite = Sprite.Create((Texture2D)displayTexture, dimensions, pivot);
        image.enabled = true;
       Debug.Log("Read");
    }

}
