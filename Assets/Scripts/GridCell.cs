using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int row;
    public int column;
    private BallController ball;

    private int maxCount = 3;
    private int currentCount = 0;
    public void Initialize(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public bool IsEmpty()
    {
        return ball == null;
    }

    public void SetBall(BallController ball)
    {
        this.ball = ball;
        ball.SetCurrentCell(this);
    }

    public void ClearBall()
    {
        ball = null;
    }

    public BallController GetBall()
    {
        return ball;
    }

    public void IncreaseCount()
    {
        currentCount++;
    }
    public bool IsFullCount()
    {
        return currentCount >= maxCount;
    }
    public void ResetCount()
    {
        currentCount=0;
    }
}
