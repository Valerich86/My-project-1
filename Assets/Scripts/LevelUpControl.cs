
using TMPro;
using UnityEngine;

public class LevelUpControl : MonoBehaviour
{

    public TextMeshProUGUI LevelInfo;

    void Start()
    {
        
    }

    void Update()
    {
        LevelInfo.text = $"������� � {GameManager.Instance.Level}";
    }
}
