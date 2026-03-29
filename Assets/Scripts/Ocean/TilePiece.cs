using UnityEngine;

public class TilePiece : MonoBehaviour
{
    public Point boardPosition;
    public SpriteRenderer spriteRenderer;
    public Game game;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    void OnMouseDown()
    {
        if (game != null)
        {
            game.SelectTile(this);
        }
    }
}