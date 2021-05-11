using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	private Transform _orcTrans;
	private Transform _goblinTrans;

	void Update () {
		int mouseButton = -1;
		if (Input.GetMouseButtonDown(0)) {
			mouseButton = 0;
		}
		else if (Input.GetMouseButtonDown(1)) {
			mouseButton = 1;
		}

		if (mouseButton != -1) {
			Vector3 mousePosition = SHelper.GetMousePositionInWorld();

			if (mouseButton == 0) {
				_goblinTrans = _goblinTrans == null ? (GameObject.Instantiate(Resources.Load("Goblin")) as GameObject).transform : _goblinTrans;
				_goblinTrans.name = "Goblin";
				_goblinTrans.transform.position = mousePosition;
			}
			else if (mouseButton == 1) {
				_orcTrans = _orcTrans == null ? (GameObject.Instantiate(Resources.Load("Orc")) as GameObject).transform : _orcTrans;
				_orcTrans.name = "Orc";
				_orcTrans.transform.position = mousePosition;
			}
		}

		if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
			Application.LoadLevel(0);
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
