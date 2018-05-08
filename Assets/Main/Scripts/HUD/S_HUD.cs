using UnityEngine;
using UnityEngine.UI;

public class S_HUD : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] GameObject _MainMenu;
    [SerializeField] GameObject _Gameplay;
    [SerializeField] S_WidgetSwitcher _WSPanels;

    [Header("Main Menu")]
    [SerializeField] Text _TVersionProject;
    [SerializeField] string _Version = "v0.1";
    [SerializeField] S_WidgetSwitcher _WSMenu;

    [Header("Gameplay")]
    [SerializeField] Text _TScorePlayer;
    [SerializeField] Text _TScoreAI;
    [SerializeField] S_WidgetSwitcher _WSCountDown;
    [SerializeField] S_WidgetSwitcher _WSPauseMenu;

    [Header("FX")]
    [SerializeField] AudioClip _ButtonClickClip;
    [SerializeField] Animator _Animator;

    bool _ShowPause;
    bool _isSubscribedTick, _isSubscribedScore;

    int _ScorePlayer, _ScoreAI;

    public void E_FXClick()
    {
        S_AudioManager.PlayOneShoot(_ButtonClickClip);
    }

    public void E_PlayButton()
    {
        _WSMenu.SetWidgetActive(1);
    }

    public void E_EasyButton()
    {
        StartGame(GM_Main.Dificule.Easy);
    }

    public void E_MediumButton()
    {
        StartGame(GM_Main.Dificule.Medium);
    }

    public void E_HardButton()
    {
        StartGame(GM_Main.Dificule.Hard);
    }

    private void StartGame(GM_Main.Dificule _difficule)
    {
        GM_Main.m_Dificule = _difficule;
        _WSPanels.SetWidgetActive(1);

        E_BackButton();
        SetCursorVisible(false);

        _ScorePlayer = _ScoreAI = 0;
        SetScore(0, false);
        SetScore(0, true);

        _Animator.SetBool("MainMenu", false);
    }

    public void E_StartGame()
    {
        GM_Main.m_GameParameter = GM_Main.GameParameter.Start;
        GM_Main.m_GameState = GM_Main.GameState.Gameplay;
    }

    public void E_EndGame()
    {
        GM_Main.m_GameParameter = GM_Main.GameParameter.End;
        GM_Main.m_GameState = GM_Main.GameState.MainMenu;
        _WSPanels.SetWidgetActive(0);
        SetCursorVisible(true);
        _Animator.SetBool("MainMenu", true);
    }

    public void E_BackButton()
    {
        _WSMenu.SetWidgetActive(0);
    }

    public void E_QuitButton()
    {
        Application.Quit();
    }

    public void E_CountDown(int _index)
    {
        _WSCountDown.SetWidgetActive(_index);
    }

    public void E_ShowPauseMenu()
    {
        _ShowPause = !_ShowPause;
        SetCursorVisible(_ShowPause);
        switch (_ShowPause)
        {
            case true:
                _WSPauseMenu.SetWidgetActive(0);
                Time.timeScale = 0.0f;
                break;

            case false:
                _WSPauseMenu.SetWidgetActive(1);
                Time.timeScale = 1.0f;
                break;
        }
    }

    private void SetScore(int _score, bool _isPlayer)
    {
        if (_isPlayer)
        {
            _ScoreAI += _score;
            _TScoreAI.text = "AI: " + _ScoreAI;
            _Animator.SetTrigger("ScoreAI");
        }
        else
        {
            _ScorePlayer += _score;
            _TScorePlayer.text = "YOU: " + _ScorePlayer;
            _Animator.SetTrigger("ScorePlayer");
        }
    }

    private void SetCursorVisible(bool _value)
    {
        Cursor.visible = _value;
        if(!_value) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    private void Awake()
    {
        _TVersionProject.text = _Version;
        _Animator.SetBool("MainMenu", true);
    }

    private void OnEnable()
    {
        if (!_isSubscribedTick) GM_Main.SubscribeTick(Tick, ref _isSubscribedTick);
        if (!_isSubscribedScore) S_Character.SubscribeScore(SetScore, ref _isSubscribedScore);

        if (!GameObject.FindWithTag("GM_Main") || !GM_Main.m_isInited)
        {
            GM_Main _Main = new GameObject("GM_Main", typeof(GM_Main)).GetComponent<GM_Main>();
            _Main.gameObject.tag = "GM_Main";
        }
    }

    private void OnDisable()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
        if (_isSubscribedScore) S_Character.UnsubscribeScore(SetScore, ref _isSubscribedScore);
    }

    private void OnDestroy()
    {
        if (_isSubscribedTick) GM_Main.UnsubscribeTick(Tick, ref _isSubscribedTick);
        if (_isSubscribedScore) S_Character.UnsubscribeScore(SetScore, ref _isSubscribedScore);
    }

    private void Tick(float _deltaTime, GM_Main.GameState _state)
    {
        if (_state != GM_Main.GameState.MainMenu)
        {
            if (S_PlayerController.m_Escape)
            {
                E_ShowPauseMenu();
            }
        }
    }
}
