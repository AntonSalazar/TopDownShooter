using UnityEngine;
using System.Collections;

public class S_Camera : MonoBehaviour
{
    [Header("Camera Position")]
    [SerializeField] Transform[] _CameraPoints;
    [SerializeField] AnimationCurve _Curve;
    [SerializeField] float _Speed = 1.0f;

    [Header("FX")]
    [SerializeField] CameraFX _CameraFX;

    float _Timer;
    bool _isSubscribedTick, _isTimed;
    int _PrevIndex, _NextIndex, _CurrentIndex;

    static CameraFX _SCameraFX;
    static S_Camera _Instance;

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

    private void Awake()
    {
        _Instance = this;
    }

    private void OnEnable()
    {
        if(!_isSubscribedTick) GM_Main.SubscribeTick(Tick, ref _isSubscribedTick);
        _SCameraFX = _CameraFX;
    }

    private void OnDisable()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
    }

    private void OnDestroy()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
    }

    private void Tick(float _deltaTime, GM_Main.GameState _gameState)
    {
        if (_isTimed) TimerTick(_deltaTime);
    }

    private void TimerTick(float _deltaTime)
    {
        _Timer = Mathf.Clamp01(_Timer + _deltaTime * _Speed);
        m_Transform.position = Vector3.Lerp(_CameraPoints[_PrevIndex].position, _CameraPoints[_NextIndex].position, _Curve.Evaluate(_Timer));
        m_Transform.rotation = Quaternion.Slerp(_CameraPoints[_PrevIndex].rotation, _CameraPoints[_NextIndex].rotation, _Curve.Evaluate(_Timer));
        if (_Timer >= 1.0f)
        {
            _Timer = 0.0f;
            _isTimed = false;
        }
    }

    internal void MoveCameraToPoint(int _index)
    {
        _PrevIndex = _NextIndex;
        _NextIndex = _index;
        _isTimed = true;
    }

    public void E_MoveCameraNext()
    {
        MoveCameraToPoint(++_CurrentIndex);
    }

    public void E_MoveCameraBack()
    {
        MoveCameraToPoint(--_CurrentIndex);
    }

    internal static void ShakeCamera(float _magnitude, float _roughness, float _fadeInTime, float _fadeOutTime)
    {
        _SCameraFX.ShakeCamera(_magnitude, _roughness, _fadeInTime, _fadeOutTime);
    }

    internal static void Glicth()
    {
        _Instance.StartCoroutine(_Instance.Glitch());
    }

    internal IEnumerator Glitch()
    {
        bool _isVerticalJump = true, _isDigital = true;
        while (_isVerticalJump || _isDigital)
        {
            if(_isVerticalJump) _SCameraFX.VerticalJump(ref _isVerticalJump);
            if (_isDigital) _SCameraFX.Digital(ref _isDigital);

            yield return null;
        }
    }

}

[System.Serializable]
class CameraFX
{
    [Header("Analog")]
    [SerializeField] Kino.AnalogGlitch _AnalogGlitch;
    [SerializeField] AnimationCurve _VerticalJumpCurve;
    [SerializeField, Range(0.0f, 5.0f)] float _VerticalJumpDuration = 1.0f;
    float vj_Timer;

    [Header("Digital")]
    [SerializeField] Kino.DigitalGlitch _DigitalGlitch;
    [SerializeField] AnimationCurve _DigitalCurve;
    [SerializeField, Range(0.0f, 5.0f)] float _DigitalDuration = 1.0f;
    float d_Timer;

    [Header("Shaker")]
    [SerializeField] EZCameraShake.CameraShaker _Shaker;

    internal void ShakeCamera(float _magnitude, float _roughness, float _fadeInTime, float _fadeOutTime)
    {
        _Shaker.ShakeOnce(_magnitude, _roughness, _fadeInTime, _fadeOutTime);
    }

    internal void Digital(ref bool _isDigital)
    {
        _DigitalGlitch.intensity = Mathf.Lerp(0.0f, 1.0f, _DigitalCurve.Evaluate(d_Timer));
        Timing(ref _isDigital, ref d_Timer, _DigitalDuration);
    }

    internal void VerticalJump(ref bool _isVJump)
    {
        _AnalogGlitch.verticalJump = Mathf.Lerp(0.0f, 1.0f, _VerticalJumpCurve.Evaluate(vj_Timer));
        Timing(ref _isVJump, ref vj_Timer, _VerticalJumpDuration);
    }

    void Timing(ref bool _isEffected, ref float _timer, float _duration)
    {
        _timer = Mathf.Clamp01(_timer + Time.deltaTime * _duration);
        if (_timer >= 1.0f)
        {
            _timer = 0.0f;
            _isEffected = false;
        }
    }
}
