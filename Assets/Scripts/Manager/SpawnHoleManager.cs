using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHoleManager : SingletonMonoBehaviour<SpawnHoleManager>
{
    [SerializeField] private List<Vector3> _spawnPos = new List<Vector3>();
    private List<SpawnHole> _spawnHoles = new List<SpawnHole>();
    protected override void Awake()
    {
        Init();
    }
    private void Init()
    {
        foreach (var pos in _spawnPos)
        {
            _spawnHoles.Add(new SpawnHole() { Position = pos, IsUse = false });
        }
    }
    public SpawnHole GetSpawnSpawnHole()
    {
        List<SpawnHole> availableHoles = _spawnHoles.FindAll(hole => hole.IsUse == false);
        if (availableHoles == null || availableHoles.Count == 0)
        {
            return null;
        }
        SpawnHole spawnHole = availableHoles[UnityEngine.Random.Range(0, availableHoles.Count)];
        spawnHole.IsUse = true;
        return spawnHole;
    }
    public void ReleaseSpawnPos(SpawnHole spawnHole)
    {
        if (spawnHole != null)
        {
            spawnHole.IsUse = false;
        }
    }
    public bool IsAllUse()
    {
        List<SpawnHole> result = _spawnHoles.FindAll(hole => hole.IsUse == false);
        return (result == null || result.Count == 0);
    }
}

[Serializable]
public class SpawnHole
{
    public Vector3 Position;
    public bool IsUse;
}