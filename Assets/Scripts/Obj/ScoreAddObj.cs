using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreAddObj : MonoBehaviour, IPool
{
    private TextMeshProUGUI _textMeshPro;
    public bool IsGenericUse { get; set; }
    private bool TmpCheck()
    {
        if (_textMeshPro == null)
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }
        return _textMeshPro != null;
    }
    public void SetScore(int score, CalcType calcType)
    {
        if (TmpCheck() == false) return;
        _textMeshPro.text = $"{(calcType == CalcType.Add ? "+" : "-")} {score}";
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
}
