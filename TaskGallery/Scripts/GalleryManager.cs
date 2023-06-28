using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public RectTransform contentPanel;
    public GameObject imagePrefab;
    public float spacing = 10f;
    public int initialLoadCount = 10;
    public int batchSize = 5;

    private string baseUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";
    private int totalImages = 66;
    private int loadedImages = 0;
    private HashSet<string> loadedImageUrls = new HashSet<string>();

    private ScrollRect scrollRect;
    private float imagePrefabSizeX;
    private float contentWidth;
    private float contentHeight;

    private bool isLoadingImages = false;
    private List<string> imageBatchUrls = new List<string>();

    private void Awake()
    {
        imagePrefabSizeX = imagePrefab.GetComponent<RectTransform>().sizeDelta.x;
        int rows = (totalImages + 1) / 2;
        contentWidth = (imagePrefabSizeX + spacing) * rows;

        GridLayoutGroup gridLayout = contentPanel.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 2;
        gridLayout.cellSize = new Vector2(imagePrefabSizeX, imagePrefab.GetComponent<RectTransform>().sizeDelta.y);
        gridLayout.spacing = new Vector2(spacing, spacing);
        gridLayout.padding = new RectOffset((int)spacing, (int)spacing, (int)spacing, (int)spacing);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;

        scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private async void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        await LoadInitialImagesAsync();
    }

    public async void OnScrollValueChanged(Vector2 normalizedPosition)
    {
        if (loadedImages >= totalImages)
        {
            return;
        }

        float threshold = scrollRect.content.rect.height - scrollRect.viewport.rect.height - scrollRect.content.anchoredPosition.y;

        if (threshold < scrollRect.viewport.rect.height * 0.2f)
        {
            await LoadImageBatchAsync();
        }
    }

    private async Task LoadInitialImagesAsync()
    {
        int endIndex = Mathf.Min(totalImages, initialLoadCount);
        await LoadImagesAsync(0, endIndex);
    }

    private async Task LoadImagesAsync(int startIndex, int endIndex)
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            StringBuilder imageUrlBuilder = new StringBuilder(baseUrl);
            imageUrlBuilder.Append((i + 1).ToString());
            imageUrlBuilder.Append(".jpg");
            string imageUrl = imageUrlBuilder.ToString();

            if (loadedImageUrls.Contains(imageUrl))
            {
                continue;
            }

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            var requestOperation = www.SendWebRequest();

            while (!requestOperation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                if (loadedImageUrls.Contains(imageUrl))
                {
                    continue;
                }

                GameObject imageObject = Instantiate(imagePrefab, contentPanel);
                Image imageComponent = imageObject.GetComponent<Image>();
                imageComponent.sprite = sprite;

                int imageIndex = loadedImages;
                int rowIndex = imageIndex / 2;
                int columnIndex = imageIndex % 2;

                float xPosition = (columnIndex * (imagePrefabSizeX + spacing)) - (contentWidth / 2f) + (imagePrefabSizeX / 2f);
                float yPosition = (-rowIndex * (imagePrefab.GetComponent<RectTransform>().sizeDelta.y + spacing));

                imageObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);

                loadedImages++;
                loadedImageUrls.Add(imageUrl);

                contentWidth = Mathf.Max(contentWidth, (columnIndex + 1) * (imagePrefabSizeX + spacing));
                contentHeight = Mathf.Max(contentHeight, (rowIndex + 1) * (imagePrefab.GetComponent<RectTransform>().sizeDelta.y + spacing));
                contentPanel.sizeDelta = new Vector2(contentWidth, contentHeight);

                Button button = imageObject.GetComponent<Button>();
                if (button == null)
                    button = imageObject.AddComponent<Button>();

                button.onClick.AddListener(() => OnImageClicked(imageUrl));
            }
            else
            {
                Debug.Log("Failed to download image: " + www.error);
            }
        }
    }

    private async Task LoadImageBatchAsync()
    {
        if (isLoadingImages)
        {
            return;
        }

        int startIndex = loadedImages;
        int endIndex = Mathf.Min(totalImages, startIndex + batchSize);

        if (startIndex >= endIndex)
        {
            return;
        }

        isLoadingImages = true;
        imageBatchUrls.Clear();

        for (int i = startIndex; i < endIndex; i++)
        {
            StringBuilder imageUrlBuilder = new StringBuilder(baseUrl);
            imageUrlBuilder.Append((i + 1).ToString());
            imageUrlBuilder.Append(".jpg");
            string imageUrl = imageUrlBuilder.ToString();

            if (!loadedImageUrls.Contains(imageUrl))
            {
                imageBatchUrls.Add(imageUrl);
            }
        }

        await LoadImageBatchTexturesAsync();
        isLoadingImages = false;
    }

    private async Task LoadImageBatchTexturesAsync()
    {
        foreach (string imageUrl in imageBatchUrls)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            var requestOperation = www.SendWebRequest();

            while (!requestOperation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                if (loadedImageUrls.Contains(imageUrl))
                {
                    continue;
                }

                GameObject imageObject = Instantiate(imagePrefab, contentPanel);
                Image imageComponent = imageObject.GetComponent<Image>();
                imageComponent.sprite = sprite;

                int imageIndex = loadedImages;
                int rowIndex = imageIndex / 2;
                int columnIndex = imageIndex % 2;

                float xPosition = (columnIndex * (imagePrefabSizeX + spacing)) - (contentWidth / 2f) + (imagePrefabSizeX / 2f);
                float yPosition = (-rowIndex * (imagePrefab.GetComponent<RectTransform>().sizeDelta.y + spacing));

                imageObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, yPosition);

                loadedImages++;
                loadedImageUrls.Add(imageUrl);

                contentWidth = Mathf.Max(contentWidth, (columnIndex + 1) * (imagePrefabSizeX + spacing));
                contentHeight = Mathf.Max(contentHeight, (rowIndex + 1) * (imagePrefab.GetComponent<RectTransform>().sizeDelta.y + spacing));
                contentPanel.sizeDelta = new Vector2(contentWidth, contentHeight);

                Button button = imageObject.GetComponent<Button>();
                if (button == null)
                    button = imageObject.AddComponent<Button>();

                button.onClick.AddListener(() => OnImageClicked(imageUrl));
            }
            else
            {
                Debug.Log("Failed to download image: " + www.error);
            }
        }
    }

    public void OnImageClicked(string imageUrl)
    {
        PlayerPrefs.SetString("ImageURL", imageUrl);
        SceneManager.LoadScene("GalleryViewer");
    }
}
