using UnityEngine;

public class FlightController : MonoBehaviour



{
    public float speed = 5f;
    public float destroyX = 10f; // position where plane disappears
    public string nextSceneName;

    void Update()
    {
        // Move plane to the right
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Destroy when off screen (optional)
        if (transform.position.x > destroyX)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
