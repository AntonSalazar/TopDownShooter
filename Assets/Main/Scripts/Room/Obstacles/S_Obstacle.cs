using UnityEngine;

public class S_Obstacle : S_PoolObject
{
    [Header("Obstacle Settings")]

    [SerializeField] Vector3 _TimeOffset = new Vector3(0.0f, 0.5f, 1.0f);
    [SerializeField] Vector2 _HeightOffset;

    float _Timer;
    float _Sign = 1.0f;
    Vector3 _LocalPosition = Vector3.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
    }

    private void Init()
    {
        _TimeOffset = new Vector3(_TimeOffset.x, Random.Range(_TimeOffset.x, _TimeOffset.z), _TimeOffset.z);
    }

    internal override void LifeCicle(float _deltaTime, GM_Main.GameState _gameState)
    {
        base.LifeCicle(_deltaTime, _gameState);
        Timing(_deltaTime);
        Movement();
    }

    private void Timing(float _deltaTime)
    {
        _Timer = Mathf.Clamp01(_Timer + _deltaTime * _Sign * _TimeOffset.y);
        if (_Timer == 0.0f || _Timer == 1.0f) _Sign *= -1.0f;
    }

    private void Movement()
    {
        _LocalPosition = m_Transform.localPosition;
        _LocalPosition.y = GM_Main.MapValue(_Timer, 0.0f, 1.0f, _HeightOffset.x, _HeightOffset.y);
        m_Transform.localPosition = _LocalPosition;
    }
}
