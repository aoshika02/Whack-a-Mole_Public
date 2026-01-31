using UnityEngine;

public class ScoreAddPool : SingletonMonoBehaviour<ScoreAddPool>
{
    [SerializeField] private GameObject _scoreAddPrefab;
    private GenericObjectPool<ScoreAddObj> _pool;
    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            return;
        }
        _pool = new GenericObjectPool<ScoreAddObj>(_scoreAddPrefab, transform);
    }
    public ScoreAddObj GetScoreAddObj()
    {
        return _pool.Get();
    }
    public void ReleaseScoreAddObj(ScoreAddObj obj)
    {
        obj.transform.SetParent(transform);
        _pool.Release(obj);
    }
}
