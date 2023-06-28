using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar; // Ссылка на компонент Slider, отображающий прогресс загрузки
    public Text progressText; // Ссылка на компонент Text, отображающий проценты загрузки

    public string gallerySceneName = "Gallery"; // Название сцены "Галерея"

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(LoadGalleryScene());
    }

    private IEnumerator LoadGalleryScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gallerySceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Нормализуем прогресс загрузки (0-1)

            progressBar.value = progress;
            progressText.text = $"{progress * 100}%"; // Отображаем проценты загрузки

            yield return null;
        }
    }
}
