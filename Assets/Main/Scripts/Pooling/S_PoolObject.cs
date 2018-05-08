using UnityEngine;
using System.Collections;

public class S_PoolObject : MonoBehaviour
{
    [Header("Pool Object Settings")]
    [SerializeField] protected bool _UseLifeTime = true;
    [SerializeField] protected float _LifeTime = 5.0f;

    bool _isSubscribedTick;

    bool _isActive;
    internal bool m_isActive
    {
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (_UseLifeTime)
                {
                    switch (_isActive)
                    {
                        case false:
                            StopAllCoroutines();
                            gameObject.SetActive(false);
                            break;
                        case true:
                            gameObject.SetActive(true);
                            StartCoroutine(_ILifeCicle());
                            break;
                    }
                }
                else { gameObject.SetActive(_isActive); }
            }
        }
        get { return _isActive; }
    }

    bool g_Transform;
    Transform _Transform;
    internal Transform m_Transform
    {
        get
        {
            if (!g_Transform)
                if (_Transform = transform) g_Transform = true;
            return _Transform;
        }
    }

    private IEnumerator _ILifeCicle()
    {
        yield return new WaitForSeconds(_LifeTime);
        m_isActive = false;
    }

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if(!_isSubscribedTick) GM_Main.SubscribeTick(Tick, ref _isSubscribedTick);
    }

    protected virtual void OnDisable()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
    }

    protected virtual void OnDestroy()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
    }

    private void Tick(float _deltaTime, GM_Main.GameState _gameState)
    {
        LifeCicle(_deltaTime, _gameState);
    }
    internal virtual void LifeCicle(float _deltaTime, GM_Main.GameState _gameState)
    {

    }
}
