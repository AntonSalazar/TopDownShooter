using UnityEngine;
using System.Linq;
public class S_PlayerController : MonoBehaviour
{
    static Vector2 _Input;
    internal static Vector2 m_Input
    {
        get
        {
            _Input.x = Input.GetAxis("Horizontal");
            _Input.y = Input.GetAxis("Vertical");
            return _Input;
        }
    }

    static bool _Fire;
    internal static bool m_Fire
    {
        get
        {
            _Fire = Input.GetKey(KeyCode.Mouse0);
            return _Fire;
        }
    }

    static bool _Escape;
    internal static bool m_Escape
    {
        get
        {
            _Escape = Input.GetKeyDown(KeyCode.Escape);
            return _Escape;
        }
    }

    static bool g_ControlledCharacter;
    static Transform _ControlledCharacter;
    internal static Transform m_ControlledCharacter
    {
        get
        {
            if (!g_ControlledCharacter)
                if (_ControlledCharacter = FindObjectsOfType<S_Character>().Where(ch => ch.m_IsPlayer).ToArray()[0].transform) g_ControlledCharacter = true;
            return _ControlledCharacter;
        }
    }
}
