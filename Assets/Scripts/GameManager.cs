using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject gridCellPrefab;
    public GameObject destinationCellPrefab;
    public GameObject ballPrefab;

    public int rows = 6;
    public int columns = 3;
    public Vector2 offset;
    private GridCell[,] grid;

    public int destinationRows = 2;
    public int destinationColumns = 3;
    public Vector2 destinationOffset;
    private GridCell[,] destinationGrid;

    public int waitingRows = 1;
    public int waitingColumns = 3;
    public Vector2 waitingOffset;
    private GridCell[,] waitingGrid;

    [SerializeField] private Transform ballContainer;
    [SerializeField] private Transform waitingContainer;
    [SerializeField] private Transform targetContainer;
    private float cellSize = 1.5f; // Scale factor for cell size
    void Start()
    {
        CreateGrid();
        CreateDestinationGrid();
        CreateWaitingGrid();
        PlaceBallsRandomly();
        PlaceDestinationBallsRandomly();
        //PositionCamera();
    }

    void CreateGrid()
    {
        grid = new GridCell[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * cellSize, 0, row * cellSize);
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, ballContainer);
                grid[row, col] = cell.GetComponent<GridCell>();
                grid[row, col].Initialize(row, col);
            }
        }
    }

    void CreateDestinationGrid()
    {
        destinationGrid = new GridCell[destinationRows, destinationColumns];
        for (int row = 0; row < destinationRows; row++)
        {
            for (int col = 0; col < destinationColumns; col++)
            {
                Vector3 position = new Vector3((col + destinationOffset.x) * cellSize, 0, (row + destinationOffset.y) * cellSize);
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, targetContainer);
                destinationGrid[row, col] = cell.GetComponent<GridCell>();
                destinationGrid[row, col].Initialize(row, col);
            }
        }

    }
    void CreateWaitingGrid()
    {
        waitingGrid = new GridCell[waitingRows, waitingColumns];
        for (int row = 0; row < waitingRows; row++)
        {
            for (int col = 0; col < waitingColumns; col++)
            {
                Vector3 position = new Vector3((col + waitingOffset.x) * cellSize, 0, (row + waitingOffset.y) * cellSize);
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, waitingContainer);
                waitingGrid[row, col] = cell.GetComponent<GridCell>();
                waitingGrid[row, col].Initialize(row, col);
            }
        }

    }

    void PlaceDestinationBallsRandomly()
    {
        List<COLOR> colors = new List<COLOR>
    {
        COLOR.RED,
        COLOR.YELLOW,
        COLOR.GREEN,
        COLOR.BLUE,
        COLOR.PINK,
        COLOR.ORANGE
    };

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int row = 0; row < destinationRows; row++)
        {
            for (int col = 0; col < destinationColumns; col++)
            {
                availablePositions.Add(new Vector2Int(row, col));
            }
        }

        while (colors.Count > 0 && availablePositions.Count > 0)
        {
            int randomColorIndex = Random.Range(0, colors.Count);
            COLOR colorToPlace = colors[randomColorIndex];
            colors.RemoveAt(randomColorIndex);

            int randomPositionIndex = Random.Range(0, availablePositions.Count);
            Vector2Int positionToPlace = availablePositions[randomPositionIndex];
            availablePositions.RemoveAt(randomPositionIndex);

            //PlaceBall(positionToPlace.x, positionToPlace.y, colorToPlace);
            PlaceDestinationBall(destinationGrid[positionToPlace.x, positionToPlace.y], positionToPlace.x, positionToPlace.y, colorToPlace);
        }

    }
    void PlaceBallsRandomly()
    {
        List<COLOR> colors = new List<COLOR>
    {
        COLOR.RED, COLOR.RED, COLOR.RED,
        COLOR.YELLOW, COLOR.YELLOW, COLOR.YELLOW,
        COLOR.GREEN, COLOR.GREEN, COLOR.GREEN,
        COLOR.BLUE, COLOR.BLUE, COLOR.BLUE,
        COLOR.PINK, COLOR.PINK, COLOR.PINK,
        COLOR.ORANGE, COLOR.ORANGE, COLOR.ORANGE
    };

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                availablePositions.Add(new Vector2Int(row, col));
            }
        }

        while (colors.Count > 0 && availablePositions.Count > 0)
        {
            int randomColorIndex = Random.Range(0, colors.Count);
            COLOR colorToPlace = colors[randomColorIndex];
            colors.RemoveAt(randomColorIndex);

            int randomPositionIndex = Random.Range(0, availablePositions.Count);
            Vector2Int positionToPlace = availablePositions[randomPositionIndex];
            availablePositions.RemoveAt(randomPositionIndex);

            PlaceBall(positionToPlace.x, positionToPlace.y, colorToPlace);
        }
    }

    void PlaceBall(int row, int col, COLOR color)
    {
        Vector3 position = new Vector3(col * cellSize, 0.5f, row * cellSize);
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity, ballContainer);
        BallController ballController = ball.GetComponent<BallController>();
        ballController.SetColor(color);
        grid[row, col].SetBall(ballController);
    }
    void PlaceDestinationBall(GridCell gridCell, int row, int col, COLOR color)
    {
        Vector3 position = new Vector3(col, 0.5f, row);
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity, ballContainer);
        BallController ballController = ball.GetComponent<BallController>();
        ballController.IsDisabled = true;
        ballController.SetColor(color);
        gridCell.SetBall(ballController);
        ball.transform.position = gridCell.transform.position + Vector3.up * 0.5f;
    }

    public bool IsPathAvailable(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == end) return true;

            foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + direction;
                if (IsValidCell(neighbor) && !visited.Contains(neighbor) && grid[neighbor.x, neighbor.y].IsEmpty())
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return false;
    }

    bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < rows && cell.y >= 0 && cell.y < columns;
    }

    public GridCell GetGridCell(int row, int col)
    {
        if (IsValidCell(new Vector2Int(row, col)))
        {
            return grid[row, col];
        }
        return null;
    }


    void PositionCamera()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = rows / 2f;
        Camera.main.transform.position = new Vector3(columns / 2f - 0.5f, rows, rows / 2f - 0.5f);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public GridCell GetAvailableWaitingGridCell()
    {
        for (int row = 0; row < waitingRows; row++)
        {
            for (int col = 0; col < waitingColumns; col++)
            {
                if (waitingGrid[row, col].IsEmpty())
                    return waitingGrid[row, col];
            }
        }
        return null;
    }

    public GridCell GetColorMatch(COLOR colorToCheck)
    {
        for (int col = 0; col < destinationColumns; col++)
        {
            GridCell cell = destinationGrid[destinationRows - 1, col];
            if (cell.GetBall() != null && cell.GetBall().GetColor() == colorToCheck)
            {
                cell.IncreaseCount();
                if (cell.IsFullCount())
                {
                    cell.ResetCount();
                    //delete ball
                    ShiftDestinationColumn(col);
                }
                return cell;
            }
        }
        return null;
    }

    public void ShiftDestinationColumn(int col)
    {
        GridCell cell = destinationGrid[destinationRows - 1, col];
        BallController ballController = cell.GetBall();
        Destroy(ballController.gameObject);
        cell.ClearBall();
        for (int i = destinationRows-1; i > 0; i--)
        {
            if (i - 1 < 0) break;

            ballController = destinationGrid[i - 1, col].GetBall();
            if (ballController == null) break;
            
            cell.SetBall(ballController);
            ballController.transform.position = destinationGrid[i, col].transform.position + Vector3.up * 0.5f;
            destinationGrid[i - 1, col].ClearBall();
            //Next cell
            cell = destinationGrid[i - 1, col];
        }
    }
}
