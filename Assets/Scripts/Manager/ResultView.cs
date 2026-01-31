using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : SingletonMonoBehaviour<ResultView>
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _hitCountText;
    [SerializeField] private TextMeshProUGUI _missCountText;
    [SerializeField] private CanvasGroup _resultViewRoot;
    [SerializeField] private Button _nextButton;
    private void Start()
    {
        _resultViewRoot.alpha = 0;
        _resultViewRoot.interactable = false;
        _resultViewRoot.blocksRaycasts = false;

        _nextButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                SceneLoadMananger.Instance.LoadSceneAsync(SceneType.InGame).Forget();
            }).AddTo(this);
    }
    public void SetResult(ResultData resultData)
    {
        _rankText.text = GetRank(resultData.Score);
        _scoreText.CountDownAsync(0, resultData.Score,destroyCancellationToken).Forget();
        _hitCountText.CountDownAsync(0, resultData.HitCount, destroyCancellationToken).Forget();
        _missCountText.CountDownAsync(0, resultData.MissCount, destroyCancellationToken).Forget();
    }
    public async UniTask ShowAsync(float endValue = 1, float duration = 0.5f)
    {
        _resultViewRoot.interactable = true;
        _resultViewRoot.blocksRaycasts = true;
        await _resultViewRoot.DOFade(endValue, duration).SetEase(Ease.Linear);
    }
    private string GetRank(int score)
    {
        if (score >= 6000) return "S";
        if (score >= 4000) return "A";
        if (score >= 2000) return "B";
        return "C";
    }

}
public record ResultData
{
    public int Score;
    public int HitCount;
    public int MissCount;
}
