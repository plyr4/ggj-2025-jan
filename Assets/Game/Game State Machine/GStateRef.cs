using System;
using System.Collections.Generic;

[Serializable]
public class GStateRef
{
    public string _type;

    public GStateBase CreateInstance()
    {
        Type type = Type.GetType(_type);
        return (GStateBase)Activator.CreateInstance(type, new object[2]
        {
            null,
            null
        });
    }
}

[Serializable]
public class GStates
{
    public GStateRef[] _states;
    public List<GStateBase> _internalStates;
    public bool _dirty = true;

    public List<GStateBase> ToList()
    {
        if (_states?.Length != _internalStates?.Count) _dirty = true;
        if (_dirty)
        {
            _internalStates = new List<GStateBase>();
            foreach (GStateRef stateRef in _states)
            {
                _internalStates.Add(stateRef.CreateInstance());
            }

            _dirty = false;
        }

        return _internalStates;
    }
}