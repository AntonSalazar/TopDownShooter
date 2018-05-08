using UnityEngine;

public class S_Shoot : S_PoolObject
{
    [Header("Shoot Settings")]
    [SerializeField] Vector4 _LifeZone;
    [SerializeField] float _Speed;
    [SerializeField] Material _PlayerMaterial, _AIMaterial;
    [SerializeField] AudioClip[] _HitClips;

    RaycastHit _Hit;
    bool _PlayOwner;
    Vector3 _Position, _Direction;
    MeshRenderer _Renderer;

    internal delegate void GetDamageHandler(Transform _hitTranform);
    static event GetDamageHandler GetDamageEvent = (_hitTransform) => { };

    protected override void Awake()
    {
        base.Awake();
        _Renderer = GetComponentInChildren<MeshRenderer>();
    }

    internal static void SubscribeGetDamage(GetDamageHandler _handler, ref bool _isSubscribed)
    {
        GetDamageEvent += _handler;
        _isSubscribed = true;
    }

    internal static void UnsubscribeGetDamage(GetDamageHandler _handler, ref bool _isSubscribed)
    {
        GetDamageEvent -= _handler;
        _isSubscribed = false;
    }

    internal void Init(bool _Onwer)
    {
        _PlayOwner = _Onwer;
        if (_PlayOwner) _Renderer.sharedMaterial = _PlayerMaterial;
        else _Renderer.sharedMaterial = _AIMaterial;

        switch (GM_Main.m_Dificule)
        {
            case GM_Main.Dificule.Easy:
                _Speed = 10.0f;
                break;

            case GM_Main.Dificule.Medium:
                _Speed = 15.0f;
                break;

            case GM_Main.Dificule.Hard:
                _Speed = 20.0f;
                break;
        }
    }

    internal override void LifeCicle(float _deltaTime, GM_Main.GameState _gameState)
    {
        if (_gameState != GM_Main.GameState.Pause)
        {
            LifeZoneCalculate();
            Movement();
            Rebounded();
        }
    }

    private void LifeZoneCalculate()
    {
        _Position = m_Transform.position;
        if ((_Position.x <= _LifeZone.x || _Position.x >= _LifeZone.z) ||
            (_Position.z <= _LifeZone.y || _Position.z >= _LifeZone.w))
        {
            m_isActive = false;
        }
    }

    private void Movement()
    {
        _Direction = m_Transform.forward * _Speed * Time.deltaTime;
        m_Transform.position += _Direction;
    }

    private void Rebounded()
    {
        if (Physics.Raycast(m_Transform.position, m_Transform.forward, out _Hit, _Speed * Time.deltaTime))
        {
            if (_PlayOwner && _Hit.collider.CompareTag("AI"))
            {
                m_isActive = false;
                GetDamageEvent(_Hit.transform);
            }
            else if (!_PlayOwner && _Hit.collider.CompareTag("Player"))
            {
                m_isActive = false;
                GetDamageEvent(_Hit.transform);
            }
            else
            {
                if (!_Hit.collider.CompareTag("Player") && !_Hit.collider.CompareTag("AI") && !_Hit.collider.CompareTag("IgnoreCollider"))
                {
                    Vector3.Reflect(_Direction, _Hit.normal);
                    m_Transform.rotation = Quaternion.LookRotation(Vector3.Reflect(_Direction, _Hit.normal), Vector3.up);
                    S_AudioManager.PlayOneShoot(_HitClips[Random.Range(0, _HitClips.Length)]);
                }
            }
        }
    }
}
