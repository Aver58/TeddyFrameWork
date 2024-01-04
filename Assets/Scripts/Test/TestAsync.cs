using System;
using System.Threading.Tasks;
using UnityEngine;

public class TestAsync : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Test();
    }

    public async void Test() {
        await Task.Run(() => {
            try {
                throw new Exception("111");
            } catch (Exception e) {
                Debug.LogError(e.Message);
                throw;
            }
        });

        Debug.LogError("222");
    }
}
