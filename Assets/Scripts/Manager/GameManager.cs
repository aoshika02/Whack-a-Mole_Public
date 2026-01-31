using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private float _moveValue;
    public float MaxTime => _maxTime;
    [SerializeField] private float _maxTime = 30f;

    private SpawnHoleManager _spawnHoleManager;
    private AnimalPool _pool;
    private TimerManager _timerManager;
    private ParamByTime _paramByTime;
    private StartView _startView;
    private CancellationTokenSource _tokenSource;
    private readonly int _resetCount = 1;
    private readonly List<AnimalType> _animalTypes = new List<AnimalType>()
    {
        AnimalType.Crocodile,
        AnimalType.Crocodile,
        AnimalType.Crocodile,
        AnimalType.Turtle,
        AnimalType.Dog,
    };
    private readonly Queue<AnimalType> _animalTypeQueue = new Queue<AnimalType>();

    private void Start()
    {
        _spawnHoleManager = SpawnHoleManager.Instance;
        _pool = AnimalPool.Instance;
        _timerManager = TimerManager.Instance;
        _paramByTime = ParamByTime.Instance;
        _startView = StartView.Instance;
        _startView.Init();
        _tokenSource = new CancellationTokenSource();
        GameFlow().Forget();
    }
    private async UniTask GameFlow()
    {
        await _startView.StartAsync();
        TimeUpAsync().Forget();
        try
        {
            while (!_tokenSource.Token.IsCancellationRequested && _timerManager.GetTime() <= _maxTime)
            {
                if (_resetCount > _animalTypeQueue.Count)
                {
                    _animalTypeQueue.Clear();
                }
                var type = RandomTypeBase.GetRandomType(_animalTypeQueue, _animalTypes);
                ArrivalFlow(type, _tokenSource.Token);
                await UniTask.WaitForSeconds(_paramByTime.GetSpawnInterval(_timerManager.GetTime()), cancellationToken: _tokenSource.Token);
            }
        }
        catch { }
        await UniTask.WaitUntil(() => !_spawnHoleManager.IsAllUse());
        ResultView.Instance.SetResult(
            new ResultData
            {
                Score = ScoreManager.Instance.GetScore(),
                HitCount = TapCounter.Instance.GetHitCount(),
                MissCount = TapCounter.Instance.GetMissCount()
            });
        await ResultView.Instance.ShowAsync();
    }
    private async void ArrivalFlow(AnimalType animalType, CancellationToken cancellationToken)
    {
        //出現位置が埋まっている場合スキップする処理
        var availableHoles = _spawnHoleManager.GetSpawnSpawnHole();
        if (availableHoles == null)
        {
            return;
        }
        var spawnPos = availableHoles.Position;
        var animalObj = _pool.GetAnimalObj(animalType, spawnPos);
        await animalObj.MoveFlow(spawnPos.z + _moveValue,
            _paramByTime.GetMoveDuration(_timerManager.GetTime()),
            _paramByTime.GetWaitDuration(_timerManager.GetTime()), animalType);
        _spawnHoleManager.ReleaseSpawnPos(availableHoles);
    }
    private async UniTask TimeUpAsync()
    {
        _timerManager.StartTimer();
        await UniTask.WaitUntil(() => _timerManager.GetTime() > _maxTime);
        _tokenSource?.Cancel();
    }
}
