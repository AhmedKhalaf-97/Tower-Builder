using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public Slider antiAliasingSlider;
    public Text antiAliasingText;

    public Slider renderScaleSlider;
    public Text renderScaleText;

    public Button sprBatcher;
    public Text sprBatcherStatus;

    UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset lwrpAsset;

    void Awake()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        lwrpAsset = GraphicsSettings.renderPipelineAsset as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
    }

    void Start()
    {
        if (antiAliasingSlider == null)
            return;

        antiAliasingSlider.value = lwrpAsset.msaaSampleCount;
        renderScaleSlider.value = lwrpAsset.renderScale;
    }

    void Update()
    {
        if (antiAliasingSlider == null)
            return;

        antiAliasingText.text = antiAliasingSlider.value.ToString();
        renderScaleText.text = renderScaleSlider.value.ToString();
        if (lwrpAsset.useSRPBatcher)
            sprBatcherStatus.text = ": Enabled";
        else
            sprBatcherStatus.text = ": Disabled";

        if (antiAliasingSlider.value == 1)
            lwrpAsset.msaaSampleCount = 1;

        if (antiAliasingSlider.value == 2)
            lwrpAsset.msaaSampleCount = 2;

        if (antiAliasingSlider.value == 4)
            lwrpAsset.msaaSampleCount = 4;

        if (antiAliasingSlider.value == 8)
            lwrpAsset.msaaSampleCount = 8;

        lwrpAsset.renderScale = renderScaleSlider.value;
    }

    public void UseSRPBatcher()
    {
        lwrpAsset.useSRPBatcher = !lwrpAsset.useSRPBatcher;
    }

    public void GoToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
