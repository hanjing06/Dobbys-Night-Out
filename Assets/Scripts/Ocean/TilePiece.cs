using System.Collections;
using UnityEngine;

public class TilePiece : MonoBehaviour
{
    public Point boardPosition;
    public SpriteRenderer spriteRenderer;
    public Game game;

    private Vector3 mouseStartWorld;
    private Vector3 tileStartWorld;
    private bool isDragging = false;

    void OnMouseDown()
    {
        if (game == null) return;

        mouseStartWorld = GetMouseWorldPosition();
        tileStartWorld = transform.position;
        isDragging = true;

        game.BeginDrag(this);
    }

    void OnMouseDrag()
    {
        if (game == null || !isDragging) return;
        
        spriteRenderer.color = Color.gray;
        
        Vector3 currentMouseWorld = GetMouseWorldPosition();
        Vector3 dragDelta = currentMouseWorld - mouseStartWorld;

        game.UpdateDragPreview(this, dragDelta, tileStartWorld);
        
    }

    void OnMouseUp()
    {
        if (game == null || !isDragging) return;

        Vector3 mouseEndWorld = GetMouseWorldPosition();
        Vector3 dragDelta = mouseEndWorld - mouseStartWorld;

        isDragging = false;
        spriteRenderer.color = Color.white;
        game.EndDrag(this, dragDelta);
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
        {
            spriteRenderer.sprite = sprite;
        }
    }

    public IEnumerator BreakAnimation()
    {
        if (spriteRenderer == null) yield break;

        float duration = 0.18f;
        float time = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Color startColor = spriteRenderer.color;

        Vector3 endPos = startPos + new Vector3(0f, 0.15f, 0f);
        float randomRotate = Random.Range(-20f, 20f);
        Quaternion endRot = Quaternion.Euler(0f, 0f, randomRotate);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = c;

            yield return null;
        }

        transform.localScale = startScale;
        transform.position = startPos;
        transform.rotation = startRot;
        spriteRenderer.color = Color.white;
    }
}