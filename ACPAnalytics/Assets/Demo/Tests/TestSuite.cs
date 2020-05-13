/*
Copyright 2020 Adobe. All rights reserved.
This file is licensed to you under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License. You may obtain a copy
of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under
the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
OF ANY KIND, either express or implied. See the License for the specific language
governing permissions and limitations under the License.
*/

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
            InvokeButtonClick("BatchAnalyticsHits");
            InvokeButtonClick("ClearQueue");
            return AssertEqualResult("GetQueueSize", "Queue size is : 0");
        }

        [UnityTest]
        public IEnumerator GetQueueSize()
        {
            return AssertEqualResult("BatchAnalyticsHits",  "Queue size is : 3");

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
