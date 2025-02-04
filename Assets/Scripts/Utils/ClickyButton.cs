using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ClickyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _default, _pressed;
    [SerializeField] private AudioClip _compressClip, _uncompressClip;
    [SerializeField] private AudioSource _source;

    public void OnPointerDown(PointerEventData eventData)
    {
        _img.sprite = _pressed;

        if (_compressClip != null)
        {
            _source.clip = _compressClip;
            _source.Play(); // Play the compression sound
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(WaitForSoundAndReset());
    }

    private IEnumerator WaitForSoundAndReset()
    {
        if (_uncompressClip != null)
        {
            _source.clip = _uncompressClip;
            _source.Play(); // Play the uncompress sound
            yield return new WaitForSeconds(_source.clip.length); // Wait for it to finish
        }

        _img.sprite = _default; // Change back to default sprite
    }
}
