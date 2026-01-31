using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public static class CountDownExtensions
{
    public static async UniTask CountDownAsync(this TextMeshProUGUI textMeshProUGUI, int startValue, int endValue, CancellationToken token, float duration = 1f)
    {
        await DOTween.To(() => startValue, x =>
        {
            textMeshProUGUI.SetText(x);
        }, endValue, duration).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
    }

    public static void SetText(this TextMeshProUGUI textMeshProUGUI, int value)
    {
        textMeshProUGUI.text = $"{value}";
    }
}

