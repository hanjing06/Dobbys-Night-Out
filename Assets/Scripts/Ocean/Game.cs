using System.Collections;
using System.Collections.Generic;
using Ocean;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    public ArrayLayout boardLayout;
    public Sprite[] pieces;
    public GameObject tilePrefab;
    public BoatBuilder boatBuilder;

    [Header("Board")]
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSpacing = 1.2f;
    [SerializeField] private Vector2 boardOffset = Vector2.zero;
    [SerializeField] private bool centerBoard = true;
    [SerializeField] private float dragThreshold = 0.3f;

    [Header("Game Progress")]
    [SerializeField] private int maxMatchesAllowed = 12;
    [SerializeField] private int boatProgressNeeded = 12;

    [Header("UI")]
    public Slider progressBar;
    public TextMeshProUGUI matchesText;
    public GameObject winPanel;
    public GameObject gameOverPanel;

    private TilePiece dragStartTile;
    private Node[,] board;
    private TilePiece[,] tilePieces;
    private System.Random random;

    private int matchesUsed = 0;
    private int boatProgress = 0;
    private bool gameEnded = false;
    
    private bool isResolving = false;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());

        matchesUsed = 0;
        boatProgress = 0;
        gameEnded = false;

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        InitializeBoard();
        RemoveStartingMatches();
        UpdateBoardVisuals();
        UpdateUI();
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

    public void BeginDrag(TilePiece tile)
    {
        if (gameEnded || isResolving) return;
        dragStartTile = tile;
    }

    public void EndDrag(TilePiece tile, Vector3 dragDelta)
    {
        if (gameEnded || isResolving) return;

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

        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            target = dragDelta.x > 0
                ? Point.add(start, Point.right)
                : Point.add(start, Point.left);
        }
        else
        {
            target = dragDelta.y > 0
                ? Point.add(start, Point.up)
                : Point.add(start, Point.down);
        }

        if (IsInBoard(target))
        {
            if (TrySwap(start, target))
            {
                UpdateBoardVisuals();
                StartCoroutine(ResolveBoardRoutine());
            }
        }

        dragStartTile = null;
    }
    
    IEnumerator PlayBreakAnimations(List<Point> matches)
    {
        foreach (Point p in matches)
        {
            TilePiece tile = tilePieces[p.x, p.y];
            if (tile != null && tile.gameObject.activeSelf)
            {
                StartCoroutine(tile.BreakAnimation());
            }
        }

        yield return new WaitForSeconds(0.2f);
    }
    
    int GetProgressEarned(List<Point> matches)
    {
        int progress = 1;

        if (matches.Count >= 4)
            progress = 2;

        if (GetBonusForShape(matches) > 0)
            progress = 2;

        return progress;
    }

    bool TrySwap(Point a, Point b)
    {
        if (gameEnded) return false;

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

    IEnumerator ResolveBoardRoutine()
    {
        if (gameEnded) yield break;

        isResolving = true;

        List<Point> matches = FindAllMatches();
        bool countedThisPlayerMove = false;

        while (matches.Count > 0)
        {
            int progressEarned = GetProgressEarned(matches);

            if (!countedThisPlayerMove)
            {
                matchesUsed++;
                countedThisPlayerMove = true;
            }

            yield return StartCoroutine(PlayBreakAnimations(matches));

            boatProgress += progressEarned;

            if (boatBuilder != null)
            {
                boatBuilder.AddProgress(progressEarned);
            }

            UpdateUI();
            DebugMatchShapes(matches);

            ClearMatches(matches);
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.08f);

            CollapseBoard();
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.08f);

            RefillBoard();
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.1f);

            if (boatProgress >= boatProgressNeeded)
            {
                WinGame();
                isResolving = false;
                yield break;
            }

            matches = FindAllMatches();
        }

        if (matchesUsed >= maxMatchesAllowed && boatProgress < boatProgressNeeded)
        {
            LoseGame();
        }

        isResolving = false;
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
                    tile.transform.position = GetWorldPosition(x, y);
                    tile.transform.localScale = Vector3.one;
                    tile.transform.rotation = Quaternion.identity;
                    tile.SetSprite(pieces[value - 1]);
                }
                else
                {
                    tile.gameObject.SetActive(false);
                }
            }
        }
    }

    void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = boatProgressNeeded;
            progressBar.value = boatProgress;
        }

        if (matchesText != null)
        {
            matchesText.text = "Matches: " + matchesUsed + " / " + maxMatchesAllowed;
        }
    }

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("You built the boat!");

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    void LoseGame()
    {
        gameEnded = true;
        Debug.Log("Game Over");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
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

    int GetBonusForShape(List<Point> matches)
    {
        foreach (Point p in matches)
        {
            MatchShape shape = GetMatchShape(p);
            if (shape == MatchShape.LShape || shape == MatchShape.TShape)
                return 1;
        }

        return 0;
    }

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