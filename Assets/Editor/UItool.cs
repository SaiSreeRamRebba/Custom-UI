using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.PackageManager.UI;
using UnityEngine.UI;
using TMPro;

public class UItool : EditorWindow
{
    public ObjectList myList= new ObjectList(); //List to contain details of JSON file
    //GameObject cube;
    TextAsset jsonFile;
    Vector2 scroll;
    GameObject globalParentObject;

    List<GameObject> generatedObjects = new List<GameObject>();

    string uiParentName = "TheParent";

    [MenuItem("Tools/UI Customisation")] //Tool address
    public static void showWindow()
    {
        GetWindow(typeof(UItool));
    }

    private void OnGUI()
    {
        //---------------------TITLE-------------------
        GUILayout.Label("CREATE UI TEMPLATE");
        GUILayout.Space(10);
        
        //---------------------TITLE-------------------


        //----------------PROPERTIES-------------------#start
        uiParentName = EditorGUILayout.TextField("UI Parent Name", uiParentName); //Scroll View
        jsonFile = EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false) as TextAsset; //Json File
        //cube = EditorGUILayout.ObjectField("Game Object", cube, typeof(GameObject), false) as GameObject; //Object To Spawn
        //globalParentObject = EditorGUILayout.ObjectField("Parent Object", Selection.activeGameObject, typeof(GameObject), false) as GameObject; 

        scroll =  GUILayout.BeginScrollView(scroll); //#Scroll Start

        //----------------PROPERTIES-------------------#end
        
        ShowJsonArrayGUI(); //Show the JSON Data in an array format in Editor Window.
       

        GUILayout.EndScrollView(); //#Scroll end

        //----------------BUTTONS-------------------#start

        if (GUILayout.Button("Read Json")) //Reads the JSON File.
        {
            ReadJSON();
        }
        

        if (GUILayout.Button("Save Json"))  //Saves the JSON File.
        {
            SaveJSON();
        }

        if (GUILayout.Button("clear Json")) //Clears the JSON File.
        {
            ClearJSON();
        }

        if (GUILayout.Button("Generate UI")) //Generates the UI Object.
        {
            GenerateObject();
        }

        if(GUILayout.Button("Update generated object Data")) //Updates generated object data
        {
            UpdateObjectData();
        }

        if(GUILayout.Button("Destroy OBJ")) //Destroys the selected Object.
        {
            DestroyParent(globalParentObject.name);
        }

