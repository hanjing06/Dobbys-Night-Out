using System.Collections.Generic;
using Ocean;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ArrayLayout boardLayout;
    public Sprite[] pieces;
    public GameObject tilePrefab;

    [SerializeField] private int width = 7;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSpacing = 1.2f;
    [SerializeField] private Vector2 boardOffset = Vector2.zero;
    [SerializeField] private bool centerBoard = true;
    [SerializeField] private float dragThreshold = 0.3f;

    private TilePiece dragStartTile;
    private Node[,] board;
    private TilePiece[,] tilePieces;
    private System.Random random;

    private TilePiece selectedTile;

    void Start()
    {
        StartGame();
    }
    
    Vector3 GetWorldPosition(int x, int y)
    {
        float xPos = x;
        float yPos = y;

        if (centerBoard)
        {
            float offsetX = (width - 1) / 2f;
            float offsetY = (height - 1) / 2f;

            xPos = (x - offsetX) * tileSpacing;
            yPos = (y - offsetY) * tileSpacing;
        }
        else
        {
            xPos = x * tileSpacing;
            yPos = y * tileSpacing;
        }

        return new Vector3(
            xPos + boardOffset.x,
            yPos + boardOffset.y,
            0f
        );
    }
    
    public void BeginDrag(TilePiece tile)
    {
        dragStartTile = tile;
    }

    public void EndDrag(TilePiece tile, Vector3 dragDelta)
    {
        if (dragStartTile == null || tile != dragStartTile)
        {
            dragStartTile = null;
            return;
        }

        if (dragDelta.magnitude < dragThreshold)
        {
            dragStartTile = null;
            return;
        }

        Point start = tile.boardPosition;
        Point target = start;

        // Choose strongest drag direction
        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            if (dragDelta.x > 0)
                target = Point.add(start, Point.right);
            else
                target = Point.add(start, Point.left);
        }
        else
        {
            if (dragDelta.y > 0)
                target = Point.add(start, Point.up);
            else
                target = Point.add(start, Point.down);
        }

        if (IsInBoard(target))
        {
            if (TrySwap(start, target))
            {
                UpdateBoardVisuals();
                ResolveBoard();
                UpdateBoardVisuals();
            }
        }

        dragStartTile = null;
    }

    void StartGame()
    {
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());

        InitializeBoard();
        RemoveStartingMatches();
        UpdateBoardVisuals();
    }

    void InitializeBoard()
    {
        board = new Node[width, height];
        tilePieces = new TilePiece[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = FillPiece();
                board[x, y] = new Node(value, new Point(x, y));

                GameObject obj = Instantiate(tilePrefab, GetWorldPosition(x, y), Quaternion.identity);
                TilePiece tile = obj.GetComponent<TilePiece>();

                tile.boardPosition = new Point(x, y);
                tile.game = this;

                tilePieces[x, y] = tile;
            }
        }
    }

    public void SelectTile(TilePiece tile)
    {
        if (tile == null)
            return;

        Point p = tile.boardPosition;

        if (!IsInBoard(p))
            return;

        if (board[p.x, p.y].value <= 0)
            return;

        if (selectedTile == null)
        {
            selectedTile = tile;
            Debug.Log("Selected first tile: " + p.x + ", " + p.y);
            return;
        }

        if (selectedTile == tile)
        {
            selectedTile = null;
            return;
        }

        Point a = selectedTile.boardPosition;
        Point b = tile.boardPosition;

        if (TrySwap(a, b))
        {
            UpdateBoardVisuals();
            ResolveBoard();
            UpdateBoardVisuals();
        }

        selectedTile = null;
    }

    bool TrySwap(Point a, Point b)
    {
        if (!IsInBoard(a) || !IsInBoard(b))
            return false;

        if (!IsAdjacent(a, b))
            return false;

        if (board[a.x, a.y].value <= 0 || board[b.x, b.y].value <= 0)
            return false;

        SwapValues(a, b);

        List<Point> allMatches = FindAllMatches();

        if (allMatches.Count == 0)
        {
            SwapValues(a, b);
            UpdateBoardVisuals();
            Debug.Log("Invalid swap");
            return false;
        }

        Debug.Log("Valid swap");
        return true;
    }

    void SwapValues(Point a, Point b)
    {
        int temp = board[a.x, a.y].value;
        board[a.x, a.y].value = board[b.x, b.y].value;
        board[b.x, b.y].value = temp;
    }

    bool IsAdjacent(Point a, Point b)
    {
        return (Mathf.Abs(a.x - b.x) == 1 && a.y == b.y) ||
               (Mathf.Abs(a.y - b.y) == 1 && a.x == b.x);
    }

    bool IsInBoard(Point p)
    {
        return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
    }

    int GetValueAtPoint(Point p)
    {
        if (!IsInBoard(p))
            return -999;

        return board[p.x, p.y].value;
    }

    int FillPiece()
    {
        return random.Next(1, pieces.Length + 1);
    }

    void AddPoints(ref List<Point> points, List<Point> newPoints)
    {
        foreach (Point p in newPoints)
        {
            if (!points.Contains(p))
            {
                points.Add(p);
            }
        }
    }

    List<Point> GetHorizontalMatch(Point p)
    {
        List<Point> result = new List<Point>();

        if (!IsInBoard(p))
            return result;

        int val = GetValueAtPoint(p);

        if (val <= 0)
            return result;

        result.Add(p);

        Point left = Point.add(p, Point.left);
        while (IsInBoard(left) && GetValueAtPoint(left) == val)
        {
            result.Add(left);
            left = Point.add(left, Point.left);
        }

        Point right = Point.add(p, Point.right);
        while (IsInBoard(right) && GetValueAtPoint(right) == val)
        {
            result.Add(right);
            right = Point.add(right, Point.right);
        }

        if (result.Count < 3)
            result.Clear();

        return result;
    }

    List<Point> GetVerticalMatch(Point p)
    {
        List<Point> result = new List<Point>();

        if (!IsInBoard(p))
            return result;

        int val = GetValueAtPoint(p);

        if (val <= 0)
            return result;

        result.Add(p);

        Point up = Point.add(p, Point.up);
        while (IsInBoard(up) && GetValueAtPoint(up) == val)
        {
            result.Add(up);
            up = Point.add(up, Point.up);
        }

        Point down = Point.add(p, Point.down);
        while (IsInBoard(down) && GetValueAtPoint(down) == val)
        {
            result.Add(down);
            down = Point.add(down, Point.down);
        }

        if (result.Count < 3)
            result.Clear();

        return result;
    }

    List<Point> GetMatch(Point p)
    {
        List<Point> combined = new List<Point>();

        List<Point> horizontal = GetHorizontalMatch(p);
        List<Point> vertical = GetVerticalMatch(p);

        AddPoints(ref combined, horizontal);
        AddPoints(ref combined, vertical);

        return combined;
    }

    List<Point> FindAllMatches()
    {
        List<Point> matches = new List<Point>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y].value <= 0)
                    continue;

                Point p = new Point(x, y);
                List<Point> match = GetMatch(p);

                if (match.Count >= 3)
                {
                    AddPoints(ref matches, match);
                }
            }
        }

        return matches;
    }

    void ClearMatches(List<Point> matches)
    {
        foreach (Point p in matches)
        {
            if (board[p.x, p.y].value > 0)
            {
                board[p.x, p.y].value = 0;
            }
        }
    }

    void CollapseBoard()
    {
        for (int x = 0; x < width; x++)
        {
            CollapseColumn(x);
        }
    }

    void CollapseColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (board[x, y].value == 0)
            {
                for (int above = y + 1; above < height; above++)
                {
                    if (board[x, above].value > 0)
                    {
                        board[x, y].value = board[x, above].value;
                        board[x, above].value = 0;
                        break;
                    }
                }
            }
        }
    }

    void RefillBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y].value == 0)
                {
                    board[x, y].value = FillPiece();
                }
            }
        }
    }

    void ResolveBoard()
    {
        List<Point> matches = FindAllMatches();

        while (matches.Count > 0)
        {
            DebugMatchShapes(matches);

            ClearMatches(matches);
            CollapseBoard();
            RefillBoard();
            UpdateBoardVisuals();

            matches = FindAllMatches();
        }
    }

    void RemoveStartingMatches()
    {
        List<Point> matches = FindAllMatches();

        while (matches.Count > 0)
        {
            foreach (Point p in matches)
            {
                if (board[p.x, p.y].value > 0)
                {
                    board[p.x, p.y].value = FillPiece();
                }
            }

            matches = FindAllMatches();
        }
    }

    void UpdateBoardVisuals()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TilePiece tile = tilePieces[x, y];
                int value = board[x, y].value;

                if (value > 0)
                {
                    tile.gameObject.SetActive(true);
                    tile.SetSprite(pieces[value - 1]);
                }
                else
                {
                    tile.gameObject.SetActive(false);
                }
            }
        }
    }

    string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";

        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }

        return seed;
    }

    // -------------------------
    // L SHAPE / T SHAPE HELPERS
    // -------------------------

    void DebugMatchShapes(List<Point> matches)
    {
        foreach (Point p in matches)
        {
            MatchShape shape = GetMatchShape(p);

            if (shape == MatchShape.LShape)
            {
                Debug.Log("L shape found at: " + p.x + ", " + p.y);
            }
            else if (shape == MatchShape.TShape)
            {
                Debug.Log("T shape found at: " + p.x + ", " + p.y);
            }
        }
    }

    MatchShape GetMatchShape(Point p)
    {
        List<Point> horizontal = GetHorizontalMatch(p);
        List<Point> vertical = GetVerticalMatch(p);

        bool hasH = horizontal.Count >= 3;
        bool hasV = vertical.Count >= 3;

        if (!hasH && !hasV)
            return MatchShape.None;

        if (hasH && hasV)
        {
            int val = GetValueAtPoint(p);

            bool left = IsInBoard(Point.add(p, Point.left)) &&
                        GetValueAtPoint(Point.add(p, Point.left)) == val;

            bool right = IsInBoard(Point.add(p, Point.right)) &&
                         GetValueAtPoint(Point.add(p, Point.right)) == val;

            bool up = IsInBoard(Point.add(p, Point.up)) &&
                      GetValueAtPoint(Point.add(p, Point.up)) == val;

            bool down = IsInBoard(Point.add(p, Point.down)) &&
                        GetValueAtPoint(Point.add(p, Point.down)) == val;

            int horizontalSides = 0;
            int verticalSides = 0;

            if (left) horizontalSides++;
            if (right) horizontalSides++;
            if (up) verticalSides++;
            if (down) verticalSides++;

            if ((horizontalSides == 2 && verticalSides >= 1) ||
                (verticalSides == 2 && horizontalSides >= 1))
            {
                return MatchShape.TShape;
            }

            return MatchShape.LShape;
        }

        return MatchShape.Line;
    }
}

public enum MatchShape
{
    None,
    Line,
    LShape,
    TShape
}