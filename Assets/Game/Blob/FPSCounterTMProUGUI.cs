using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounterTMProUGUI : MonoBehaviour
{
    private const float fpsMeasurePeriod = 0.5f;
    [SerializeField]
    public string display = "{0}";
    private int m_CurrentFps;
    private int m_FpsAccumulator;
    private float m_FpsNextPeriod;
    private TextMeshProUGUI m_Text;

    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            m_Text.text = string.Format(display, m_CurrentFps);
        }
    }
}