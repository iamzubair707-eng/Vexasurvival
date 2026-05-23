using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static MobileInput Instance;
    
    public bool IsTouching { get; private set; }
    public Vector2 TouchPosition { get; private set; }
    public Vector2 TouchDelta { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            IsTouching = true;
            TouchPosition = touch.position;
            TouchDelta = touch.deltaPosition;
        }
        else
        {
            IsTouching = false;
            TouchDelta = Vector2.zero;
        }
    }
    
    public void OnPointerDown(PointerEventData data)
    {
        IsTouching = true;
        TouchPosition = data.position;
    }
    
    public void OnPointerUp(PointerEventData data)
    {
        IsTouching = false;
    }
    
    public void OnDrag(PointerEventData data)
    {
        TouchDelta = data.delta;
    }
}