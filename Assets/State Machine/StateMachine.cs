using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class StateMachine
{
    public StateNode _current;
    public Dictionary<Type, StateNode> _nodes = new();
    public HashSet<ITransition> _anyTransitions = new();

    public class StateNode
    {
        public IState _state;
        public HashSet<ITransition> _transitions;

        public StateNode(IState state)
        {
            _state = state;
            _transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition)
        {
            _transitions.Add(new Transition(to, condition));
        }
    }

    public void Update()
    {
        ITransition transition = GetTransition();
        if (transition != null)
            ChangeState(transition._to);

        if (_current == null) return;

        _current?._state?.Update();
    }

    public void FixedUpdate()
    {
        if (_current == null) return;

        _current?._state?.FixedUpdate();
    }
    
    public virtual void OnGameStateChange(IState previousState, IState newState)
    {
    }
    
    public void SetState(IState state)
    {
        _current = _nodes[state.GetType()];
        _current?._state?.OnEnter();
    }

    void ChangeState(IState state)
    {
        if (_current == null) return;
        if (state == _current._state) return;

        IState previousState = _current._state;
        IState nextState = _nodes[state.GetType()]._state;

        previousState?.OnExit();
        nextState?.OnEnter();
        
        _current = _nodes[state.GetType()];
        
        OnGameStateChange(previousState, nextState);
    }

    ITransition GetTransition()
    {
        if (_anyTransitions != null)
        {
            foreach (ITransition transition in _anyTransitions)
                if (transition._condition.Evaluate())
                    return transition;
        }

        if (_current == null) return null;

        if (_current._transitions != null)
        {
            foreach (ITransition transition in _current._transitions)
                if (transition._condition.Evaluate())
                    return transition;
        }

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to)._state, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to)._state, condition));
    }

    StateNode GetOrAddNode(IState state)
    {
        StateNode node = _nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            _nodes.Add(state.GetType(), node);
        }

        return node;
    }

    public int NumStates()
    {
        return _nodes.Count;
    }

    public string[] GetStates()
    {
        string[] states = new string[_nodes.Count];
        int i = 0;
        foreach (KeyValuePair<Type, StateNode> node in _nodes)
        {
            states[i] = node.Key.Name;
            i++;
        }

        return states;
    }

    public string[] GetTransitions()
    {
        List<string> transitions = new List<string>();
        foreach (ITransition transition in _anyTransitions)
        {
            transitions.Add($"any->{transition._to.ToString()}");
        }

        foreach (KeyValuePair<Type, StateNode> node in _nodes)
        {
            foreach (ITransition transition in node.Value._transitions)
            {
                transitions.Add($"{node.Key.ToString()}->{transition._to.ToString()}");
            }
        }

        return transitions.ToArray();
    }
}