using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestSuite
    {
        public string extensionVersion = "";
        [UnityTest]
        public IEnumerator AnalyticsExtensionVersion()
        {
            if (Application.platform == RuntimePlatform.Android) {
                return AssertEqualResult("ExtensionVersion","Analytics extension version : 1.2.4");
            } else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return AssertEqualResult("ExtensionVersion","Analytics extension version : 2.2.3");
            } else
            {
                return null;
            }
        }

        [UnityTest]
        public IEnumerator ClearQueue()
        {
            InvokeButtonClick("ClearQueue");
            return AssertEqualResult("GetQueueSize","Queue size is : 0");
        }

        [UnityTest]
        public IEnumerator GetQueueSize()
        {
            return AssertGreaterLengthResult("GetQueueSize",  "Queue size is : ".Length);
        }

        [UnityTest]
        public IEnumerator GetVisitorIdentifier()
        {
            var setVisitorIdentiferTextGameObject = GameObject.Find("VisitorIdentifier");
            var setVisitorIdentifierText = setVisitorIdentiferTextGameObject.GetComponent<InputField>();
            setVisitorIdentifierText.text = "vid";

            InvokeButtonClick("SetVisitorIdentifier");
            return AssertEqualResult("GetVisitorIdentifier","Visitor identifier is : vid");
        }

        // Helper functions
        private IEnumerator LoadScene() {
            AsyncOperation async = SceneManager.LoadSceneAsync("Demo/DemoScene");

            while (!async.isDone)
            {
                yield return null;
            }
        }

        private void InvokeButtonClick(string gameObjName) {
            var gameObj = GameObject.Find(gameObjName);
            var button = gameObj.GetComponent<Button>();
            button.onClick.Invoke();
        }

        private string GetActualResult()
        {
            var callbackResultsGameObject = GameObject.Find("CallbackResults");
            var callbackResults = callbackResultsGameObject.GetComponent<Text>();
            return callbackResults.text;
        }

        private IEnumerator AssertEqualResult(string gameObjectName, string expectedResult) {
            yield return LoadScene();
            InvokeButtonClick(gameObjectName);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(expectedResult, GetActualResult());
        }

        private IEnumerator AssertGreaterLengthResult(string gameObjectName, int expectedLength) {
            yield return LoadScene();
            InvokeButtonClick(gameObjectName);
            yield return new WaitForSeconds(1f);
            Assert.Greater(GetActualResult().Length, expectedLength);
        }
    }
}
