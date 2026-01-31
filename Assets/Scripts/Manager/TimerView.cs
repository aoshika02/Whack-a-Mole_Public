using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TimerView : SingletonMonoBehaviour<TimerView>
{
    [SerializeField] private Gradient _timerGradient;
    [SerializeField] private Image _timerImage;
    GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        TimerManager.Instance.Timer.Subscribe(x =>
        {
            UpdateTimerView(x);
        }).AddTo(this);
    }

    private void UpdateTimerView(float time)
    {
        float normalizedTime = Mathf.Clamp01(time / _gameManager.MaxTime);
        _timerImage.fillAmount = normalizedTime;
        _timerImage.color = _timerGradient.Evaluate(normalizedTime);
    }
}
