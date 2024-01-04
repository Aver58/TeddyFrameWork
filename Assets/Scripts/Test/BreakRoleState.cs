using UnityEngine;

public class Role {
    public void ToString() {
        Debug.LogError("Role.ToString");
    }
}

public class BreakRoleState {
    private Role role;
    public void Init() {
        role = new Role();
    }

    public void TestReflection() {
        role.ToString();
    }
}