/**
* LoadLinks.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 06/10/18 (dd/mm/yy)
* Revised on: 06/10/18 (dd/mm/yy)
*/
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadLinks : MonoBehaviour
{
    [SerializeField]
    string spellsRootPath;
    [SerializeField]
    bool run;

    void Awake()
    {
        if (run)
            LoadFiles();
    }

    void LoadFiles()
    {
        for (int i = 0; i <= 9; i++)
        {
            List<string> links = new List<string>();
            string path = spellsRootPath + "/" + i + "/";
            DirectoryInfo info = new DirectoryInfo(path);
            if (info == null || !info.Exists)
                continue;
            FileInfo[] fileInfoList = info.GetFiles();
            if (fileInfoList == null || fileInfoList.Length < 1)
                continue;
            int counterIndex = -1;
            for (int j = 0; j < fileInfoList.Length; j++)
            {
                if (fileInfoList[j].Name.Contains(".meta"))
                    continue;
                string fileText = File.ReadAllText(path + fileInfoList[j].Name);
                if (string.IsNullOrEmpty(fileText))
                    continue;
                counterIndex++;
                // create a array with each line of the file
                string[] lines = fileText.Split("\n"[0]);
                links.Add(lines[0]);
            }
            File.WriteAllLines(spellsRootPath + "/links " + i + ".txt", links.ToArray());
        }
    }
}