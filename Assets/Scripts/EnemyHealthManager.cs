using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    private GameObject _UIObject;
    private Image _healthBar;
    private float _healthAmount = 100f;

    private void Start() {
        var enemySpawnManager = GameObject.Find("EnemySpawnManager").GetComponent<EnemySpawnManager>();

        _UIObject = enemySpawnManager.UIObject;
        _healthBar = enemySpawnManager.HealthBar.GetComponent<Image>();
    }

    public void TakeDamage(float damage) {
        if (!_UIObject.activeSelf) {
            _UIObject.SetActive(true);
        }

        _healthAmount -= damage;
        _healthBar.fillAmount = _healthAmount / 100f;

        if (_healthAmount <= 0) {
            _healthAmount = 100;
            _UIObject.SetActive(false);
        }
    }
}
