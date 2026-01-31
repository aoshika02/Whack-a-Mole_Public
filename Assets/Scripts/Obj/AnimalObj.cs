using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

public class AnimalObj : MonoBehaviour, IPool
{
    public bool IsGenericUse { get; set; }
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;
    private bool _isCall = false;
    [SerializeField] private bool _isTappable = false;
    private CancellationTokenSource _tokenSource;
    private TapCounter _tapCounter;
    private EffectPool _effectPool;
    private ScoreManager _scoreManager;
    private AnimalType _animalType;


    private void Start()
    {
        _tapCounter= TapCounter.Instance;
        _effectPool = EffectPool.Instance;
        _scoreManager = ScoreManager.Instance;
        MaterialCheck();
        InputManager.Instance.OnTapped.Subscribe(x =>
        {
            if (x.obj != gameObject) return;
            OnTapped(x.screenPos);
        }).AddTo(this);
    }
    private bool MaterialCheck()
    {
        if (_material == null)
        {
            _material = _meshRenderer.material;
            _material.SetFloat(Shader.PropertyToID("_Smoothness"), 0);
        }
        return _material != null;
    }
    private void OnTapped(Vector3 screenPos)
    {
        if (_isTappable == false) return;
        _isTappable = false;
        if (_tokenSource != null)
        {
            _tokenSource.Cancel();
        }
        var effectObj = _effectPool.GetEffectObj(screenPos);
        if (effectObj != null)
        {
            effectObj.PlayAsync();
        }
        SetColor(new Color(0.05f, 0.05f, 0.05f));
        _scoreManager.AddScore(ScoreByType.GetScore(_animalType));
        _tapCounter.OnTap(_animalType);
    }
    public async UniTask MoveFlow(float targetPosZ, float duration, float waitDuration, AnimalType type)
    {
        if (_isCall) return;
        _isCall = true;
        _animalType = type;
        var returnPosZ = transform.position.z;
        try
        {
            _tokenSource = new CancellationTokenSource();
            SetColor(new Color(1, 1, 1));
            _isTappable = true;
            await UniTask.Yield();
            await transform.DOMoveZ(targetPosZ, duration).ToUniTask(cancellationToken: _tokenSource.Token);
            await UniTask.WaitForSeconds(waitDuration, cancellationToken: _tokenSource.Token);
            _isTappable = false;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
            //キャンセル時の処理
        }
        finally
        {
            await transform.DOMoveZ(returnPosZ, duration).ToUniTask(cancellationToken:destroyCancellationToken).SuppressCancellationThrow();
            _isCall = false;
            AnimalPool.Instance?.ReleaseAnimalObj(this, type);
        }
    }
    public void SetColor(Color color)
    {
        if (MaterialCheck())
        {
            _material.color = color;
        }
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _tokenSource?.Cancel();
    }
}
