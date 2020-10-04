using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(TimeScaleController))]
public class BulletTimeController : MonoBehaviour
{
	[Serializable]
	public class TargetTrackingSetup
	{
		public CinemachinePathController avaliableTrack;
		public CameraDollyController avaliableDolly;
	}

	[Serializable]
	public class BulletTrackingSetup : TargetTrackingSetup
	{
		public float minDistance;
		public float maxDistance;
	}

	[SerializeField] private GameObject canvas;
	[SerializeField] private CinemachineBrain cameraBrain;
	[SerializeField] private BulletTrackingSetup[] bulletTackingSetup;
	[SerializeField] private TargetTrackingSetup[] enemyTrackingSetup;
	[SerializeField] private PlayerShootingController shootingController;
	[SerializeField] private float distanceToChangeCamera;
	[SerializeField] private float finishingCameraDuration;

	private TimeScaleController timeScaleController;
	private CinemachineSmoothPath trackInstance;
	private CameraDollyController dollyInstance;
	private Bullet activeBullet;
	private Vector3 targetPosition;
	private List<TargetTrackingSetup> clearTracks = new List<TargetTrackingSetup>();
	private bool isLastCameraActive = false;

	private void Awake()
	{
		timeScaleController = GetComponent<TimeScaleController>();
	}

	internal void StartSequence(Bullet activeBullet, Vector3 targetPosition)
	{
		ResetVariables();
		float distanceToTarget = Vector3.Distance(activeBullet.transform.position, targetPosition);
		var setupsInRange = bulletTackingSetup.Where(s => distanceToTarget > s.minDistance && distanceToTarget < s.maxDistance).ToArray();
		var selectedTrackingSetup = SelectTrackingSetup(activeBullet.transform, setupsInRange, activeBullet.transform.rotation);
		if (selectedTrackingSetup == null)
			return;
		this.activeBullet = activeBullet;
		this.targetPosition = targetPosition;

		CreateBulletPath(activeBullet.transform, selectedTrackingSetup.avaliableTrack);
		CreateDolly(selectedTrackingSetup);
		cameraBrain.gameObject.SetActive(true);
		shootingController.gameObject.SetActive(false);
		canvas.gameObject.SetActive(false);
		float speed = CalculateDollySpeed();
		dollyInstance.InitDolly(trackInstance, activeBullet.transform, speed);
	}

	private float CalculateDollySpeed()
	{
		if (trackInstance == null || activeBullet == null)
			return 0f;

		float distanceToTarget = Vector3.Distance(activeBullet.transform.position, targetPosition);
		float speed = activeBullet.GetBulletSpeed();
		float pathDistance = trackInstance.PathLength;
		return pathDistance * speed / distanceToTarget;
	}

	private void CreateDolly(TargetTrackingSetup setup)
	{
		var selectedDolly = setup.avaliableDolly;
		dollyInstance = Instantiate(selectedDolly);
	}

	private void CreateBulletPath(Transform bulletTransform, CinemachinePathController selectedPath)
	{
		trackInstance = Instantiate(selectedPath.path, bulletTransform);
		trackInstance.transform.localPosition = selectedPath.transform.position;
		trackInstance.transform.localRotation = selectedPath.transform.rotation;
	}


	private void CreateEnemyPath(Transform enemytransform, Transform bulletTransform, CinemachinePathController selectedPath)
	{
		Quaternion rotation = Quaternion.Euler(Vector3.up * bulletTransform.root.eulerAngles.y);
		trackInstance = Instantiate(selectedPath.path, enemytransform.position, rotation);
	}

	private TargetTrackingSetup SelectTrackingSetup(Transform trans, TargetTrackingSetup[] setups, Quaternion orientation)
	{
		clearTracks.Clear();
		for (int i = 0; i < setups.Length; i++)
		{
			if (CheckIfPathIsClear(setups[i].avaliableTrack, trans, orientation))
				clearTracks.Add(setups[i]);
		}
		if (clearTracks.Count == 0)
			return null;
		return clearTracks[UnityEngine.Random.Range(0, clearTracks.Count)];
	}

	private bool CheckIfPathIsClear(CinemachinePathController path, Transform trans, Quaternion orientation)
	{
		return path.CheckIfPathISClear(trans, Vector3.Distance(trans.position, targetPosition), orientation);
	}

	private void Update()
	{
		if (!activeBullet)
			return;

		if (CheckIfBulletIsNearTarget())
			ChangeCamera();
	}

	private bool CheckIfBulletIsNearTarget()
	{
		return Vector3.Distance(activeBullet.transform.position, targetPosition) < distanceToChangeCamera;
	}

	private void ChangeCamera()
	{
		if (isLastCameraActive)
			return;
		isLastCameraActive = true;
		DestroyCinemachineSetup();
		Transform hitTransform = activeBullet.GetHitEnemyTransform();
		if (hitTransform)
		{
			Quaternion rotation = Quaternion.Euler(Vector3.up * activeBullet.transform.rotation.eulerAngles.y);
			var selectedTrackingSetup = SelectTrackingSetup(hitTransform, enemyTrackingSetup, rotation);
			if (selectedTrackingSetup != null)
			{
				CreateEnemyPath(hitTransform, activeBullet.transform, selectedTrackingSetup.avaliableTrack);
				CreateDolly(selectedTrackingSetup);
				dollyInstance.InitDolly(trackInstance, hitTransform.transform);
				timeScaleController.SlowDownTime();
			}
		}
		StartCoroutine(FinishSequence());
	}

	private void DestroyCinemachineSetup()
	{
		Destroy(trackInstance.gameObject);
		Destroy(dollyInstance.gameObject);
	}

	private IEnumerator FinishSequence()
	{
		yield return new WaitForSecondsRealtime(finishingCameraDuration);
		cameraBrain.gameObject.SetActive(false);
		shootingController.gameObject.SetActive(true);
		canvas.gameObject.SetActive(true);
		timeScaleController.SpeedUpTime();
		DestroyCinemachineSetup();
		Destroy(activeBullet.gameObject);
		ResetVariables();
	}

	private void ResetVariables()
	{
		isLastCameraActive = false;
		trackInstance = null;
		dollyInstance = null;
		activeBullet = null;
		clearTracks.Clear();
		targetPosition = Vector3.zero;
	}
}