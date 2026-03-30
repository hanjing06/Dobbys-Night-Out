using System.Collections;
using UnityEngine;

public class TilePiece : MonoBehaviour
{
    public Point boardPosition;
    public SpriteRenderer spriteRenderer;
    public Game game;

    private Vector3 mouseStartWorld;
    private bool isDragging = false;

    private void OnMouseDown()
    {
        if (game == null) return;

        mouseStartWorld = GetMouseWorldPosition();
        isDragging = true;

        game.BeginDrag(this);
    }

    private void OnMouseUp()
    {
        if (game == null || !isDragging) return;

        Vector3 mouseEndWorld = GetMouseWorldPosition();
        Vector3 drag = mouseEndWorld - mouseStartWorld;

        isDragging = false;

        game.EndDrag(this, drag);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = 10f;
        Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
        world.z = 0f;
        return world;
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }

    public IEnumerator BreakAnimation()
    {
        if (spriteRenderer == null) yield break;

        float duration = 0.18f;
        float time = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.position;
        Color startColor = spriteRenderer.color;

        Vector3 endPos = startPos + new Vector3(0f, 0.15f, 0f);
        float randomRotate = Random.Range(-20f, 20f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, randomRotate, t));

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = c;

            yield return null;
        }

        transform.localScale = startScale;
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        spriteRenderer.color = Color.white;
    }
}