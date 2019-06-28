using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace XEditor
{
    public class AssembliesBuildMenu
    {
        [MenuItem("XFramework/Scripts Builder/Build Assembly Sync")]
        public static void BuildAssemblySync()
        {
            BuildAssembly(true);
        }

        static bool Exists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }                
            return Directory.Exists(path);
        }

        public static void TraverseFiles(string dir, Action<string> callBack, bool isTraverse = false)
        {
            if (!Exists(dir) || callBack == null)
                return;
            DirectoryInfo folder = new DirectoryInfo(dir);
            FileSystemInfo[] files = folder.GetFileSystemInfos();
            for (int i = 0, length = files.Length; i < length; i++)
            {
                var file = files[i];
                if (file is DirectoryInfo)
                {
                    if (isTraverse)
                        TraverseFiles(file.FullName, callBack, isTraverse);
                }
                else
                {
                    callBack(file.FullName.Replace('\\', '/').ToLower());
                }
            }
        }

        static void BuildAssembly(bool wait)
        {
            List<string> shareFiles = new List<string>();
            List<string> onlyEditorFiles = new List<string>();

            TraverseFiles("Assets/Assemblies/Share", (string fullPath) =>
            {
                Debug.Log(fullPath);

                if (Path.GetExtension(fullPath) == ".cs")
                {
                   shareFiles.Add(fullPath);
                }
            }, true);

            TraverseFiles("Assets/Assemblies/OnlyEditor", (string fullPath) =>
            {
                if (Path.GetExtension(fullPath) == ".cs")
                {
                    onlyEditorFiles.Add(fullPath);
                }
            }, true);


            Directory.CreateDirectory("Temp/MyAssembly");

            string[] outputAssemblyList = { 
                "Temp/MyAssembly/Share.dll",
                "Temp/MyAssembly/OnlyEditor.dll",
            };

            string[] assemblyProjectPathList = {
                "../Editor/Assets/Plugins/Share.dll",
                "../Editor/Assets/Plugins/OnlyEditor.dll",
            };

            string[][] filesList = {
                shareFiles.ToArray(),
                onlyEditorFiles.ToArray(),
            };

            bool[] editorFlags = { true, true, true, true }; 

            for (int i = 0; i < outputAssemblyList.Length; i++)
            {
                string outputAssembly = outputAssemblyList[i];
                string assemblyProjectPath = assemblyProjectPathList[i];
                string[] files = filesList[i];
                if (files.Length == 0)
                {
                    //AssetDatabase.DeleteAsset(assemblyProjectPath);
                    File.Delete(assemblyProjectPath);
                    continue;
                }

                bool editorFlag = editorFlags[i];

                var assemblyBuilder = new AssemblyBuilder(outputAssembly, files.ToArray());

                // Exclude a reference to the copy of the assembly in the Assets folder, if any.
                assemblyBuilder.excludeReferences = new string[] { assemblyProjectPath };

                if (editorFlag)
                {
                    assemblyBuilder.flags = AssemblyBuilderFlags.EditorAssembly;
                }

                // Called on main thread
                assemblyBuilder.buildStarted += delegate (string assemblyPath)
                {
                    Debug.LogFormat("Assembly build started for {0}", assemblyPath);
                };

                // Called on main thread
                assemblyBuilder.buildFinished += delegate (string assemblyPath, CompilerMessage[] compilerMessages)
                {
                    foreach (var v in compilerMessages)
                    {
                        if (v.type == CompilerMessageType.Error)
                            Debug.LogError(v.message);
                        else
                            Debug.LogWarning(v.message);
                    }

                    var errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
                    var warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

                    Debug.LogFormat("Assembly build finished for {0}", assemblyPath);
                    Debug.LogFormat("Warnings: {0} - Errors: {0}", errorCount, warningCount);

                    if (errorCount == 0)
                    {
                        File.Copy(outputAssembly, assemblyProjectPath, true);
                        AssetDatabase.ImportAsset(assemblyProjectPath);
                    }
                };

                // Start build of assembly
                if (!assemblyBuilder.Build())
                {
                    Debug.LogErrorFormat("Failed to start build of assembly {0}!", assemblyBuilder.assemblyPath);
                    return;
                }

                if (wait)
                {
                    while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
                        System.Threading.Thread.Sleep(10);
                }
            }
        }
    }
}