using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 8;
    public int numColors = 5;
    public TileItem tileItemPrefab;
    private Tile[,] _tiles;
    public GameObject grid;
    public AudioSource audioSource;

    [Header("Simulate Matches")]
    public int numMovesToSimulate;
    public Button buttonSimulateMoves;
    private bool continueSimulation;

    private TileItem _selectedTileItem;
    private bool _isTileItemMoving;
    private List<TileItem> tilesToRemove = new List<TileItem>();

    void Start()
    {
        InitializeGrid();
        buttonSimulateMoves.onClick.AddListener(() => SimulateRandomMoves());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<TileItem>())
            {
                if (_isTileItemMoving)
                    return;

                TileItem tileItem = hit.collider.gameObject.GetComponent<TileItem>();

                SelectTileItem(tileItem);
            }
        }
    }

    void InitializeGrid()
    {
        DestroyTiles();

        _tiles = new Tile[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 position = new Vector2(x, y);

                TileItem tileItem = Instantiate(tileItemPrefab, position, Quaternion.identity);
                tileItem.transform.SetParent(grid.transform);
                tileItem.SetIndex(x, y);
                tileItem.SetColor(Random.Range(0, numColors));

                _tiles[x, y] = new Tile(tileItem);
            }
        }

        if (CheckGridMatches())
        {
            InitializeGrid();
        }
    }

    bool CheckGridMatches()
    {
        bool hasMatched = false;

        tilesToRemove.Clear();

        foreach (Tile tile in _tiles)
        {
            if (tile.tileItem != null)
            {
                tile.tileItem.isMatched = false;
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                TileItem tileItem = _tiles[x, y].tileItem;

                if (!tileItem.isMatched)
                {
                    MatchResult matchedTiles = IsConnected(tileItem);

                    if (matchedTiles.connectedTiles.Count >= 3)
                    {

                        tilesToRemove.AddRange(matchedTiles.connectedTiles);

                        foreach (TileItem item in matchedTiles.connectedTiles)
                        {
                            item.isMatched = true;
                        }

                        hasMatched = true;
                    }
                }
            }
        }

        return hasMatched;
    }

    void DestroyTiles()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region Falling Logic

    private void RemoveAndRefill(List<TileItem> _tilesToRemove)
    {
        foreach (TileItem item in _tilesToRemove)
        {
            int xIndex = item.xIndex;
            int yIndex = item.yIndex;

            Destroy(item.gameObject);

            _tiles[xIndex, yIndex] = new Tile(null);
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (_tiles[x, y].tileItem == null)
                {
                    RefillGrid(x, y);
                }

            }
        }
    }

    private void RefillGrid(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < gridHeight && _tiles[x, y + yOffset].tileItem == null)
        {
            yOffset++;
        }

        if (y + yOffset < gridHeight && _tiles[x, y + yOffset].tileItem != null)
        {
            TileItem tileAbove = _tiles[x, y + yOffset].tileItem;

            Vector3 targetPos = new Vector3(x, y, tileAbove.transform.position.z);
            tileAbove.MoveToTarget(targetPos);

            tileAbove.SetIndex(x, y);
            _tiles[x, y] = _tiles[x, y + yOffset];

            _tiles[x, y + yOffset] = new Tile(null);
        }

        if (y + yOffset == gridHeight)
        {
            SpawnTileAtTop(x);
        }
    }

    private void SpawnTileAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMove = gridHeight - index;

        Vector2 position = new Vector2(x, gridHeight);

        TileItem tileItem = Instantiate(tileItemPrefab, position, Quaternion.identity);
        tileItem.transform.SetParent(grid.transform);
        tileItem.SetIndex(x, index);
        tileItem.SetColor(Random.Range(0, numColors));

        _tiles[x, index] = new Tile(tileItem);

        Vector3 targetPosition = new Vector3(tileItem.transform.position.x, tileItem.transform.position.y - locationToMove, tileItem.transform.position.z);
        tileItem.MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;

        for (int y = (gridHeight - 1); y >= 0; y--)
        {
            if (_tiles[x, y].tileItem == null)
            {
                lowestNull = y;
            }
        }

        return lowestNull;
    }

    #endregion

    #region Matching Logic

    MatchResult IsConnected(TileItem tileItem)
    {
        List<TileItem> connectedTiles = new List<TileItem>();
        Color tileColor = tileItem.tileColor;

        connectedTiles.Add(tileItem);

        Vector2Int rightDirection = new Vector2Int(1, 0);
        CheckDirection(tileItem, rightDirection, connectedTiles);

        Vector2Int leftDirection = new Vector2Int(-1, 0);
        CheckDirection(tileItem, leftDirection, connectedTiles);

        if (connectedTiles.Count >= 3)
        {
            return new MatchResult
            {
                connectedTiles = connectedTiles
            };
        }

        connectedTiles.Clear();
        connectedTiles.Add(tileItem);

        Vector2Int upDirection = new Vector2Int(0, 1);
        CheckDirection(tileItem, upDirection, connectedTiles);

        Vector2Int downDirection = new Vector2Int(0, -1);
        CheckDirection(tileItem, downDirection, connectedTiles);

        if (connectedTiles.Count >= 3)
        {
            return new MatchResult
            {
                connectedTiles = connectedTiles
            };
        }
        else
        {
            return new MatchResult
            {
                connectedTiles = connectedTiles
            };
        }
    }

    void CheckDirection(TileItem tileItem, Vector2Int direction, List<TileItem> connectedTiles)
    {
        Color tileColor = tileItem.tileColor;

        int x = tileItem.xIndex + direction.x;
        int y = tileItem.yIndex + direction.y;

        while (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            TileItem nextTileItem = _tiles[x, y].tileItem;

            if (!nextTileItem.isMatched && nextTileItem.tileColor == tileColor)
            {
                connectedTiles.Add(nextTileItem);

                x += direction.x;
                y += direction.y;
            }
            else
            {
                break;
            }
        }
    }

    public IEnumerator ProcessTurnOnGrid()
    {
        foreach (TileItem item in tilesToRemove)
        {
            item.isMatched = false;
        }

        RemoveAndRefill(tilesToRemove);
        GameManager.Instance.ProcessTurn(tilesToRemove.Count);
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);

        if (CheckGridMatches())
        {
            yield return StartCoroutine(ProcessTurnOnGrid());
        }
    }

    #endregion

    #region Swap Logic

    public void SelectTileItem(TileItem tileItem)
    {
        if (_selectedTileItem == null)
        {
            _selectedTileItem = tileItem;
        }
        else if (_selectedTileItem == tileItem)
        {
            _selectedTileItem = null;
        }
        else if (_selectedTileItem != tileItem)
        {
            SwapTile(_selectedTileItem, tileItem);
            _selectedTileItem = null;
        }
    }

    void SwapTile(TileItem currentTileItem, TileItem adjacentTileItem)
    {
        if (!IsAdjacent(currentTileItem, adjacentTileItem))
        {
            return;
        }

        DoSwap(currentTileItem, adjacentTileItem);

        GameManager.Instance.ProcessMovements();

        _isTileItemMoving = true;

        StartCoroutine(ProcessMatches(currentTileItem, adjacentTileItem));
    }

    void DoSwap(TileItem currentTileItem, TileItem adjacentTileItem)
    {
        TileItem tempTileItem = _tiles[currentTileItem.xIndex, currentTileItem.yIndex].tileItem;

        _tiles[currentTileItem.xIndex, currentTileItem.yIndex].tileItem = _tiles[adjacentTileItem.xIndex, adjacentTileItem.yIndex].tileItem;
        _tiles[adjacentTileItem.xIndex, adjacentTileItem.yIndex].tileItem = tempTileItem;

        int tempXIndex = currentTileItem.xIndex;
        int tempYIndex = currentTileItem.yIndex;

        currentTileItem.xIndex = adjacentTileItem.xIndex;
        currentTileItem.yIndex = adjacentTileItem.yIndex;
        adjacentTileItem.xIndex = tempXIndex;
        adjacentTileItem.yIndex = tempYIndex;

        currentTileItem.MoveToTarget(_tiles[adjacentTileItem.xIndex, adjacentTileItem.yIndex].tileItem.transform.position);
        adjacentTileItem.MoveToTarget(_tiles[currentTileItem.xIndex, currentTileItem.yIndex].tileItem.transform.position);
    }

    bool IsAdjacent(TileItem currentTileItem, TileItem adjacentTileItem)
    {
        return Mathf.Abs(currentTileItem.xIndex - adjacentTileItem.xIndex) + Mathf.Abs(currentTileItem.yIndex - adjacentTileItem.yIndex) == 1;
    }

    IEnumerator ProcessMatches(TileItem currentTileItem, TileItem adjacentTileItem)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckGridMatches())
        {
            yield return StartCoroutine(ProcessTurnOnGrid());
        }
        else
        {
            DoSwap(currentTileItem, adjacentTileItem);
        }

        _isTileItemMoving = false;
    }

    #endregion

    #region Simulate Random Moves

    public void SimulateRandomMoves()
    {
        StartCoroutine(SimulateRandomMovesRoutine(numMovesToSimulate));
    }

    IEnumerator SimulateRandomMovesRoutine(int numMoves)
    {
        for (int i = 0; i < numMoves; i++)
        {
            TileItem firstTile = _tiles[Random.Range(0, gridWidth - 1), Random.Range(0, gridHeight - 1)].tileItem;
            TileItem secondTile = GetRandomAdjacentTile(firstTile);

            yield return StartCoroutine(ProcessMatchesSimulation(firstTile, secondTile));
        }
    }

    IEnumerator ProcessMatchesSimulation(TileItem currentTileItem, TileItem adjacentTileItem)
    {
        DoSwap(currentTileItem, adjacentTileItem);

        _isTileItemMoving = true;

        GameManager.Instance.ProcessMovements();

        yield return new WaitForSeconds(0.6f);

        if (CheckGridMatches())
        {
            yield return StartCoroutine(ProcessTurnOnGrid());
        }
        else
        {
            DoSwap(currentTileItem, adjacentTileItem);
        }

        _isTileItemMoving = false;

        yield return new WaitForSeconds(0.6f);
        
    }

    TileItem GetRandomAdjacentTile(TileItem firstTile)
    {
        TileItem secondTile;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int randomDirection = directions[Random.Range(0, directions.Length)];

        int adjustedXIndex = Mathf.Clamp(firstTile.xIndex + randomDirection.x, 0, gridWidth - 1);
        int adjustedYIndex = Mathf.Clamp(firstTile.yIndex + randomDirection.y, 0, gridHeight - 1);

        secondTile = _tiles[adjustedXIndex, adjustedYIndex].tileItem;

        while (firstTile.xIndex == secondTile.xIndex && firstTile.yIndex == secondTile.yIndex)
        {
            randomDirection = directions[Random.Range(0, directions.Length)];

            adjustedXIndex = Mathf.Clamp(firstTile.xIndex + randomDirection.x, 0, gridWidth - 1);
            adjustedYIndex = Mathf.Clamp(firstTile.yIndex + randomDirection.y, 0, gridHeight - 1);

            secondTile = _tiles[adjustedXIndex, adjustedYIndex].tileItem;
        }

        return secondTile;
    }

    #endregion

}