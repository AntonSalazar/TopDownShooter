using System;
using UnityEngine;

public class S_PoolManager : MonoBehaviour
{
    [SerializeField] PoolCategory[] _Pool;

    private void Awake()
    {
        PreSpawn();
    }

    private void PreSpawn()
    {
        Vector2Int _index = new Vector2Int(0, 0);
        for (int i = 0; i < _Pool.Length; i++)
        {
            _index.x = i;
            for (int j = 0; j < _Pool[i]._PoolSettings.Length; j++)
            {
                _index.y = j;
                int _Count = _Pool[i]._PoolSettings[j]._Count;
                for (int n = 0; n < _Count; n++)
                {
                    SpawnPoolObject(_Pool[i]._PoolSettings[j]._PoolObject, _index, Vector3.zero, Quaternion.identity);
                }
            }
        }
    }

    T SpawnPoolObject<T>(T _PoolObject, Vector2Int _Index, Vector3 _Location, Quaternion _Rotation) where T : S_PoolObject
    {
        Transform _Category;
        if (transform.childCount > _Index.x) _Category = transform.GetChild(_Index.x);
        else
        {
            _Category = new GameObject(_Index.x +"_" + _Pool[_Index.x]._Name).transform;
            _Category.SetParent(transform);
        }
        T _Object = Instantiate(_PoolObject, _Location, _Rotation);
        _Object.transform.SetParent(_Category);
        _Pool[_Index.x]._PoolSettings[_Index.y].AddObject(_Object);
        return _Object;
    }

    internal T GetPoolObject<T>(T _Object, Vector3 _Location, Quaternion _Rotation) where T : S_PoolObject
    {
        bool _isFounded = false;
        Vector2Int _index = new Vector2Int(-1, -1);
        T _Value = null;
        for (int i = 0; i < _Pool.Length; i++)
        {
            _Value = _Pool[i].GetPoolObject(_Object, ref _isFounded, ref _index);
            if(_isFounded) _index.x = i;
            if (_Value)
            {
                _Value.transform.position = _Location;
                _Value.transform.rotation = _Rotation;
                _Value.m_isActive = true;
                return _Value;
            }
        }
        if (_isFounded)
        {
            _Value = SpawnPoolObject(_Object, _index, _Location, _Rotation);
            _Value.m_isActive = true;
            return _Value;
        } 
        else
        {
            Debug.LogError("Pool Object: " + _Object.name + " has not founded in pool manager! Please, add this is pool object in manager!");
            return null;
        }
    }
}

[System.Serializable]
class PoolCategory
{
    [SerializeField] internal string _Name;
    [SerializeField] internal PoolSettings[] _PoolSettings;

    internal T GetPoolObject<T>(T _Object, ref bool _isFounded, ref Vector2Int _index) where T: S_PoolObject
    {
        T _Value = null;
        for (int i = 0; i < _PoolSettings.Length; i++)
        {
            _Value = _PoolSettings[i].GetPoolObject(_Object, ref _isFounded);
            if (_isFounded && _index.y != i) _index.y = i;
            if (_Value) return _Value;
        }
        return _Value;
    }

}

[System.Serializable]
class PoolSettings
{
    [SerializeField] internal string _Name;
    [SerializeField] internal  S_PoolObject _PoolObject;
    [SerializeField] internal int _Count;
    [SerializeField] S_PoolObject[] _OnScene;

    internal void AddObject(S_PoolObject _Object)
    {
        Array.Resize(ref _OnScene, _OnScene.Length + 1);
        _OnScene[_OnScene.Length - 1] = _Object;
        _Object.name = (_OnScene.Length - 1) + "_" + _Name;
    }

    internal T GetPoolObject<T>(T _Object, ref bool _isFounded) where T: S_PoolObject
    {
        T _ObjectScene = null;
        if (_Object.name == _PoolObject.name)
        {
            _isFounded = true;
            for (int i = 0; i < _OnScene.Length; i++)
            {
                if (!_OnScene[i].m_isActive)
                {
                    _ObjectScene = _OnScene[i].GetComponent<T>();
                    return _ObjectScene;
                }
            }
        }
        return _ObjectScene;
    }
}
