using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
	[SerializeField] private float slowTimeScale;

	public void SlowDownTime()
	{
		Time.timeScale = slowTimeScale;
	}

	public void SpeedUpTime()
	{
		Time.timeScale = 1f;
	}
}
