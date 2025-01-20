public interface IState
{
    void OnEnter();
    void OnExit();
    void Update();
    void FixedUpdate();
}