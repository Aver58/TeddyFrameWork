using UnityEngine;
using System.Collections;
using BT;

public class DoRun : BTAction {

	private string _destinationDataName;
	private int _destinationDataId;

	private Vector3 _destination;
	private float _tolerance = 0.01f;
	private float _speed;

	private Transform _trans;
	

	public DoRun (string destinationDataName, float speed) {
		_destinationDataName = destinationDataName;
		_speed = speed;
	}

	public override void Activate (Database database) {
		base.Activate (database);

		_destinationDataId = database.GetDataId(_destinationDataName);
		_trans = database.transform;
	}

//	protected override void Enter () {
//		database.GetComponent<Animator>().Play("Run");
//	}

	protected override BTResult Execute () {
		UpdateDestination();
		UpdateFaceDirection();

		if (CheckArrived()) {
			return BTResult.Ended;
		}
		MoveToDestination();
		return BTResult.Running;
	}

	private void UpdateDestination () {
		_destination = database.GetData<Vector3>(_destinationDataId);
	}

	private void UpdateFaceDirection () {
		Vector3 offset = _destination - _trans.position;
		if (offset.x >= 0) {
			_trans.localEulerAngles = new Vector3(0, 180, 0);
		}
		else {
			_trans.localEulerAngles = Vector3.zero;
		}
	}

	private bool CheckArrived () {
		Vector3 offset = _destination - _trans.position;

		return offset.sqrMagnitude < _tolerance * _tolerance;
	}

	private void MoveToDestination () {
		Vector3 direction = (_destination - _trans.position).normalized;
		_trans.position += direction * _speed;
	}
}
