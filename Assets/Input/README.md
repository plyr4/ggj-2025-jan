# Unity Input System

A library for handling input using [plyr4/unity-event-system](https://github.com/plyr4/unity-event-system).

### How To Use

1. Create a game object and attach a `PlayerInput` monobehaviour.

#### Adding New Actions

When adding a new action add boiler plate code to the following files:
- `GameInput.cs`

#### Polling for Input

Use the singleton.

```csharp
if (GameInput.Instance._firePressed)
```

Wrap with GameState
    
```csharp
public bool ShouldReceiveInput()
{
    if (!_viewParent.activeSelf) return false;
    switch (GStateMachineGame.Instance.CurrentState())
    {
        case GStatePause _: return true;
        default: return false;
    }
}

public void Update()
{
    if (!ShouldReceiveInput()) return;
    if (GameInput.Instance._firePressed)
    {
        // do something
    }
}
```
