using UnityEngine;

public class BoatAnimator : MonoBehaviour
{
    public float bobHeight = 0.1f;
    public float bobSpeed = 2f;
    public float rotationAmount = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        float rot = Mathf.Sin(Time.time * bobSpeed * 0.8f) * rotationAmount;

        transform.localPosition = startPos + new Vector3(0f, y, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, rot);
    }
}
