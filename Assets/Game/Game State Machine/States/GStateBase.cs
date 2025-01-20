using System;
using UnityEngine;

public abstract class GStateBase : StateBase
{
    protected new GStateMachineMono _context
    {
        get
        {
            try
            {
                return (GStateMachineMono)base._context;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"using the wrong context type: InvalidCastException: {e.Message}");
                return null;
            }
        }
        set => base._context = value;
    }

    protected GStateBase()
    {
    }

    protected GStateBase(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }
}