using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEventListener : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent PointerUp;
    public UnityEvent PointerDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp.Invoke();
    }
}
