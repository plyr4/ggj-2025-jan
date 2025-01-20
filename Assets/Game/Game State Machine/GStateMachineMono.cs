using System;
using UnityEngine;

public class GStateMachineMono : StateMachineMono
{
    protected new GStateMachine _stateMachine
    {
        get
        {
            try
            {
                return (GStateMachine)base._stateMachine;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        set => base._stateMachine = value;
    }
    [Header("State Change")]
    [SerializeField]
    protected GameEvent _onStateChange;
    [Space]
    [Header("Debug")]
    [Header("Skip Start")]
    [SerializeField]
    public bool _skipStartInEditMode;

    public override void Start()
    {
        base.Start();

        Initialize(new GStateMachine(_onStateChange), new GStateFactory(this));
    }

    private void Reset()
    {
    }
}