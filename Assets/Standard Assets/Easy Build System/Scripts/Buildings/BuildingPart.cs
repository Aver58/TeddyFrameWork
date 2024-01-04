using EasyBuildSystem.Features.Runtime.Bases.Buildings;
using UnityEngine;

public class BuildingPart : MonoBehaviour {
    private BuildingSocket[] buildingSockets;
    public BuildingSocket[] GetBuildingSockets() {
        return buildingSockets;
    }

    private void Awake() {
        buildingSockets = GetComponentsInChildren<BuildingSocket>();
    }
}