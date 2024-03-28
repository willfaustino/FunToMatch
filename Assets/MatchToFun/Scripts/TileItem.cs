using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileItem : MonoBehaviour
{
    public Color tileColor;
    
    public bool isMatched;
    public int xIndex;
    public int yIndex;
    public bool isMoving;

    [SerializeField]private SpriteRenderer _spriteRenderer;

    public TileItem(int x, int y) 
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetIndex(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetColor(int colorIndex)
    {
        _spriteRenderer.color = GetColorFromIndex(colorIndex);
        tileColor = GetColorFromIndex(colorIndex);
    }

    Color GetColorFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    public void MoveToTarget(Vector2 targetPos) 
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration) 
        {
            float t = elapsedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }
}
