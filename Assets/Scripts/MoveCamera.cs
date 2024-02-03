using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private float _speed = 5f;

    private bool NoEnemiesOnScene()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length < 1;
    }

    private bool PlayerIsOnCenterPosition()
    {
        var player = GameObject.Find("Player");
        return player.transform.position.x >= transform.position.x;
    }

    public bool CanMoveRight => NoEnemiesOnScene() && PlayerIsOnCenterPosition();

    public void MoveRight()
    {
        transform.Translate(_speed * Time.deltaTime, 0, 0);
    }
}
