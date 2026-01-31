using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EffectObj : MonoBehaviour, IPool
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _effectImage;
    public bool IsGenericUse { get; set; }
    private bool _isCall = false;
    public void Init(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }
    public async void PlayAsync()
    {
        if(_isCall)return;
        _isCall = true;
        await _effectImage.rectTransform.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBack).ToUniTask();
        _isCall = false;
        EffectPool.Instance.Release(this);
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        _effectImage.rectTransform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }
}

public static class ParticleSystemExtension
{
    public static async UniTask PlayAsync(this ParticleSystem ps, CancellationToken token)
    {
        ps.Play();

        await UniTask.WaitUntil(
            () => !ps.IsAlive(true),
            cancellationToken: token
        );
    }
}
