using UnityEngine;
using System.Collections;
using BT;



public abstract class FindTargetDestination : BTAction {
	protected string _targetName;
	protected string _destinationDataName;
	protected int _destinationDataId;

	protected Transform _trans;

	public FindTargetDestination (string targetName, string destinationDataName, BTPrecondition precondition = null) : base (precondition) {
		_targetName = targetName;
		_destinationDataName = destinationDataName;
	}

	public override void Activate (Database database) {
		base.Activate (database);

		_trans = database.transform;
		_destinationDataId = database.GetDataId(_destinationDataName);
	}

	protected Vector3 GetToTargetOffset () {
		GameObject targetGo = GameObject.Find(_targetName) as GameObject;
		if (targetGo == null) {
			return Vector3.zero;
		}
		
		return targetGo.transform.position - _trans.position;
	}

	protected bool checkTarget () {
		return GameObject.Find(_targetName) as GameObject != null;
	}
}


public class FindEscapeDestination : FindTargetDestination {
	private float _safeDistance;
	
	public FindEscapeDestination (string targetName, string destinationDataName, float safeDistance, BTPrecondition precondition = null) : base (targetName, destinationDataName, precondition) {
		_safeDistance = safeDistance;
	}
	
	protected override BTResult Execute () {
		if (!checkTarget()) {
			return BTResult.Ended;
		}
		
		Vector3 offset = GetToTargetOffset();
		
		if (offset.sqrMagnitude <= _safeDistance * _safeDistance) {
			Vector3 direction = -offset.normalized;
			Vector3 destination = _safeDistance * direction * Random.Range(1.2f, 1.3f);
			database.SetData<Vector3>(_destinationDataId, destination);
			return BTResult.Running;
		}

		return BTResult.Ended;
	}
}


public class FindToTargetDestination : FindTargetDestination {
	private float _minDistance;

	public FindToTargetDestination (string targetName, string destinationDataName, float minDistance, BTPrecondition precondition = null) : base (targetName, destinationDataName, precondition) {
		_minDistance = minDistance;
	}

	protected override BTResult Execute () {
		if (!checkTarget()) {
			return BTResult.Ended;
		}
		
		Vector3 offset = GetToTargetOffset();

		if (offset.sqrMagnitude >= _minDistance * _minDistance) {
			Vector3 direction = offset.normalized;
			Vector3 destination = _trans.position + offset - _minDistance * direction;
			database.SetData<Vector3>(_destinationDataId, destination);
			return BTResult.Running;
		}
		return BTResult.Ended;
	}
}