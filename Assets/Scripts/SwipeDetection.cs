using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minimumDistance = 10f;
    [SerializeField]
    private float maximumTime = 1f;
    [SerializeField, Range(0f, 1f)]
    private float directionThreshold = 0.9f;

    private InputManager inputManager;

    private Vector2 _startPosition;
    private float _startTime;

    private Vector2 _endPosition;
    private float _endTime;

    #region Setup Functions
    private void Awake() => inputManager = InputManager.Instance;

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    #endregion

    private void SwipeStart(Vector2 position, float time)
    {
        _startPosition = position;
        _startTime = time;
        Debug.Log("Start Swipe");
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        _endPosition = position;
        _endTime = time;
        DetectSwipe();
        Debug.Log("End Swipe");
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(_startPosition, _endPosition) >= minimumDistance &&
            (_endTime - _startTime) <= maximumTime)
        {
            Vector3 direction = _endPosition - _startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);

            Debug.Log("Success");
        }
        else Debug.Log("Fail");
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            Debug.Log("UP");
        }
        if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("DOWN");
        }
        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            PropManager.Instance.NextProp(1);
            Debug.Log("LEFT");
        }
        if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            PropManager.Instance.NextProp(-1);
            Debug.Log("RIGHT");
        }
    }
}
