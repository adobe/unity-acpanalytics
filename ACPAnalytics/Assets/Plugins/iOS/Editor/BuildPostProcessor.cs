/*
Copyright 2020 Adobe
All Rights Reserved.
NOTICE: Adobe permits you to use, modify, and distribute this file in
accordance with the terms of the Adobe license agreement accompanying
it. If you have received this file from a source other than Adobe,
then your use, modification, or distribution of it requires the prior
written permission of Adobe. (See LICENSE-MIT for details)
*/
 
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
 
 
public class BuildPostProcessor
{
 
 
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }
        string projectPath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projectPath));
        string targetGUID = project.GetUnityFrameworkTargetGuid();
 
        AddFrameworks(project, targetGUID);
 
        File.WriteAllText(projectPath, project.WriteToString());
    }
 
    static void AddFrameworks(PBXProject project, string targetGUID)
    {
        project.AddFrameworkToProject(targetGUID, "WebKit.framework", false);
        project.AddFrameworkToProject(targetGUID, "SystemConfiguration.framework", false);
        project.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);
        project.AddFrameworkToProject(targetGUID, "UIKit.framework", false);
        project.AddFrameworkToProject(targetGUID, "libsqlite3.tbd", false);
        project.AddFrameworkToProject(targetGUID, "libz.tbd", false);
        project.AddFrameworkToProject(targetGUID, "libc++.tbd", false);
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
    }
}