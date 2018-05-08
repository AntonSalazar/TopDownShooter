using System;
using UnityEngine;

public class S_Room : MonoBehaviour
{
    [Header("Obstacles")]
    [SerializeField] Vector2Int _Cells;
    [SerializeField] S_Obstacle _Obstacle;
    [SerializeField] Transform _SpawnPoint;
    [SerializeField, Range(0.0f, 1.0f)] float _GenerationChance = 0.5f;
    [SerializeField] float _Step;

    [Header("Characters")]
    [SerializeField] S_Character _Player;
    [SerializeField] S_Character _AI;
    [SerializeField] Vector4 _LiveBoard;
    

    bool[,] _BusyCells;
    S_Obstacle[] _Obstacles = new S_Obstacle[0];
    S_Character[] _OnScene = new S_Character[0];
    bool _isSubscribedTick, _isSubscribedStartGame;

    private void OnEnable()
    {
        if (!_isSubscribedTick) GM_Main.SubscribeTick(Tick, ref _isSubscribedTick);
        if (!_isSubscribedStartGame) GM_Main.SubscribeStartGame(StartGame, ref _isSubscribedStartGame);

        _BusyCells = new bool[_Cells.x,_Cells.y];
    }

    private void OnDisable()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
        if (_isSubscribedStartGame) GM_Main.UnsubscribeStartGame(StartGame, ref _isSubscribedStartGame);
    }

    private void OnDestroy()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
        if (_isSubscribedStartGame) GM_Main.UnsubscribeStartGame(StartGame, ref _isSubscribedStartGame);
    }

    private void Start()
    {
        RegenerateObstacles();
    }

    private void Tick(float _deltaTime, GM_Main.GameState _gameState)
    {
        if (_gameState == GM_Main.GameState.Gameplay)
        {
            Border();
        }
    }

    private void StartGame(GM_Main.Dificule dificule, GM_Main.GameParameter _gameParameter)
    {
        if (_gameParameter == GM_Main.GameParameter.Start)
        {
            RegenerateObstacles();
            SpawnCharacter(_Player);
            SpawnCharacter(_AI);
        }
        else
        {
            KiilAll();
        }
    }

    private void GenerateObstacles()
    {
        Vector3 _Location = _SpawnPoint.localPosition;
        for (int i = 0; i < _Cells.x; i++)
        {
            for (int j = 0; j < _Cells.y; j++)
            {
                float _Random = UnityEngine.Random.Range(0.0f, 1.0f);
                if (_Random <= _GenerationChance)
                {
                    _Location.x = i * _Step + _SpawnPoint.localPosition.x;
                    _Location.z = j * _Step + _SpawnPoint.localPosition.z;
                    Array.Resize(ref _Obstacles, _Obstacles.Length + 1);
                    _Obstacles[_Obstacles.Length - 1] = GM_Main.m_PoolManager.GetPoolObject(_Obstacle, _Location, Quaternion.identity) as S_Obstacle;
                    _BusyCells[i, j] = true;
                }
            }

        }
    }

    internal void RegenerateObstacles()
    {
        _BusyCells = new bool[_Cells.x, _Cells.y];
        for (int i = 0; i < _Obstacles.Length; i++)
        {
            _Obstacles[i].m_isActive = false;
        }
        _Obstacles = new S_Obstacle[0];
        GenerateObstacles();
    }

    internal void SpawnPlayer()
    {
        SpawnCharacter(_Player);
    }

    internal void SpawnAI()
    {
        SpawnCharacter(_AI);
    }

    private void KiilAll()
    {
        for (int i = 0; i < _OnScene.Length; i++)
        {
            _OnScene[i].m_isActive = false;
        }
        _OnScene = new S_Character[0];
    }

    private void SpawnCharacter(S_Character _character)
    {
        Vector2 _Coordinate = SearchFreeCell();
        Vector3 _Location = _SpawnPoint.localPosition;

        _Location.x = _Coordinate.x * _Step + _SpawnPoint.localPosition.x;
        _Location.y = 0.1f;
        _Location.z = _Coordinate.y * _Step + _SpawnPoint.localPosition.z;
        Array.Resize(ref _OnScene, _OnScene.Length + 1);
        _OnScene[_OnScene.Length - 1] = GM_Main.m_PoolManager.GetPoolObject(_character, _Location, Quaternion.identity);
        _OnScene[_OnScene.Length - 1].SetDificule(GM_Main.m_Dificule);
    }

    private Vector2 SearchFreeCell()
    {
        Vector2 _Coordinate = new Vector2(-1.0f, -1.0f);
        for (int i = 0; i < _Cells.x; i++)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
            {
                for (int j = 0; j < _Cells.y; j++)
                {
                    if (!_BusyCells[i,j])
                    {
                        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                        {
                            _Coordinate.x = i;
                            _Coordinate.y = j;
                            return _Coordinate;
                        }
                    }
                }
            }
        }
        return SearchFreeCell();
    }

    private void Border()
    {
        Transform _character = null;
        Vector3 _characterPosition = Vector3.zero;
        for (int i = 0; i < _OnScene.Length; i++)
        {
            _character = _OnScene[i].m_Transform;
            _characterPosition = _OnScene[i].m_Transform.position;
            _characterPosition.y = 0.1f;
            _character.position = _characterPosition;
            if ((_characterPosition.x <= _LiveBoard.x || _characterPosition.x >= _LiveBoard.z) ||
                (_characterPosition.z <= _LiveBoard.y || _characterPosition.z >= _LiveBoard.w))
            {
                _characterPosition.x = Mathf.Clamp(_character.position.x, _LiveBoard.x, _LiveBoard.z);
                _characterPosition.z = Mathf.Clamp(_character.position.z, _LiveBoard.y, _LiveBoard.w);
                _character.position = _characterPosition;
            }
        }
    }

}

