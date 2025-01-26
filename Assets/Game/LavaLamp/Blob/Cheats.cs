using UnityEngine;

namespace Game.LavaLamp.Blob
{
    public class Cheats : MonoBehaviour
    {
        [SerializeField]
        private global::Blob _blob;

        private void Start()
        {
#if UNITY_EDITOR
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }

        private void Update()
        {
            if (GameInput.Instance._jumpPressed)
            {
                
                Bubble b = new Bubble();
                b._colorID = 0;
                _blob._playerBubbleMono._bubble.AbsorbBubble(_blob, b);
                return;
            }

            if (GameInput.Instance._buildMenuPressed)
            {
                Bubble b = new Bubble();
                b._colorID = 1;
                _blob._playerBubbleMono._bubble.AbsorbBubble(_blob, b);
                return;
            }
        }
    }
}