using UnityEngine;

public class EffectPool : SingletonMonoBehaviour<EffectPool>
{
    [SerializeField] private Canvas _effectCanvas;
    [SerializeField] private GameObject _effectObj;
    private GenericObjectPool<EffectObj> _pool;

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        _pool = new GenericObjectPool<EffectObj>(_effectObj, transform);
    }
    public EffectObj GetEffectObj(Vector3 screenPos)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 _effectCanvas.transform as RectTransform,
                 screenPos,
                  _effectCanvas.worldCamera,
                 out Vector2 localPos
             ))
        {
            Debug.Log($"{screenPos} -> {localPos}");
            var obj = _pool.Get();
            obj.Init(localPos);
            return obj;
        }
        return null;
    }
    public void Release(EffectObj obj)
    {
        _pool.Release(obj);
    }
}
