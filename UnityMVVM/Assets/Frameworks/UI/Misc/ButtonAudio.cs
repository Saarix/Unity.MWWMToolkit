using MVVMToolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip audioClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.PlaySfx(audioClip);
    }
}
