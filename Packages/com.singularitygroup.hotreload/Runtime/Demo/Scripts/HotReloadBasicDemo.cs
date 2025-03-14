﻿#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload.Demo {
    class HotReloadBasicDemo : MonoBehaviour {
        
        public GameObject cube;
        public Text informationText;
        public Button openWindowButton;
        public Button openScriptButton;
        public TextAsset thisScript;
        
        void Start() {
            if(Application.isEditor) {
                openWindowButton.onClick.AddListener(Demo.I.OpenHotReloadWindow);
                openScriptButton.onClick.AddListener(() => Demo.I.OpenScriptFile(thisScript, 31, 13));
            } else {
                openWindowButton.gameObject.SetActive(false);
                openScriptButton.gameObject.SetActive(false);
                informationText.gameObject.SetActive(false);
            }
        }
        
        // Update is called once per frame
        void Update() {
            if (Demo.I.IsServerRunning()) {
                informationText.text = "Hot Reload is running";
            } else {
                informationText.text = "Hot Reload is not running";
            }
            
            // // 1. Editing functions in monobehaviours, normal classes or static classes
            // // Edit the vector to rotate the cube in the scene differently or change the speed
            // var speed = 100;
            // cube.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speed); 
            
            // // 1. Editing functions in monobehaviours, normal classes or static classes
            // // Uncomment this code to scale the cube
            // cube.transform.localScale = Mathf.Sin(Time.time) * Vector3.one;
            
            // // 1. Editing functions in monobehaviours, normal classes or static classes
            // // Uncomment this code to make the cube move from left to right and back
            // var newPos = cube.transform.position += (cube.transform.localScale.x < 0.5 ? Vector3.left : Vector3.right) * Time.deltaTime;
            // if(Mathf.Abs(newPos.x) > 10) {
            //     cube.transform.position = Vector3.zero;
            // }
        }
        
        // 2. Editing lambda methods
        static Func<int, int> addFunction = x => {
            var result = x + 10;
            Debug.Log("Add: " + result);
            // // uncomment to change the operator to multiply and log the result
            // result = x * 10;
            // Debug.Log("Multiply: " + result);
            return result;
        };
        
        // 3. Editing async/await methods
        async Task AsyncMethod() {
            // await Task.Delay(500);
            // Debug.Log("AsyncMethod");
            
            // // silicense warning
            await Task.CompletedTask;
        }
        
        // 4. Editing properties (get/set)
        public static string SomeString {
            // edit the get method
            get {
                var someStringHere = "This is some string";
                return someStringHere;
            }
        }
        
        // 5. Editing indexers (square bracket access such as dictionaries)
        class CustomDictionary : Dictionary<string, int> {
            public new int this[string key] {
                get {
                    // // uncomment to change the indexer and log a different entry based on case
                    // return base[key.ToLower()];
                    return base[key.ToUpper()];
                }
                set {
                    base[key.ToUpper()] = value;
                }
            }
        }
        CustomDictionary randomDict = new CustomDictionary {
            { "a", 4 },
            { "A", 5 },
            { "b", 9 },
            { "B", 10 },
            { "c", 14 },
            { "C", 15 },
            { "d", 19 },
            { "D", 20 }
        };
        
        // 6. Editing operators methods (explicit and implicit operators)
        public class Email {
            public string Value { get; }

            public Email(string value) {
                Value = value;
            }

            // Define implicit operator
            public static implicit operator string(Email value)
                // Uncomment to change the implicit operator
                // => value.Value + " FOO";
                => value.Value;
            
            // // Uncomment to change add an implicit operator
            // public static implicit operator byte[](Email value)
            //     => Encoding.UTF8.GetBytes(value.Value);

            // Define explicit operator
            public static explicit operator Email(string value)
                => new Email(value);
        }
        
        void LateUpdate() {
            // // 2. Editing lambda methods
            // addFunction(10);
            
            
            // // 3. Editing async/await methods
            // AsyncMethod().Forget();
            
            
            // // 4. Editing properties (get/set)
            // Debug.Log(SomeString);
            
            
            // // 5. Editing indexers (square bracket access such as dictionaries)
            // Debug.Log(randomDict["A"]);
            
            
            // // 6. Editing operators methods (explicit and implicit operators)
            Email email = new Email("example@example.com");
            // string stringEmail = email;
            // Debug.Log(stringEmail);
            
            // // Uncomment new operator in Email class + Uncomment this to add byte implicit operator
            // byte[] byteEmail = email;
            // var hexRepresentation = BitConverter.ToString(byteEmail);
            // Debug.Log(hexRepresentation);
            // Debug.Log(Encoding.UTF8.GetString(byteEmail));
            
            // // 7. Editing lambda methods with closures
            // // Uncomment to log sorted array
            // // Switch a and b to reverse the sorting
            // int[] numbers = { 5, 3, 8, 1, 9 };
            // Array.Sort(numbers, (b, a) => a.CompareTo(b));
            // Debug.Log(string.Join(", ", numbers));
            
        }
        
        // This function gets invoked every time it's patched
        [InvokeOnHotReloadLocal]
        static void OnHotReloadMe() {
            // change the string to see the method getting invoked
            Debug.Log("Hello there");
        }

        // // 8. Adding event functions
        // void OnDisable() {
        //     Debug.Log("OnDisable");
        // }
    }
}
#endif
