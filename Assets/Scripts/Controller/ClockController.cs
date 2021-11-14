using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ClockController : MonoBehaviour
{
    // Reference to the transforms for clock handle axies
    Transform SecondsHandle, MinutesHandle, HoursHandle;
    // Start time values
    float hours, minutes, seconds;
    public float Hours
    {
        get { return hours; }
        set
        {
            hours = value % 24;
            HoursHandle.rotation =   Quaternion.Euler(Hours * -30,  0, 0);
        }
    }
    public float Minutes
    {
        get { return minutes; }
        set 
        {
            Hours += (value - minutes) / 60;
            minutes = value % 60;
            MinutesHandle.rotation = Quaternion.Euler((int)Minutes * -6, 0, 0);
        }
    }
    public float Seconds
    {
        get { return seconds; }
        set
        {
            Minutes += (value - seconds) / 60;
            seconds = value % 60;
            SecondsHandle.rotation = Quaternion.Euler((int)Seconds * -6, 0, 0);
        }
    }

    float deltaTime;
    // Start is called before the first frame update
    void Awake()
    {
        SecondsHandle = transform.Find("Seconds").transform;
        MinutesHandle = transform.Find("Minutes").transform;
        HoursHandle = transform.Find("Hours").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Seconds += Time.deltaTime;
    }
}
