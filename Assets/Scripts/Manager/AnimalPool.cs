using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPool : SingletonMonoBehaviour<AnimalPool>
{
    [SerializeField] private List<AnimalPoolData> _animalPoolDatas = new List<AnimalPoolData>();
    private Dictionary<AnimalType, AnimalPoolData> _animalDic = new Dictionary<AnimalType, AnimalPoolData>();

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        foreach (var poolData in _animalPoolDatas)
        {
            poolData.Pool = new GenericObjectPool<AnimalObj>(poolData.AnimalPrefab, transform);
            _animalDic.TryAdd(poolData.AnimalType, poolData);
        }
    }
    public AnimalObj GetAnimalObj(AnimalType type,Vector3 pos)
    {
        if (_animalDic.TryGetValue(type, out var poolData))
        {
            return poolData.Pool.Get(pos);
        }
        return null;
    }
    public void ReleaseAnimalObj(AnimalObj obj, AnimalType type)
    {
        if (_animalDic.TryGetValue(type, out var poolData))
        {
            poolData.Pool.Release(obj);
        }
    }
}
[Serializable]
public class AnimalPoolData
{
    public GameObject AnimalPrefab;
    public AnimalType AnimalType;
    public GenericObjectPool<AnimalObj> Pool;
}