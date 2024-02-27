using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject UIObject;
    public GameObject HealthBar;

    private float _spawnDistanceMark = 10f;
    private int _spawnLevel = 1;

    private GameObject _player;

    [SerializeField] GameObject _enemyPrefab;

    private Camera _camera;

    private float _spawnMinX;
    private float _spawnMaxX;
    private float _spawnMinY = -0.8f;
    private float _spawnMaxY = 0.25f;

    private System.Random _random;

    void Start()
    {
        _player = GameObject.Find("Player");

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        _random = new System.Random();
    }

    void Update()
    {
        if (_player.transform.position.x >= (_spawnDistanceMark * _spawnLevel))
            SpawnEnemies();
    }

    private void CalculateSpawnMinMaxXY()
    {
        var cameraObj = _camera.gameObject;
        var zDistance = Mathf.Abs(_player.transform.position.z - cameraObj.transform.position.z);

        var bottomLeftPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        var topRightPoint = _camera.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        _spawnMinX = bottomLeftPoint.x;
        _spawnMaxX = topRightPoint.x;
    }

    private void SpawnEnemies()
    {
        CalculateSpawnMinMaxXY();

        float spawnX;
        float spawnY;

        float leftSpawnX = _spawnMinX - 1;
        float rightSpawnX = _spawnMaxX + 1;

        float[] xSpawns = { leftSpawnX, rightSpawnX };
        for (int i = 0; i < _spawnLevel; i++)
        {
            spawnX = xSpawns[_random.Next(0, 2)];
            spawnY = Random.Range(_spawnMinY, _spawnMaxY);
            Instantiate(_enemyPrefab, new Vector3(spawnX, spawnY, 0), _enemyPrefab.transform.rotation);
        }

        _spawnLevel++;
    }
}
