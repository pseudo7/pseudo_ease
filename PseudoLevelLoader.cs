using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PseudoLevelLoader : MonoBehaviour
{
    [SerializeField] [Range(1f, 5f)] float loadLevelTime = 2f;
    [SerializeField] Slider levelProgressSlider;
    [SerializeField] string levelName;
  
    float countdown;

    private void Awake()
    {
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        yield return null;
        string levelName = levelName;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelName);
        asyncOperation.allowSceneActivation = false;

        while (countdown < loadLevelTime)
        {
            levelProgressSlider.value = countdown / loadLevelTime;
            countdown += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        asyncOperation.allowSceneActivation = true;
    }
}
