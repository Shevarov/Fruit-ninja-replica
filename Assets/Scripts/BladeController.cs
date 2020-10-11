using UnityEngine;
using UnityEngine.EventSystems;

public class BladeController : MonoBehaviour,IDragHandler,IPointerUpHandler,IPointerDownHandler
{
    private Vector2 origin;
    private Vector2 direction;
    private Vector2 currentPosition;
    private Blade blade;

    private void Awake()
    {
        blade = FindObjectOfType<Blade>();
        direction = Vector2.zero;
        currentPosition = Vector2.zero;
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        currentPosition = eventData.position;

        blade.UpdatePosition(currentPosition);

        direction = (currentPosition - origin).normalized;
        
        blade.SetDirection(direction);

        origin = currentPosition;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        origin = eventData.position;
        blade.UpdatePosition(origin);
        blade.StartCutting();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        direction = Vector2.zero;
        origin = Vector2.zero;
        blade.StopCutting();
    }
}
