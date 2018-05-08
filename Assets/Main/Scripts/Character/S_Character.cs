using UnityEngine;
using System.Collections;
public class S_Character : S_PoolObject
{
    [Header("Character settings")]
    [SerializeField] bool _isPlayer;
    [SerializeField] float _MoveSpeed = 5.0f;

    [Header("Shoot Settings")]
    [SerializeField] S_Shoot _Shoot;
    [SerializeField] AudioClip _FireClip;
    [SerializeField] S_Explosion _Explosion;
    [SerializeField] float _FireRate;

    Vector3 _Input, _Euler;
    bool _Collised;
    bool _isFireReady = true;
    bool _isSubcribedGetDamage;

    internal bool m_IsPlayer
    {
        get { return _isPlayer; }
    }

    internal delegate void ScoreHandler(int _score, bool _isPlayer);
    static event ScoreHandler ScoreEvent = (_score, _isPlayer) => { };

    protected override void OnEnable()
    {
        base.OnEnable();
        _isFireReady = true;
        _Collised = false;
        if(!_isSubcribedGetDamage) S_Shoot.SubscribeGetDamage(Die, ref _isSubcribedGetDamage);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_isSubcribedGetDamage) S_Shoot.UnsubscribeGetDamage(Die, ref _isSubcribedGetDamage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_isSubcribedGetDamage) S_Shoot.UnsubscribeGetDamage(Die, ref _isSubcribedGetDamage);
    }

    internal static void SubscribeScore(ScoreHandler _handler, ref bool _isSubscribed)
    {
        ScoreEvent += _handler;
        _isSubscribed = true;
    }

    internal static void UnsubscribeScore(ScoreHandler _handler, ref bool _isSubscribed)
    {
        ScoreEvent -= _handler;
        _isSubscribed = false;
    }

    internal void SetDificule(GM_Main.Dificule _dificule)
    {
        if (m_IsPlayer)
        {
            switch (_dificule)
            {
                case GM_Main.Dificule.Easy:
                    _MoveSpeed = 3.0f;
                    _FireRate = 0.15f;
                    break;
                case GM_Main.Dificule.Medium:
                    _MoveSpeed = 2.5f;
                    _FireRate = 0.35f;
                    break;
                case GM_Main.Dificule.Hard:
                    _MoveSpeed = 2.0f;
                    _FireRate = 0.45f;
                    break;
            }
        }
        else
        {
            switch (_dificule)
            {
                case GM_Main.Dificule.Easy:
                    _MoveSpeed = 1.0f;
                    _FireRate = 1.15f;
                    break;
                case GM_Main.Dificule.Medium:
                    _MoveSpeed = 2.0f;
                    _FireRate = 0.95f;
                    break;
                case GM_Main.Dificule.Hard:
                    _MoveSpeed = 2.15f;
                    _FireRate = 0.5f;
                    break;
            }
        }

    }

    internal override void LifeCicle(float _deltaTime, GM_Main.GameState _gameState)
    {
        base.LifeCicle(_deltaTime, _gameState);
        if (_gameState == GM_Main.GameState.Gameplay)
        {
            if (_isPlayer)
            {
                _Input.x = S_PlayerController.m_Input.x;
                _Input.z = S_PlayerController.m_Input.y;
                Movement(_Input, Vector3.right);

                if (_isFireReady && S_PlayerController.m_Fire)
                {
                    Fire();
                }
            }
            else
            {
                AILogic();
            }
        }
    }

    private void Movement(Vector3 _Direction, Vector3 _FromDirection)
    {
        if (_Direction.magnitude > 0.0f)
        {
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(_Direction), Time.deltaTime * 5.0f);
            m_Transform.position += m_Transform.forward * _MoveSpeed * Time.deltaTime;
        }
    }

    private void Fire()
    {
        _isFireReady = false;
        GM_Main.m_PoolManager.GetPoolObject(_Shoot, m_Transform.position + m_Transform.forward * 0.25f, m_Transform.rotation).Init(_isPlayer);
        S_AudioManager.PlayOneShoot(_FireClip);
        StartCoroutine(FireRate());
    }

    private void Die(Transform _hitTransfrom)
    {
        if (_hitTransfrom == m_Transform)
        {
            GM_Main.m_PoolManager.GetPoolObject(_Explosion, m_Transform.position, m_Transform.rotation).Init(m_IsPlayer);

            m_isActive = false;
            if(m_IsPlayer) GM_Main.m_Room.SpawnPlayer();
            else GM_Main.m_Room.SpawnAI();
            ScoreEvent(1, m_IsPlayer);
            S_Camera.ShakeCamera(8.0f, 8.0f, 0.1f, 0.5f);
            S_Camera.Glicth();
        }
    }

    private void AILogic()
    {
        Vector3 _Direction = (m_Transform.localPosition - S_PlayerController.m_ControlledCharacter.localPosition);
        Vector3 _Normalized = _Direction.normalized;
        _Normalized.x *= -1.0f;
        _Normalized.z *= -1.0f;
        if (!_Collised) Movement(_Normalized, Vector3.forward);
    }

    private void AICollision(Collision _collision)
    {
        if (!_collision.collider.CompareTag("AI"))
        {
            Vector3 _Direction = (m_Transform.localPosition - _collision.contacts[0].point);
            Vector3 _Normalized = _Direction.normalized;
            Movement(_Normalized, Vector3.forward);
            _Collised = true;
        }
    }

    private void FixedUpdate()
    {
        if (!m_IsPlayer)
        {
            RaycastHit _hit;
            Vector3 _Direction = (m_Transform.localPosition - S_PlayerController.m_ControlledCharacter.localPosition);
            if (Physics.Raycast(m_Transform.localPosition, _Direction * -1.0f, out _hit, 30.0f))
            {
                if (_hit.transform.CompareTag("Player") && _isFireReady)
                {
                    Fire();
                }
            }
        }
    }

    private void OnCollisionStay(Collision _collision)
    {
        if(!m_IsPlayer) AICollision(_collision);
    }

    private void OnCollisionExit(Collision _collision)
    {
        if (!m_IsPlayer && !_collision.collider.CompareTag("AI"))
        {
            _Collised = false;
        }
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(_FireRate);
        _isFireReady = true;
    }
}
