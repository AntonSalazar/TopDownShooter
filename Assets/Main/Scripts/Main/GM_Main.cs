using UnityEngine;

public class GM_Main : MonoBehaviour
{
    internal enum GameState
    {
        MainMenu,
        Gameplay,
        Pause
    }

    internal enum Dificule
    {
        Easy,
        Medium,
        Hard
    }

    internal enum GameParameter
    {
        Start,
        End
    }

    static GameState _GameState = GameState.MainMenu;
    internal static GameState m_GameState
    {
        set
        {
            if (_GameState != value)
            {
                _GameState = value;
                _GameEvent(m_Dificule, m_GameParameter);
                Debug.Log("Game State set to: " + _GameState.ToString());
            }
        }
        get { return _GameState; }
    }

    internal static GameParameter m_GameParameter;

    static Dificule _Dificule;
    internal static Dificule m_Dificule
    {
        set
        {
            if (_Dificule != value) _Dificule = value;
        }
        get { return _Dificule; }
    }
    
    internal delegate void TickHandler(float _deltaTime, GameState _gameState);
    static event TickHandler _Tick = (_DeltaTime, _GameState) => { };

    internal delegate void GameHandler(Dificule _dificule, GameParameter _gameParameter);
    static event GameHandler _GameEvent = (_dificule, _gameParameter) => { };

    static bool _isInited = false;
    internal static bool m_isInited
    {
        private set { _isInited = value; }
        get { return _isInited; }        
    }



    internal static void SubscribeTick(TickHandler _tick, ref bool _isSubscribed)
    {
        _Tick += _tick;
        _isSubscribed = true;
    }

    internal static void UnsubscribeTick(TickHandler _tick, ref bool _isSubscribed)
    {
        _Tick -= _tick;
        _isSubscribed = false;
    }

    internal static void SubscribeStartGame(GameHandler _handler, ref bool _isSubscribed)
    {
        _GameEvent += _handler;
        _isSubscribed = true;
    }

    internal static void UnsubscribeStartGame(GameHandler _handler, ref bool _isSubscribed)
    {
        _GameEvent -= _handler;
        _isSubscribed = false;
    }

    static bool g_PoolManager;
    static S_PoolManager _PoolManager;
    internal static S_PoolManager m_PoolManager
    {
        get
        {
            if(!g_PoolManager)
                if (_PoolManager = GameObject.FindWithTag("S_PoolManager").GetComponent<S_PoolManager>()) g_PoolManager = true;
            return _PoolManager;
        }
    }

    static bool g_Room;
    static S_Room _Room;
    internal static S_Room m_Room
    {
        get
        {
            if (!g_Room)
                if (_Room = GameObject.FindWithTag("S_Room").GetComponent<S_Room>()) g_Room = true;
            return _Room;
        }
    }

    private void Update()
    {
        _Tick(Time.deltaTime, _GameState);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDestroy()
    {
        DeInit();
    }

    private void OnDisable()
    {
        DeInit();
    }

    void Init()
    {
        m_isInited = true;
        Debug.Log("Game Mode GM_Main has inited!");
    }

    void DeInit()
    {
        m_isInited = false;
        _Tick = (_DeltaTime, _GameState) => { };
        _GameEvent = (_dificule, _gameParameter) => { };
    }

    internal static float MapValue(float _value, float _inRangeA = 0.0f, float _InRangeB = 1.0f, float _OutRangeA = 0.0f, float _OutRangeB = 1.0f)
    {
        return Mathf.Lerp(_OutRangeA, _OutRangeB, Mathf.InverseLerp(_inRangeA, _InRangeB, _value));
    }
}
