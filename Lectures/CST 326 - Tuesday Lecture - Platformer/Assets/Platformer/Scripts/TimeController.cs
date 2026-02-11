using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public float timeLeft = 500;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        timeText.text = $"TIME\n {((int)timeLeft).ToString()}";
    }
}
