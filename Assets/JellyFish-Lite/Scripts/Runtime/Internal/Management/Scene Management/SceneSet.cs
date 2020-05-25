// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

#endif

namespace JellyFish.Internal.Management
{
    [CreateAssetMenu(menuName = "JellyFish/Scene Management/Scene Set", order = 40)]
    public class SceneSet : ScriptableObject
    {
        /// <summary>
        ///     The set scenes.
        /// </summary>
        public List<SceneField> SetScenes = new List<SceneField>();

        /// <summary>
        ///     Loads this scene set according to the additive parameter.
        /// </summary>
        /// <param name="additive"></param>
        public void LoadSceneSet(bool additive)
        {
            bool firstScene = !additive;

            foreach (SceneField scene in SetScenes)
                if (scene.SceneAsset || !string.IsNullOrEmpty(scene.SceneName))
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        SceneManager.LoadSceneAsync(scene, firstScene ? LoadSceneMode.Single : LoadSceneMode.Additive);
                    else
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene.SceneAsset),
                                                     firstScene ? OpenSceneMode.Single : OpenSceneMode.Additive);
#else
                    SceneManager.LoadSceneAsync(scene, firstScene ? LoadSceneMode.Single : LoadSceneMode.Additive);
#endif
                    firstScene = false;
                }
        }

        public async void LoadSceneSetAsync(bool additive, int artificialWait, Action<SceneField, int> onSceneLoaded)
        {
            bool firstScene = !additive;

            for (int i = 0, condition = SetScenes.Count; i < condition; i++)
            {
                SceneField scene = SetScenes[i];

                if (scene.SceneAsset || !string.IsNullOrEmpty(scene.SceneName))
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        SceneManager.LoadSceneAsync(scene, firstScene ? LoadSceneMode.Single : LoadSceneMode.Additive);
                    else
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene.SceneAsset),
                                                     firstScene ? OpenSceneMode.Single : OpenSceneMode.Additive);
#else
                    SceneManager.LoadSceneAsync(scene, firstScene ? LoadSceneMode.Single : LoadSceneMode.Additive);
#endif
                    firstScene = false;
                }

                onSceneLoaded.Invoke(scene, i + 1);

                await Task.Delay(artificialWait);
            }
        }

        /// <summary>
        ///     Unloads this scene set.
        /// </summary>
        public void UnloadSceneSet()
        {
            foreach (SceneField scene in SetScenes)
                if (scene.SceneAsset || !string.IsNullOrEmpty(scene.SceneName))
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        SceneManager.UnloadSceneAsync(scene);
                    else
                        EditorSceneManager
                            .CloseScene(SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(scene.SceneAsset)), true);
#else
                    SceneManager.UnloadSceneAsync(scene);
#endif
                }
        }

#if UNITY_EDITOR
        [OnOpenAsset(0)]
        public static bool OnOpenSceneSet(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);

            if (unityObject?.GetType() == typeof(SceneSet))
            {
                SceneSet targetSet = unityObject as SceneSet;

                if (targetSet.SetScenes.Count == 0) return true;

                int loadMode = EditorUtility.DisplayDialogComplex("Load Scene Set", "Select load mode.", "Full",
                                                                  "Additive (Keep Current Scenes)", "Cancel");

                if (loadMode == 0)
                    targetSet.LoadSceneSet(false);
                else if (loadMode == 1) targetSet.LoadSceneSet(true);

                return true;
            }

            return false;
        }

        [MenuItem("Assets/Scene Set/Create Scene Set", true)]
        public static bool CreateSceneSetFromSceneValidation()
        {
            return Selection.activeObject is SceneAsset;
        }

        [MenuItem("Assets/Scene Set/Create Scene Set")]
        public static void CreateSceneSetFromScene()
        {
            SceneSet newSceneSet = CreateInstance<SceneSet>();

            SceneField newSceneField = new SceneField
                                       {
                                           SceneAsset = Selection.activeObject, SceneName = Selection.activeObject.name
                                       };

            newSceneSet.SetScenes.Add(newSceneField);

            AssetDatabase.CreateAsset(newSceneSet,
                                      AssetDatabase.GetAssetPath(Selection.activeObject)
                                                   .Replace(".unity", " Scene Set.asset"));

            AssetDatabase.Refresh();

            Selection.activeObject = newSceneSet;
        }

#endif
    }
}