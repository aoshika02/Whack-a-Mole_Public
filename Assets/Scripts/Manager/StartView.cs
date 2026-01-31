using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartView : SingletonMonoBehaviour<StartView>
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _textBG;
    [SerializeField] private TextMeshProUGUI _startText;

    public void Init()
    {
        _canvasGroup.alpha = 1;
        _textBG.rectTransform.localScale = new Vector3(1, 0, 1);
        _startText.text = "";
    }

    public async UniTask StartAsync(int maxCount = 3, float duration = 0.5f)
    {
        await _textBG.rectTransform.DOScaleY(1, duration).SetEase(Ease.OutBack).ToUniTask(cancellationToken: destroyCancellationToken);

        for (int i = maxCount; i >=0; i--)
        {
            _startText.text = i.ToString();
            await UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken);
        }
        _startText.text = "Start !!";
        await UniTask.WaitForSeconds(1, cancellationToken: destroyCancellationToken);

        await _canvasGroup.DOFade(0, duration).SetEase(Ease.Linear).ToUniTask(cancellationToken: destroyCancellationToken);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
