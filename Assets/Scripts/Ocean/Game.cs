using System.Collections;
using System.Collections.Generic;
using Ocean;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject objectivePanel;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI matchesLeft;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject setSailText;
    [SerializeField] private CanvasGroup winTextGroup;
    [SerializeField] private CanvasGroup setSailGroup;
    [SerializeField] private CanvasGroup fadeOverlay;

    [Header("Post Match Boardwalk")]
    [SerializeField] private GameObject boardRoot;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform boardwalkSpawnPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera boardwalkCameraPoint;
    [SerializeField] private float cameraMoveDuration = 1f;
    [SerializeField] private GameObject match3Cam;
    [SerializeField] private GameObject boardwalkCam;

    [Header("Timing")]
    [SerializeField] private float sceneFadeInDuration = 1f;
    [SerializeField] private float holdDuration = 0.8f;
    [SerializeField] private float fadeToBlackDuration = 1.2f;

    private Node[,] board;
    private TilePiece[,] tilePieces;
    private System.Random random;

    private TilePiece dragStartTile;
    private TilePiece previewTile;

    private int matchesUsed;
    private int boatProgress;

    private bool gameEnded;
    private bool isResolving;
    private bool showingObjective = true;
    private bool gameStarted;
    private bool postMatchPhaseStarted;

    void Start()
    {
        SetActive(objectivePanel, true);
        SetActive(winPanel, false);
        SetActive(gameOverPanel, false);

        if (matchesText != null) matchesText.gameObject.SetActive(false);
        if (matchesLeft != null) matchesLeft.gameObject.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        StartCoroutine(FadeCanvasGroup(fadeOverlay, 1f, 0f, sceneFadeInDuration, true));
    }

    void Update()
    {
        if (!showingObjective || !Input.anyKeyDown) return;

        showingObjective = false;
        gameStarted = true;

        SetActive(objectivePanel, false);

        if (matchesText != null) matchesText.gameObject.SetActive(true);
        if (matchesLeft != null) matchesLeft.gameObject.SetActive(true);

        StartGame();
    }

    void StartGame()
    {
        random = new System.Random(GetRandomSeed().GetHashCode());

        matchesUsed = 0;
        boatProgress = 0;
        gameEnded = false;
        isResolving = false;
        postMatchPhaseStarted = false;

        SetActive(winPanel, false);
        SetActive(gameOverPanel, false);

        if (playerController != null)
            playerController.enabled = false;

        if (boardRoot != null)
            boardRoot.SetActive(true);

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
                board[x, y] = new Node(FillPiece(), new Point(x, y));

                TilePiece tile = Instantiate(tilePrefab, GetWorldPosition(x, y), Quaternion.identity)
                    .GetComponent<TilePiece>();

                tile.boardPosition = new Point(x, y);
                tile.game = this;
                tilePieces[x, y] = tile;
            }
        }
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        float xPos = centerBoard ? (x - (width - 1) / 2f) * tileSpacing : x * tileSpacing;
        float yPos = centerBoard ? (y - (height - 1) / 2f) * tileSpacing : y * tileSpacing;
        return new Vector3(xPos + boardOffset.x, yPos + boardOffset.y, 0f);
    }

    public void BeginDrag(TilePiece tile)
    {
        if (!CanInteract()) return;

        dragStartTile = tile;
        previewTile = tile;
    }

    public void UpdateDragPreview(TilePiece tile, Vector3 dragDelta, Vector3 startWorldPos)
    {
        if (gameEnded || isResolving || dragStartTile == null || tile != dragStartTile) return;

        previewTile = tile;

        Vector3 offset = Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y)
            ? new Vector3(Mathf.Clamp(dragDelta.x, -previewDistance, previewDistance), 0f, 0f)
            : new Vector3(0f, Mathf.Clamp(dragDelta.y, -previewDistance, previewDistance), 0f);

        tile.transform.position = startWorldPos + offset;
    }

    public void EndDrag(TilePiece tile, Vector3 dragDelta)
    {
        if (gameEnded || isResolving || dragStartTile == null || tile != dragStartTile || dragDelta.magnitude < dragThreshold)
        {
            ResetPreviewTile();
            dragStartTile = null;
            return;
        }

        Point start = tile.boardPosition;
        Point target = Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y)
            ? (dragDelta.x > 0 ? Point.add(start, Point.right) : Point.add(start, Point.left))
            : (dragDelta.y > 0 ? Point.add(start, Point.up) : Point.add(start, Point.down));

        ResetPreviewTile();

        if (IsInBoard(target))
            StartCoroutine(PerformSwap(start, target));

        dragStartTile = null;
    }

    void ResetPreviewTile()
    {
        if (previewTile == null) return;

        previewTile.transform.position = GetWorldPosition(previewTile.boardPosition.x, previewTile.boardPosition.y);
        previewTile = null;
    }

    bool IsSwapValid(Point a, Point b)
    {
        if (gameEnded || isResolving || !IsInBoard(a) || !IsInBoard(b) || !IsAdjacent(a, b))
            return false;

        if (board[a.x, a.y].value <= 0 || board[b.x, b.y].value <= 0)
            return false;

        SwapValues(a, b);
        bool valid = FindAllMatches().Count > 0;
        SwapValues(a, b);

        return valid;
    }

    IEnumerator AnimateTileSwap(TilePiece tileA, TilePiece tileB, Vector3 targetPosA, Vector3 targetPosB, float duration)
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

        isResolving = false;
        bool valid = IsSwapValid(a, b);
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
        bool countedThisMove = false;
        List<Point> matches = FindAllMatches();

        while (matches.Count > 0)
        {
            int progressEarned = matches.Count >= 4 ? 2 : 1;

            if (!countedThisMove)
            {
                matchesUsed++;
                countedThisMove = true;
            }

            yield return StartCoroutine(PlayBreakAnimations(matches));
            yield return new WaitForSeconds(0.05f);

            boatProgress += progressEarned;

            if (boatBuilder != null)
                boatBuilder.AddProgress(progressEarned);

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
            LoseGame();

        isResolving = false;
    }

    IEnumerator PlayBreakAnimations(List<Point> matches)
    {
        foreach (Point p in matches)
        {
            TilePiece tile = tilePieces[p.x, p.y];
            if (tile != null && tile.gameObject.activeSelf)
                StartCoroutine(tile.BreakAnimation());
        }

        yield return new WaitForSeconds(0.2f);
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
        return IsInBoard(p) ? board[p.x, p.y].value : -999;
    }

    int FillPiece()
    {
        return random.Next(1, pieces.Length + 1);
    }

    void AddPoints(List<Point> points, List<Point> newPoints)
    {
        foreach (Point p in newPoints)
            if (!points.Contains(p))
                points.Add(p);
    }

    List<Point> GetHorizontalMatch(Point p)
    {
        List<Point> result = new();
        if (!IsInBoard(p)) return result;

        int val = GetValueAtPoint(p);
        if (val <= 0) return result;

        result.Add(p);

        for (Point left = Point.add(p, Point.left); IsInBoard(left) && GetValueAtPoint(left) == val; left = Point.add(left, Point.left))
            result.Add(left);

        for (Point right = Point.add(p, Point.right); IsInBoard(right) && GetValueAtPoint(right) == val; right = Point.add(right, Point.right))
            result.Add(right);

        if (result.Count < 3) result.Clear();
        return result;
    }

    List<Point> GetVerticalMatch(Point p)
    {
        List<Point> result = new();
        if (!IsInBoard(p)) return result;

        int val = GetValueAtPoint(p);
        if (val <= 0) return result;

        result.Add(p);

        for (Point up = Point.add(p, Point.up); IsInBoard(up) && GetValueAtPoint(up) == val; up = Point.add(up, Point.up))
            result.Add(up);

        for (Point down = Point.add(p, Point.down); IsInBoard(down) && GetValueAtPoint(down) == val; down = Point.add(down, Point.down))
            result.Add(down);

        if (result.Count < 3) result.Clear();
        return result;
    }

    List<Point> GetMatch(Point p)
    {
        List<Point> combined = new();
        AddPoints(combined, GetHorizontalMatch(p));
        AddPoints(combined, GetVerticalMatch(p));
        return combined;
    }

    List<Point> FindAllMatches()
    {
        List<Point> matches = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y].value <= 0) continue;

                List<Point> match = GetMatch(new Point(x, y));
                if (match.Count >= 3)
                    AddPoints(matches, match);
            }
        }

        return matches;
    }

    void ClearMatches(List<Point> matches)
    {
        foreach (Point p in matches)
            if (board[p.x, p.y].value > 0)
                board[p.x, p.y].value = 0;
    }

    void CollapseBoard()
    {
        for (int x = 0; x < width; x++)
            CollapseColumn(x);
    }

    void CollapseColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (board[x, y].value != 0) continue;

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

    void RefillBoard()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (board[x, y].value == 0)
                    board[x, y].value = FillPiece();
    }

    void RemoveStartingMatches()
    {
        List<Point> matches = FindAllMatches();

        while (matches.Count > 0)
        {
            foreach (Point p in matches)
                if (board[p.x, p.y].value > 0)
                    board[p.x, p.y].value = FillPiece();

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
                tile.gameObject.SetActive(value > 0);

                if (value > 0)
                    tile.SetSprite(pieces[value - 1]);
            }
        }
    }

    void UpdateUI()
    {
        if (matchesText != null)
            matchesText.text = (maxMatchesAllowed - matchesUsed).ToString();
    }

    void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        postMatchPhaseStarted = true;

        SetActive(winPanel, true);
        SetActive(winText, true);

        if (winTextGroup != null) winTextGroup.alpha = 0f;
        if (fadeOverlay != null) fadeOverlay.alpha = 0f;

        yield return StartCoroutine(FadeCanvasGroup(winTextGroup, 0f, 1f, 0.6f));
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeCanvasGroup(winTextGroup, 1f, 0f, 0.8f));

        SetActive(winText, false);
        SetActive(setSailText, false);
        SetActive(winPanel, false);

        if (matchesText != null) matchesText.gameObject.SetActive(false);
        if (matchesLeft != null) matchesLeft.gameObject.SetActive(false);

        if (boardRoot != null)
            boardRoot.SetActive(false);

        if (playerObject != null && boardwalkSpawnPoint != null)
            playerObject.transform.position = boardwalkSpawnPoint.position;

        if (playerController != null)
            playerController.enabled = true;
        
        if (match3Cam != null) match3Cam.SetActive(false);
        if (boardwalkCam != null) boardwalkCam.SetActive(true);
    }
    public void SailToNextLevel()
    {
        if (!postMatchPhaseStarted) return;
        StartCoroutine(FinalSailSequence());
    }

    IEnumerator FinalSailSequence()
    {
        if (fadeOverlay != null)
            fadeOverlay.alpha = 0f;

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 0f, 1f, fadeToBlackDuration));
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("MaritimesLevel");
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration, bool disableRaycastsAfter = false)
    {
        if (group == null) yield break;

        group.alpha = from;
        group.blocksRaycasts = true;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        group.alpha = to;

        if (disableRaycastsAfter && Mathf.Approximately(to, 0f))
            group.blocksRaycasts = false;
    }

    void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        SetActive(gameOverPanel, true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    bool CanInteract()
    {
        return gameStarted && !showingObjective && !gameEnded && !isResolving;
    }

    string GetRandomSeed()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        string seed = "";

        for (int i = 0; i < 20; i++)
            seed += chars[Random.Range(0, chars.Length)];

        return seed;
    }

    void SetActive(GameObject obj, bool value)
    {
        if (obj != null) obj.SetActive(value);
    }
}