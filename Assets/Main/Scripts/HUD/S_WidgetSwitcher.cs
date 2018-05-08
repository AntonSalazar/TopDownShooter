using UnityEngine;

public class S_WidgetSwitcher : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject[] _Widgets;
    [SerializeField] int _DefaultIndex = 0;

    int _PrevIndex = -1;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < _Widgets.Length; i++)
        {
            _Widgets[i].SetActive(false);
        }
        SetWidgetActive(_DefaultIndex);
    }

    internal void SetWidgetActive(int _index)
    {
        if(_PrevIndex != -1) _Widgets[_PrevIndex].SetActive(false);
        _Widgets[_index].SetActive(true);
        _PrevIndex = _index;
    }
}
