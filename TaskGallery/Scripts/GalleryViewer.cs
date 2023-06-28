using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class GalleryViewer : MonoBehaviour
{
    public RawImage imageDisplay;
    public Button backButton;

    private string imageUrl;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // Получение ссылки на изображение из PlayerPrefs или другого источника
        imageUrl = PlayerPrefs.GetString("ImageURL");

        // Загрузка изображения и отображение на экране
        StartCoroutine(LoadImage());

        // Назначение обработчика события нажатия на кнопку "Назад"
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        // Возврат в галерею
        SceneManager.LoadScene("Gallery");
    }

    private IEnumerator LoadImage()
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                imageDisplay.texture = texture;
            }
            else
            {
                Debug.Log("Failed to download image: " + www.error);
            }
        }
    }
}
