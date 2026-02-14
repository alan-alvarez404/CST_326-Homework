using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public float timeLeft = 400;

    void Update()
    {
        timeLeft -= Time.deltaTime * 3;
        timeText.text = $"TIME\n {((int)timeLeft).ToString()}";
    }
}
