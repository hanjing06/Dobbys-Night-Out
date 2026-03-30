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
        mouse.z = 10f; // distance from camera to board plane
        Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
        world.z = 0f;
        return world;
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }
}