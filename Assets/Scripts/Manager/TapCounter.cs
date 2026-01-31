public class TapCounter : SingletonMonoBehaviour<TapCounter>
{
    private int _hitCount = 0;
    private int _missCount = 0;
    public void OnTap(AnimalType type)
    {
        if (type == AnimalType.None) return;
        if (type == AnimalType.Crocodile)
        {
            _hitCount++;
            return;
        }
        _missCount++;
    }
    public int GetHitCount()
    {
        return _hitCount;
    }
    public int GetMissCount()
    {
        return _missCount;
    }
}
