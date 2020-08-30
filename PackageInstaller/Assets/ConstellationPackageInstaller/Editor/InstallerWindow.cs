using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

[InitializeOnLoad]
public class InstallerWindow : EditorWindow
{
    static AddRequest request;
    static bool hasRequested;
    static bool failure;
    [SerializeField]
    static InstallerWindow window;

    static InstallerWindow()
    {
        EditorApplication.update += Update;
    }

    // You cannot open a window on initialize so waiting for update.
    static void Update()
    {
        Initialize();
        EditorApplication.update -= Update;
    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Constellation Installer")]
    static void Initialize()
    {
        // Get existing open window or if none, make a new one:
        window = (InstallerWindow)EditorWindow.GetWindow(typeof(InstallerWindow));
        window.Show();
        window.maxSize = new Vector2(400, 350);
        window.minSize = new Vector2(400, 350);
    }

    void OnGUI()
    {
        var yellow = new Color(0.89f, 0.75f, 0.20f);
        var secondaryYellow = new Color(0.89f, 0.83f, 0.46f);
        var titleFontStyle = new GUIStyle();

        titleFontStyle.normal.textColor = yellow;
        titleFontStyle.fontStyle = FontStyle.Bold;
        titleFontStyle.alignment = TextAnchor.MiddleCenter;
        titleFontStyle.fontSize = 30;

        var labelFontStyle = new GUIStyle();
        labelFontStyle.normal.textColor = yellow;
        labelFontStyle.fontStyle = FontStyle.Bold;

        var background = AssetDatabase.LoadAssetAtPath("Assets/ConstellationPackageInstaller/Editor/ConstellationLogo.png", typeof(Texture2D)) as Texture2D;
        GUI.DrawTexture(new Rect(0, 0, 400, 400), background);

        if (!hasRequested)
        {
            GUILayout.Label("Constellation Setup", titleFontStyle);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);

            GUILayout.Label("Usefull links", labelFontStyle);
            GUI.color = secondaryYellow;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Web site", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://www.geek-zebra.com/home");
            }

            if (GUILayout.Button("Github", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://github.com/ConstellationLanguage/Constellation");
            }

            if (GUILayout.Button("Discord", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://discord.gg/Cx2k7We");
            }

            GUILayout.EndHorizontal();
            GUI.color = Color.white;
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
            GUILayout.Label("Requirements:", labelFontStyle);
            GUI.color = secondaryYellow;
            if (GUILayout.Button("Git", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://git-scm.com/");
            }

            GUILayout.FlexibleSpace();
            GUI.color = yellow;
            if (GUILayout.Button("Install", GUILayout.Height(EditorGUIUtility.singleLineHeight * 4)))
            {
                request = Client.Add("https://github.com/ConstellationLanguage/Constellation.git?path=/ConstellationPackages/ConstellationPackageManager#ConstellationPackageInstaller");
                EditorApplication.update += Progress;
                hasRequested = true;
            }
            GUI.color = Color.white;
        }
        else if(failure)
        {
            GUILayout.Label("Could not install", titleFontStyle);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
            GUILayout.Label("Setup Git before installing", labelFontStyle);
            GUI.color = secondaryYellow;

            if (GUILayout.Button("Git", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://git-scm.com/");
            }

            GUILayout.FlexibleSpace();
            GUI.color = yellow;
            if (GUILayout.Button("Install", GUILayout.Height(EditorGUIUtility.singleLineHeight * 4)))
            {
                request = Client.Add("https://github.com/ConstellationLanguage/Constellation.git?path=/ConstellationPackages/ConstellationPackageManager#ConstellationPackageInstalle");
                EditorApplication.update += Progress;
                hasRequested = true;
            }
        }
        else
        {
            GUILayout.Label("Processing", titleFontStyle);
        }
    }

    static void Progress()
    {
        if (request.IsCompleted)
        {
            if (request.Status == StatusCode.Success)
            {
                EditorUtility.DisplayDialog("Success!", "Package installer will close and uninstall itslef. To begin with constellation open the constellation installer to install your packages", "Continue");
                FileUtil.DeleteFileOrDirectory(Application.dataPath + "/ConstellationPackageInstaller");
                FileUtil.DeleteFileOrDirectory(Application.dataPath + "/ConstellationPackageInstaller.meta");
                window.Close();
            }
            else if (request.Status >= StatusCode.Failure)
            {
                failure = true;
                Debug.Log(request.Error.message);
            }

            EditorApplication.update -= Progress;
        }
    }
}
