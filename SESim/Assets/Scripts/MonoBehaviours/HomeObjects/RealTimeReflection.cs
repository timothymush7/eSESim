using System.Collections;
using UnityEngine;

/// <summary>
/// Component which contains the functional logic for processing
/// reflections during runtime. This is specifically used by mirror
/// game objects in the scene.
/// </summary>
[RequireComponent(typeof(ReflectionProbe))]
public class RealTimeReflection : MonoBehaviour
{
    /*
        I mostly had this disabled for the mirror, because it was used
        purely for cosmetic reasons (unnecessary computation). I have 
        kept the code here in case this effect is required.
    */

    [Tooltip("Boolean which indicates whether reflections are updated in realtime.")] public bool ShowMirrorReflections = false;
    [Tooltip("Value which indicates how often the reflection is updated in realtime.")] public float RefreshProbeDelay = 0.05f;
    private ReflectionProbe Probe;
    private WaitForSeconds WaitRefreshProbe;
    private IEnumerator ProbeRefreshCoroutine;

    private void Start()
    {
        if (ShowMirrorReflections)
        {
            // Start coroutine to refresh reflections according to specified delay
            Probe = GetComponent<ReflectionProbe>();
            WaitRefreshProbe = new WaitForSeconds(RefreshProbeDelay);
            ProbeRefreshCoroutine = RefreshProbeEffect();
            StartCoroutine(ProbeRefreshCoroutine);
        }
    }

    private void OnDisable()
    {
        if (ProbeRefreshCoroutine != null)
            StopCoroutine(ProbeRefreshCoroutine);
    }

    IEnumerator RefreshProbeEffect()
    {
        while (true)
        {
            Probe.RenderProbe();
            yield return WaitRefreshProbe;
        }
    }
}
