using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [System.Serializable]
    public class PseudoScene
    {
        public string name;
        public ScenePages scenePage;
        public GameObject[] gameObjectsToActivate;
        public GameObject[] gameObjectsToDeactivate;

        public void DoAction()
        {
            foreach (GameObject go in gameObjectsToDeactivate)
                go.SetActive(false);
            foreach (GameObject go in gameObjectsToActivate)
                go.SetActive(true);
            Instance.DisableLoadingAfter(1f);
        }
    }

    public static LoadingController Instance;

    public PseudoScene[] pseudoScenes;

    public GameObject loadingGO;

    private void OnEnable()
    {
        Debug.LogError("Load Value " + DDOL_Navigation.hasLoadedDiningScene);

        if (DDOL_Navigation.hasLoadedDiningScene)
        {
            foreach (PseudoScene pseudoScene in pseudoScenes)
                if (pseudoScene.scenePage == DDOL_Navigation.currentPage)
                    pseudoScene.DoAction();
        }
    }

    public void LoadDiningScene()
    {
        SceneManager.LoadScene(PseudoLevelLoader.IsTabletResolutionWide ? PseudoLevelLoader.diningScene10InchName : PseudoLevelLoader.diningScene8InchName);
    }

    public void LoadSignatureScene()
    {
        SceneManager.LoadScene(PseudoLevelLoader.IsTabletResolutionWide ? PseudoLevelLoader.signatureScene10InchName : PseudoLevelLoader.signatureScene8InchName);
    }

    private void Awake()
    {
      
        if (!Instance)
            Instance = this;
    }


    public void ToggleLoading(bool enable)
    {
        loadingGO.SetActive(enable);
    }

    public void DisableLoadingAfter(float seconds)
    {
        StartCoroutine(DisableCoroutine(seconds));
    }

    IEnumerator DisableCoroutine(float delay)
    {
        ToggleLoading(true);
        yield return new WaitForSeconds(delay);
        ToggleLoading(false);
    }

}
