using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public class StreamManager : MonoBehaviour
{
    public RawImage background;
    public AspectRatioFitter fitter;

    [Range(1, 100)]
    public int camQuality = 20;

    public bool useFrontCamera;

    WebCamTexture webCam;

    void Start()
    {
        WebCamDevice[] camDevices = WebCamTexture.devices;

        if (camDevices.Length == 0)
        {
            Debug.LogWarning("No Cameras Available");
            return;
        }

        SetUpWebCam();

        if (!webCam)
            useFrontCamera = !useFrontCamera;

        SetUpWebCam();

        if (!webCam)
        {
            Debug.LogWarning("No Front Camera");
            return;
        }

        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        webCam.Play();
        background.texture = webCam;
    }

    void SetUpWebCam()
    {
        int width = (Screen.width / 100) * camQuality;
        int height = (Screen.height / 100) * camQuality;

        foreach (var cam in WebCamTexture.devices)
            if (useFrontCamera)
            {
                if (cam.isFrontFacing)
                    webCam = new WebCamTexture(cam.name, width, height);
            }
            else
            {
                if (!cam.isFrontFacing)
                    webCam = new WebCamTexture(cam.name, width, height);
            }
    }

    private void Update()
    {
        fitter.aspectRatio = webCam.width / (float)webCam.height;
        background.rectTransform.localScale = new Vector3(1, webCam.videoVerticallyMirrored ? -1 : 1, 1);
        background.rectTransform.localEulerAngles = new Vector3(0, 0, -webCam.videoRotationAngle);
    }
}
