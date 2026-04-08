using UnityEngine;

public class HatSwitcher : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hatRenderer;

    [Header("Hat Sprites")]
    [SerializeField] private Sprite purpleHat;
    [SerializeField] private Sprite greenHat;
    [SerializeField] private Sprite redHat;

    public string selectedHat = "none";

    void Start()
    {
        ApplyHat();
    }

    void Update()
    {
        // Test key
        if (Input.GetKeyDown(KeyCode.H))
        {
            CycleHat();
        }

        ApplyHat();
    }

    void ApplyHat()
    {
        switch (selectedHat.ToLower())
        {
            case "none":
                hatRenderer.sprite = null;
                break;
            case "purple":
                hatRenderer.sprite = purpleHat;
                break;
            case "green":
                hatRenderer.sprite = greenHat;
                break;
            case "red":
                hatRenderer.sprite = redHat;
                break;
        }
    }

    void CycleHat()
    {
        if (selectedHat == "none") selectedHat = "purple";
        else if (selectedHat == "purple") selectedHat = "green";
        else if (selectedHat == "green") selectedHat = "red";
        else selectedHat = "none";
    }

    public void SetHat(string hatName)
    {
        selectedHat = hatName;
        ApplyHat();
    }
}