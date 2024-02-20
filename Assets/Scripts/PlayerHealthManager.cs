using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    private float _healthAmount = 100f;

    public void TakeDamage(float damage) {
        _healthAmount -= damage;
        _healthBar.fillAmount = _healthAmount / 100f;
    }
}
