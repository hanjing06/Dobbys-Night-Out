using System.Collections;
using System.Collections.Generic;
using Ocean;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [Header("References")]
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

    [Header("Drag / Swap")]
    [SerializeField] private float dragThreshold = 0.3f;
    [SerializeField] private float previewDistance = 0.35f;
    [SerializeField] private float swapDuration = 0.12f;
    [SerializeField] private float invalidSwapReturnDuration = 0.10f;

    [Header("Game Progress")]
    [SerializeField] private int maxMatchesAllowed = 12;
    [SerializeField] private int boatProgressNeeded = 12;

    [Header("UI")] 
    public GameObject objectivePanel;
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI matchesLeft;
    public GameObject winPanel;
    public GameObject gameOverPanel;

    private Node[,] board;
    private TilePiece[,] tilePieces;
    private System.Random random;

    private TilePiece dragStartTile;
    private TilePiece previewTile;
    private Vector3 previewStartPosition;

    private int matchesUsed = 0;
    private int boatProgress = 0;

    private bool gameEnded = false;
    private bool isResolving = false;
    
    private bool showingObjective = true;
    private bool gameStarted = false;

    void Start()
    {
        showingObjective = true;
        gameStarted = false;

        // ONLY show objective panel
        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        // Disable everything else
        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (matchesText != null) matchesText.gameObject.SetActive(false);
        if(matchesLeft != null) matchesLeft.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (showingObjective && Input.anyKeyDown)
        {
            showingObjective = false;
            gameStarted = true;

            if (objectivePanel != null)
                objectivePanel.SetActive(false);

            // Enable UI
            if (matchesText != null) matchesText.gameObject.SetActive(true);
            if(matchesLeft != null) matchesLeft.gameObject.SetActive(true);

            // NOW start the game
            StartGame();
        }
    }

    void StartGame()
    {
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());

        matchesUsed = 0;
        boatProgress = 0;
        gameEnded = false;
        isResolving = false;

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        InitializeBoard();
        RemoveStartingMatches();
        UpdateBoardVisuals();
        UpdateUI();
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

                // board layout stuff

                board[x, y] = new Node(value, new Point(x, y));

                GameObject obj = Instantiate(tilePrefab, GetWorldPosition(x, y), Quaternion.identity);
                TilePiece tile = obj.GetComponent<TilePiece>();

                tile.boardPosition = new Point(x, y);
                tile.game = this;

                tilePieces[x, y] = tile;
            }
        }
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
        if (!gameStarted || showingObjective || gameEnded || isResolving) return;

        dragStartTile = tile;
        previewTile = tile;
        previewStartPosition = tile.transform.position;
    }

    public void UpdateDragPreview(TilePiece tile, Vector3 dragDelta, Vector3 startWorldPos)
    {
        if (gameEnded || isResolving) return;
        if (dragStartTile == null || tile != dragStartTile) return;

        previewTile = tile;
        previewStartPosition = startWorldPos;

        Vector3 offset = Vector3.zero;

        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            float clampedX = Mathf.Clamp(dragDelta.x, -previewDistance, previewDistance);
            offset = new Vector3(clampedX, 0f, 0f);
        }
        else
        {
            float clampedY = Mathf.Clamp(dragDelta.y, -previewDistance, previewDistance);
            offset = new Vector3(0f, clampedY, 0f);
        }

        tile.transform.position = startWorldPos + offset;
    }

    public void EndDrag(TilePiece tile, Vector3 dragDelta)
    {
        if (gameEnded || isResolving)
        {
            ResetPreviewTile();
            dragStartTile = null;
            return;
        }

        if (dragStartTile == null || tile != dragStartTile)
        {
            ResetPreviewTile();
            dragStartTile = null;
            return;
        }

        if (dragDelta.magnitude < dragThreshold)
        {
            ResetPreviewTile();
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

        ResetPreviewTile();

        if (IsInBoard(target))
        {
            StartCoroutine(PerformSwap(start, target));
        }

        dragStartTile = null;
    }

    void ResetPreviewTile()
    {
        if (previewTile != null)
        {
            previewTile.transform.position = GetWorldPosition(previewTile.boardPosition.x, previewTile.boardPosition.y);
            previewTile = null;
        }
    }

    bool IsSwapValid(Point a, Point b)
    {
        if (gameEnded || isResolving) return false;

        if (!IsInBoard(a) || !IsInBoard(b))
            return false;

        if (!IsAdjacent(a, b))
            return false;

        if (board[a.x, a.y].value <= 0 || board[b.x, b.y].value <= 0)
            return false;

        SwapValues(a, b);
        List<Point> allMatches = FindAllMatches();
        bool valid = allMatches.Count > 0;
        SwapValues(a, b);

        return valid;
    }

    IEnumerator AnimateTileSwap(
        TilePiece tileA,
        TilePiece tileB,
        Vector3 targetPosA,
        Vector3 targetPosB,
        float duration)
    {
        Vector3 startPosA = tileA.transform.position;
        Vector3 startPosB = tileB.transform.position;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            tileA.transform.position = Vector3.Lerp(startPosA, targetPosA, t);
            tileB.transform.position = Vector3.Lerp(startPosB, targetPosB, t);

            yield return null;
        }

        tileA.transform.position = targetPosA;
        tileB.transform.position = targetPosB;
    }

    IEnumerator PerformSwap(Point a, Point b)
    {
        if (gameEnded || isResolving) yield break;

        isResolving = true;

        TilePiece tileA = tilePieces[a.x, a.y];
        TilePiece tileB = tilePieces[b.x, b.y];

        Vector3 posA = GetWorldPosition(a.x, a.y);
        Vector3 posB = GetWorldPosition(b.x, b.y);

        bool valid = IsSwapValid(a, b);

        // IsSwapValid checks isResolving, so temporarily clear it before validation is needed.
        // Since we're already inside this coroutine, we can safely do this:
        isResolving = false;
        valid = IsSwapValid(a, b);
        isResolving = true;

        if (valid)
        {
            yield return StartCoroutine(AnimateTileSwap(tileA, tileB, posB, posA, swapDuration));

            SwapValues(a, b);
            UpdateBoardVisuals();

            isResolving = false;
            yield return StartCoroutine(ResolveBoardRoutine());
        }
        else
        {
            yield return StartCoroutine(AnimateTileSwap(tileA, tileB, posB, posA, swapDuration));
            yield return StartCoroutine(AnimateTileSwap(tileA, tileB, posA, posB, invalidSwapReturnDuration));

            UpdateBoardVisuals();
            isResolving = false;
        }
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
            yield return new WaitForSeconds(0.05f);
            boatProgress += progressEarned;
            Camera.main.transform.position += new Vector3(0, 0.05f, 0);

            if (boatBuilder != null)
            {
                boatBuilder.AddProgress(progressEarned);
            }

            UpdateUI();

            ClearMatches(matches);
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.08f);

            CollapseBoard();
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.08f);

            RefillBoard();
            UpdateBoardVisuals();

            yield return new WaitForSeconds(0.10f);

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

        return progress;
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

                tile.transform.position = GetWorldPosition(x, y);
                tile.transform.localScale = Vector3.one;
                tile.transform.rotation = Quaternion.identity;

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

    void UpdateUI()
    {

        if (matchesText != null)
        {
            int matchesLeft = maxMatchesAllowed - matchesUsed;
            matchesText.text=matchesLeft.ToString();
        }
    }
    
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject setSailText;

    void WinGame()
    {
        gameEnded = true;
        Debug.Log("You built the boat!");
        StartCoroutine(WinSequence());
    }
    
    IEnumerator WinSequence()
    {
        Debug.Log("Win sequence started");

        winPanel.SetActive(true);
        winText.SetActive(true);
        setSailText.SetActive(false);

        yield return new WaitForSeconds(1f);

        winText.SetActive(false);
        setSailText.SetActive(true);

        yield return new WaitForSeconds(1f);
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
    
}