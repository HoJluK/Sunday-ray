using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private bool isTurningLeft = false;
    private bool isTurningRight = false;

    public float movementSpeed = 5f;  // Скорость движения машины
    public float rotationSpeed = 100f;  // Скорость поворота машины

    public Button moveForwardButton;
    public Button moveBackwardButton;
    public Button turnLeftButton;
    public Button turnRightButton;

    private void Start()
    {
        // Проверка, что ссылки на кнопки были успешно присвоены
        if (moveForwardButton == null || moveBackwardButton == null || turnLeftButton == null || turnRightButton == null)
        {
            Debug.LogError("One or more buttons are missing. Make sure they are assigned.");
            return;
        }

        // Привязка методов обработки событий к кнопкам
        EventTrigger forwardTrigger = moveForwardButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger backwardTrigger = moveBackwardButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger leftTrigger = turnLeftButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger rightTrigger = turnRightButton.gameObject.AddComponent<EventTrigger>();

        // Добавление событий PointerDown и PointerUp к каждому триггеру
        forwardTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerDown, OnMoveForwardButtonDown));
        forwardTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerUp, OnMoveForwardButtonUp));

        backwardTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerDown, OnMoveBackwardButtonDown));
        backwardTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerUp, OnMoveBackwardButtonUp));

        leftTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerDown, OnTurnLeftButtonDown));
        leftTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerUp, OnTurnLeftButtonUp));

        rightTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerDown, OnTurnRightButtonDown));
        rightTrigger.triggers.Add(CreateEventTriggerEntry(EventTriggerType.PointerUp, OnTurnRightButtonUp));
    }

    private EventTrigger.Entry CreateEventTriggerEntry(EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(callback);
        return entry;
    }

    private void Update()
    {
        // Далее используем состояния переменных для управления машиной

        if (isMovingForward)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }

        if (isMovingBackward)
        {
            transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        }

        if (isTurningLeft)
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        if (isTurningRight)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    // Методы обработки событий кнопок

    public void OnMoveForwardButtonDown(BaseEventData eventData)
    {
        isMovingForward = true;
    }

    public void OnMoveForwardButtonUp(BaseEventData eventData)
    {
        isMovingForward = false;
    }

    public void OnMoveBackwardButtonDown(BaseEventData eventData)
    {
        isMovingBackward = true;
    }

    public void OnMoveBackwardButtonUp(BaseEventData eventData)
    {
        isMovingBackward = false;
    }

    public void OnTurnLeftButtonDown(BaseEventData eventData)
    {
        isTurningLeft = true;
    }

    public void OnTurnLeftButtonUp(BaseEventData eventData)
    {
        isTurningLeft = false;
    }

    public void OnTurnRightButtonDown(BaseEventData eventData)
    {
        isTurningRight = true;
    }

    public void OnTurnRightButtonUp(BaseEventData eventData)
    {
        isTurningRight = false;
    }
}
