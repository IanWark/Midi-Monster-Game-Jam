using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class Readable : Interactable
{

    [SerializeField, Tooltip("Image to display.")]
    private Texture m_DisplayTexture;
    public Texture DisplayTexture{ get { return m_DisplayTexture; } }


    [SerializeField, Tooltip("Coordinates")]
    public Vector2 m_ScreenPos = Vector2.zero;

    private GameObject m_PlayerObj;
    private GameObject m_ReadableObj; // Image on Player's canvas for drawing
    // private Canvas canvas;
    private Image m_Image;
    private Vector3 m_InitialLookDirection = Vector3.zero;

    private Camera m_Camera;

    private PlayerCharacterController m_PlayerController;
    private bool m_isReading { set; get; }
    public override string GetInteractionPrompt() { return interactionPrompt; }

    public void Start()
    {
        m_PlayerObj = GameObject.Find("Player");
        m_Camera = GameObject.Find("Player/Main Camera").GetComponent<Camera>();
        m_ReadableObj = GameObject.Find("Player/Canvas/Readable");

        if (!m_ReadableObj)
        {
            return;
        }
        m_Image = m_ReadableObj.GetComponent<Image>();
        this.enabled = false; // no need for Update until reading
        
    }

    public void Update()
    {
        if (m_isReading)
        {
            if (Vector3.Angle(m_Camera.ScreenPointToRay(Input.mousePosition).direction, m_InitialLookDirection) > 10)
            {
                m_isReading = false;
                m_Image.enabled = false;
                this.enabled = false; // stop update checks
                Debug.Log("Stop read");
                
            }
        }
    }
    public override bool IsInteractable() { return true;  }

    public override void Interact(PlayerCharacterController pc)
    {
        m_PlayerController = pc;
        m_isReading = true;
        m_InitialLookDirection = m_Camera.ScreenPointToRay(Input.mousePosition).direction;
        Rect dimensions = new Rect(m_ScreenPos.x, m_ScreenPos.y, m_DisplayTexture.width, m_DisplayTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        m_Image.sprite = Sprite.Create((Texture2D)m_DisplayTexture, dimensions, pivot);
        m_Image.enabled = true;
        this.enabled = true;
       Debug.Log("Read");
    }

}
