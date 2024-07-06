using UnityEngine;
using UnityEngine.EventSystems;


public enum COLOR{NONE, RED, YELLOW, GREEN, BLUE, PINK, ORANGE, CYAN, VIOLET};
public class BallController : MonoBehaviour, IPointerClickHandler
{
    private GridCell currentCell;
    private GameManager gameManager;
    [SerializeField] private COLOR color;
    private MeshRenderer renderer;
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        
        ChangeMeshColors(GetColor(color));

    }

    public void SetCurrentCell(GridCell cell)
    {
        currentCell = cell;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Ball clicked"); // Log to ensure click is registered
        if (currentCell == null || gameManager == null)
        {
            Debug.LogError("CurrentCell or GameManager is not set");
            return;
        }

        Vector2Int start = new Vector2Int(currentCell.row, currentCell.column);
        Vector2Int[] targets = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) };

        foreach (var target in targets)
        {
            if (gameManager.IsPathAvailable(start, target))
            {
                Debug.Log("Path available to " + target); // Log to ensure path is found
                if (gameManager.IsColorMatch(color)) 
                {
                    Destroy(gameObject);
                }
                else
                {
                    GridCell targetGridCell = gameManager.GetAvailableWaitingGridCell();
                    if (targetGridCell != null)
                    {
                        MoveToTarget(targetGridCell);
                    }
                }
                
                break;
            }
        }
    }

    private void MoveToTarget(GridCell targetCell)
    {
        //GridCell targetCell = gameManager.GetGridCell(target.x, target.y);
        if (targetCell.IsEmpty())
        {
            currentCell.ClearBall();
            targetCell.SetBall(this);
            transform.position = targetCell.transform.position + Vector3.up * 0.5f;
        }
    }

    public void SetColor(COLOR colorToSet)
    {
        this.color = colorToSet;
        ChangeMeshColors(GetColor(colorToSet));
    }
    private Color GetColor(COLOR colorToCheck)
    {
        switch (colorToCheck)
        {
            case COLOR.BLUE:
                return Color.blue;
            case COLOR.GREEN:
                return Color.green;
            case COLOR.RED:
                return Color.red;
            case COLOR.CYAN:
                return Color.cyan;
            case COLOR.ORANGE:
                return new Color(1f, 0.5f, 0f); // More accurate orange color
            case COLOR.PINK:
                return new Color(1f, 0.41f, 0.71f); // Light pink
            case COLOR.VIOLET:
                return new Color(0.5f, 0f, 0.5f); // Purple
            case COLOR.YELLOW:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    private void ChangeMeshColors(Color colorToSet)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = colorToSet;
        }
        else
        {
            Debug.LogWarning("MeshRenderer component not found on ball.");
        }
    }
    public COLOR GetColor()
    {
        return color;
    }
}
