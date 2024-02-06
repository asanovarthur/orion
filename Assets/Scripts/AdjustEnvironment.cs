using System.Linq;
using UnityEngine;

public class AdjustEnvironment : MonoBehaviour
{
    private Camera _camera;

    private float _bgRightBound;
    private float _groundRightBound;

    private readonly float _bgXOffset = -0.02f;
    private readonly float _groundXOffset = -0.15f;

    [SerializeField] GameObject _backgroundLeftPart;
    [SerializeField] GameObject _backgroundRightPart;

    [SerializeField] GameObject _groundLeftPart;
    [SerializeField] GameObject _groundRightPart;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        MoveEnvObjectToRight(_backgroundLeftPart, _backgroundRightPart, ref _bgRightBound, _bgXOffset);
        MoveEnvObjectToRight(_groundLeftPart, _groundRightPart, ref _groundRightBound, _groundXOffset);
    }

    private void MoveEnvObjectToRight(GameObject leftHalf, GameObject rightHalf, ref float rightBound, float xOffset)
    {
        var zDistance = Mathf.Abs(leftHalf.transform.position.z - gameObject.transform.position.z);
        var cameraLeftBoundX = _camera.ViewportToWorldPoint(new Vector3(0, 0, zDistance)).x;
        var halfWidth = leftHalf.GetComponentsInChildren<SpriteRenderer>().Aggregate(0f, (acc, sprite) => acc + sprite.bounds.size.x);

        if (rightBound == 0)
            rightBound = cameraLeftBoundX + halfWidth;

        if (cameraLeftBoundX >= rightBound)
        {
            var halfToMove = (new GameObject[] { leftHalf, rightHalf }).OrderBy(obj => obj.transform.position.x).First();
            halfToMove.transform.position = new Vector3(halfToMove.transform.position.x + halfWidth * 2 + xOffset, halfToMove.transform.position.y, halfToMove.transform.position.z);
            
            rightBound = cameraLeftBoundX + halfWidth;
        }
    }
}
