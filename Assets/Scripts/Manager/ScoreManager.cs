using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Transform _scoreAddParent;
    private readonly ReactiveProperty<int> _totalScore = new ReactiveProperty<int>(0);
    private ScoreAddPool _scoreAddPool;
    private Queue<int> _scoreQueue = new Queue<int>();
    private bool _isAddingScore = false;

    void Start()
    {
        _scoreAddPool = ScoreAddPool.Instance;
        _totalScore.Subscribe(score =>
        {
            _scoreText.text = $"Score : {score}";
        }).AddTo(this);
    }
    public void AddScore(int score)
    {
        _scoreQueue.Enqueue(score);
        AddScoreFlow().Forget();
    }
    private async UniTask AddScoreFlow(int changeValue = 10, float duration = 0.05f)
    {
        if (_isAddingScore) return;
        _isAddingScore = true;
        while (_scoreQueue.Count > 0)
        {
            int score = _scoreQueue.Dequeue();
            if (score < 0)
            {
                await CalcFlow(-score, changeValue, CalcType.Sub, duration);
                continue;
            }
            await CalcFlow(score, changeValue, CalcType.Add, duration);
        }
        _isAddingScore = false;
    }
    private async UniTask CalcFlow(int score, int changeValue, CalcType calcType, float duration = 0.05f)
    {
        var scoreAddObj = _scoreAddPool.GetScoreAddObj();
        if (scoreAddObj == null)
        {
            return;
        }
        scoreAddObj.transform.SetParent(_scoreAddParent);
        int loopCount = score / changeValue;
        if (score % changeValue != 0)
        {
            loopCount++;
        }
        for (int i = 0; i < loopCount; i++)
        {
            scoreAddObj.SetScore(
            Mathf.Clamp(
            score - (i * changeValue),
            0,
            score),calcType);
            var calcValue = Mathf.Min(changeValue, score - (i * changeValue));
            _totalScore.Value += calcValue * (calcType == CalcType.Add ? 1 : -1);
            await UniTask.WaitForSeconds(duration);
        }
        _scoreAddPool.ReleaseScoreAddObj(scoreAddObj);
    }
    public int GetScore()
    {
        return _totalScore.Value;
    }
}
public enum CalcType
{
    Add,
    Sub
}
