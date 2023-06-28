using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar; // ������ �� ��������� Slider, ������������ �������� ��������
    public Text progressText; // ������ �� ��������� Text, ������������ �������� ��������

    public string gallerySceneName = "Gallery"; // �������� ����� "�������"

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
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // ����������� �������� �������� (0-1)

            progressBar.value = progress;
            progressText.text = $"{progress * 100}%"; // ���������� �������� ��������

            yield return null;
        }
    }
}
