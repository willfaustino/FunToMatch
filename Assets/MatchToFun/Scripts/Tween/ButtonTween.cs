using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonTween : MonoBehaviour, IPointerDownHandler
{
    private void Start()
    {
        gameObject.transform.DOScale(0.9f, 0.7f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //gameObject.transform.DOScale(0.9f, 0.15f).OnComplete(() => transform.DOScale(1f, 0.15f));
    }
}
