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

        // ��������� ������ �� ����������� �� PlayerPrefs ��� ������� ���������
        imageUrl = PlayerPrefs.GetString("ImageURL");

        // �������� ����������� � ����������� �� ������
        StartCoroutine(LoadImage());

        // ���������� ����������� ������� ������� �� ������ "�����"
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        // ������� � �������
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
