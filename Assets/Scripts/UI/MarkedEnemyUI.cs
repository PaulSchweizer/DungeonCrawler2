using UnityEngine;
using UnityEngine.UI;

public class MarkedEnemyUI : MonoBehaviour
{
    public Text Name;
    public Slider Slider;
    public CharacterSet PlayerSet;
    public Canvas Canvas;
    public BaseCharacter MarkedEnemy;

    private void Awake()
    {
        Canvas.enabled = false;
    }

    public void MarkedEnemyChanged()
    {
        MarkedEnemy = PlayerSet.Items[0].MarkedEnemy;
        if (MarkedEnemy == null)
        {
            Canvas.enabled = false;
            return;
        }
        Name.text = MarkedEnemy.Stats.Name;
        Slider.maxValue = MarkedEnemy.Stats.Health.MaxValue;
        Slider.minValue = MarkedEnemy.Stats.Health.MinValue;
        Canvas.enabled = true;
    }

    public void Update()
    {
        if (MarkedEnemy != null)
        {
            Slider.value = MarkedEnemy.Stats.Health.Value;
        }
    }
}