        //----------------BUTTONS-------------------#end
    }

  
   void ShowJsonArrayGUI() //Function to shoa JSON data in array format in the inspector.
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        //so.Update();
        SerializedProperty stringsProperty = so.FindProperty("myList");
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
    }

    void ReadJSON()
    {
        myList = jsonFile != null ? JsonUtility.FromJson<ObjectList>(jsonFile.text) : null;
    }
    void SaveJSON()
    {
        string json = JsonUtility.ToJson(myList);
        File.WriteAllText(AssetDatabase.GetAssetPath(jsonFile), json);
        Debug.Log("Saved JSON");
    }

    void ClearJSON()
    {
        myList = null;
    }
    void GenerateObject() //Function to Generata Object.
    {
        generatedObjects.Clear();

        if (globalParentObject == null)
        {
            globalParentObject = new GameObject();
            globalParentObject.name = "Basic UI Setup";
        }

        ObjectData[] objList = myList.StartUI; 
        for(int i=0;i<objList.Length;i++) // This loop Itertes though all the elements in the ObjList Array.
        {

            GenerateObjectOfType(objList[i],objList[i].uiType, SearchParent(globalParentObject, objList[i].parent));

        }
    }

    void GenerateObjectOfType(ObjectData obj, UIType uitype, GameObject parent)
    {
        if (uitype == UIType.Canvas)
        {
            GameObject canvas = new GameObject();
            canvas.AddComponent<Canvas>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.gameObject.name = obj.name;
            canvas.transform.SetParent(parent.transform);
            generatedObjects.Add(canvas);


        }

        if (uitype == UIType.GameObject)
        {
            GameObject newObject = new GameObject();
            newObject.name = obj.name;
            newObject.transform.SetParent(parent.transform);
            generatedObjects.Add(newObject);
        }

        if (uitype == UIType.Image)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.gameObject.name = obj.name;
            image.gameObject.transform.SetParent(parent.transform);
            image.gameObject.GetComponent<RectTransform>().anchorMin = obj.anchorMin;
            image.gameObject.GetComponent<RectTransform>().anchorMax = obj.anchorMax;
            image.gameObject.GetComponent<RectTransform>().sizeDelta = obj.sizeDelta;
            image.gameObject.GetComponent<RectTransform>().anchoredPosition = obj.anchoredPosition;

            generatedObjects.Add(image.gameObject);
        }

        if (uitype == UIType.Panel)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.gameObject.name = obj.name;
            image.gameObject.transform.SetParent(parent.transform);
            image.gameObject.GetComponent<RectTransform>().anchorMin = obj.anchorMin;
            image.gameObject.GetComponent<RectTransform>().anchorMax = obj.anchorMax;
            image.gameObject.GetComponent<RectTransform>().sizeDelta = obj.sizeDelta;
            image.gameObject.GetComponent<RectTransform>().anchoredPosition = obj.anchoredPosition;
            image.color = obj.color;
            // image.rectTransform.sizeDelta = new Vector2(0, 0);
            generatedObjects.Add(image.gameObject);

        }

        if (uitype == UIType.Text)
        {
            TextMeshProUGUI text = new GameObject().AddComponent<TextMeshProUGUI>();
            text.gameObject.gameObject.name = obj.name;
            text.gameObject.gameObject.transform.SetParent(parent.transform);

            text.gameObject.GetComponent<RectTransform>().anchorMin = obj.anchorMin;
            text.gameObject.GetComponent<RectTransform>().anchorMax = obj.anchorMax;
            text.gameObject.GetComponent<RectTransform>().sizeDelta = obj.sizeDelta;
            text.gameObject.GetComponent<RectTransform>().anchoredPosition = obj.anchoredPosition;
            text.color = obj.color;
            text.GetComponent<TextMeshProUGUI>().text = obj.name + "";
            text.GetComponent<TextMeshProUGUI>().fontSize = 30;

            generatedObjects.Add(text.gameObject);
        }
        if (uitype == UIType.Button)
        {
            GameObject button = new GameObject();
            button.AddComponent<CanvasRenderer>();
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            button.gameObject.name = obj.name;

            button.gameObject.transform.SetParent(parent.transform);
            button.gameObject.GetComponent<RectTransform>().anchorMin = obj.anchorMin;
            button.gameObject.GetComponent<RectTransform>().anchorMax = obj.anchorMax;

            button.gameObject.GetComponent<RectTransform>().anchoredPosition = obj.anchoredPosition;
            button.gameObject.GetComponent<RectTransform>().sizeDelta = obj.sizeDelta;

            Color normalCol = button.GetComponent<Button>().colors.normalColor;
            normalCol = obj.color;

            // ---------------------------BUTTON TEXT-----------------------------------#start


            GameObject buttonText = new GameObject();
            buttonText.AddComponent<TextMeshProUGUI>();
            buttonText.gameObject.AddComponent<CanvasRenderer>();
            buttonText.name = obj.name + "_ButtonText";
            buttonText.GetComponent<TextMeshProUGUI>().text = obj.name + "";
            buttonText.GetComponent<TextMeshProUGUI>().fontSize = 20;
            buttonText.GetComponent<TextMeshProUGUI>().color = Color.black;
            buttonText.transform.SetParent(button.transform);
            buttonText.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            // ---------------------------BUTTON TEXT-----------------------------------#end
            generatedObjects.Add(button);
        }

    }

    GameObject foundparent;
    GameObject SearchParent(GameObject parentObj,string parentName)  //Function to Search the object to set as parent.
    {
        GameObject godparent = parentObj;
        foreach(Transform t in parentObj.transform)
        {
            if(t.name==parentName)
            {
                foundparent= t.gameObject;
            }
            else if(t.childCount>0) //If child exists, calls the function again to search it child onjects.
            {
                SearchParent(t.gameObject, parentName);
            }
        }
        if(foundparent==null || foundparent.name !=parentName)
        {
            foundparent = parentObj;
        }

      
        
        return foundparent;
    }

    void DestroyParent(string name) //Function to Destroy the selected object with specific string name.
    {
        GameObject selected = Selection.activeGameObject;
        if (selected != null) 
        {
            if(selected.name==name)
            {
                DestroyImmediate(selected);
            }
        }
    }

   

    void UpdateObjectData()
    {
        ObjectData[] objList = myList.StartUI;
        for(int i=0;i<objList.Length;i++)
        {
            objList[i].anchoredPosition = generatedObjects[i].GetComponent<RectTransform>()? generatedObjects[i].GetComponent<RectTransform>().anchoredPosition:Vector2.zero;
            objList[i].anchorMin = generatedObjects[i].GetComponent<RectTransform>()? generatedObjects[i].GetComponent<RectTransform>().anchorMax:Vector2.zero;
            objList[i].anchorMin = generatedObjects[i].GetComponent<RectTransform>()? generatedObjects[i].GetComponent<RectTransform>().anchorMin:Vector2.zero;
            objList[i].sizeDelta = generatedObjects[i].GetComponent<RectTransform>()? generatedObjects[i].GetComponent<RectTransform>().sizeDelta:Vector2.zero;
            //objList[i].col = generatedObjects[i].GetComponent<RectTransform>()? generatedObjects[i].GetComponent<RectTransform>().sizeDelta:Vector2.zero;
            Debug.Log("UPDATED "+ generatedObjects[i]);
           
        }
        SaveJSON();
        ClearJSON();
        ReadJSON();
       
        
       
    }
}
[System.Serializable]
public class ObjectData
{
    public string name;
    public string parent;
    public UIType uiType;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector3 anchoredPosition;
    public Vector3 sizeDelta;
    public Color color = Color.white;
    
}

[System.Serializable]
public class ObjectList
{
    public ObjectData[] StartUI;
}

public enum UIType
{
    GameObject,
    Canvas,
    Panel,
    Image,
    Text,
    Button
}


