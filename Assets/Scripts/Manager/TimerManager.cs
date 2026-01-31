using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine;

public class TimerManager : SingletonMonoBehaviour<TimerManager>
{
    private bool _isCounting = false;
    public IReadOnlyReactiveProperty<float> Timer => _time;
    private readonly ReactiveProperty< float> _time = new ReactiveProperty<float>(0f);
    private CancellationTokenSource _cts = new CancellationTokenSource();
    protected override void Awake()
    {
        if (CheckInstance() == false) return;
    }
    // 開始時間セット
    public void SetStartTime(float startTime)
    {
        _time.Value = startTime;
    }
    // タイマー開始
    public void StartTimer()
    {
        if (_isCounting) return;
        _isCounting = true;
        TimerAsync().Forget();
    }
    // タイマー非同期処理
    private async UniTask TimerAsync()
    {
        try
        {
            _cts = new CancellationTokenSource();
            while (_isCounting)
            {
                await UniTask.Yield(_cts.Token);
                _time.Value += Time.deltaTime;
            }
        }
        finally
        {
            _isCounting = false;
        }
    }
    // 時間取得
    public float GetTime()
    {
        return _time.Value;
    }
    // タイマー停止
    public void StopTimer()
    {
        _cts = new CancellationTokenSource();
    }
    private void OnDestroy()
    {
        StopTimer();
    }
}
