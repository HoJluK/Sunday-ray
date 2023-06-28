using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyRotation : MonoBehaviour
{

    public float rotationSpeed = 150f;
    public Vector3 speedRot;
    public Color[] colors;

    private int currentColorIndex = 0;
    private Renderer coinRenderer;

    private void Start()
    {
        coinRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        // Зацикленное вращение монетки
        //this.transform.rotation *= Quaternion.Euler(speedRot);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Проверка на тап по экрану
        if (Input.GetMouseButtonDown(0))
        {
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        // Изменение цвета монетки
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        coinRenderer.material.color = colors[currentColorIndex];
    }
}
