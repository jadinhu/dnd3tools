/**
* ConvertFiles.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 24/10/18 (dd/mm/yy)
* Revised on: 24/10/18 (dd/mm/yy)
*/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConvertFiles : MonoBehaviour
{
    [SerializeField]
    bool convertFilesToTxt;
    [SerializeField]
    bool putFilesInSameFolder;

    void Awake()
    {
        if (convertFilesToTxt)
            ConvertExtension(".txt");
        if (putFilesInSameFolder)
            PutFilesInSameFolder();
    }

    public string DatabasePath
    {
        get
        {
            return "assets/resources/database/";
        }
    }

    void ConvertExtension(string extension)
    {
        // get all book folders
        string[] bookFolders = Directory.GetDirectories(DatabasePath);
        int bookFoldersSize = bookFolders.Length;
        // get all files in each folder
        for (int i = 0; i < bookFoldersSize; i++)
        {
            // get files infos
            FileInfo[] fileInfoList = new DirectoryInfo(bookFolders[i]).GetFiles();
            // if there are files
            if (fileInfoList == null || fileInfoList.Length < 1)
                continue;
            // get each file in folder
            for (int j = 0; j < fileInfoList.Length; j++)
            {
                string pathFinal = bookFolders[i] + "/" + fileInfoList[j].Name;
                // avoid .meta files
                if (fileInfoList[j].Name.Contains("."))
                    continue;
                File.Move(pathFinal, pathFinal + ".txt");
            }
        }
    }

    void PutFilesInSameFolder()
    {
        // get all book folders
        string[] bookFolders = Directory.GetDirectories(DatabasePath);
        int bookFoldersSize = bookFolders.Length;
        // get all files in each folder
        for (int i = 0; i < bookFoldersSize; i++)
        {
            // get files infos
            FileInfo[] fileInfoList = new DirectoryInfo(bookFolders[i]).GetFiles();
            // if there are files
            if (fileInfoList == null || fileInfoList.Length < 1)
                continue;
            int fileListSize = fileInfoList.Length;
            // get each file in folder
            for (int j = 0; j < fileListSize; j++)
            {
                string pathFinal = bookFolders[i] + "/" + fileInfoList[j].Name;
                StartCoroutine(MoveFile(pathFinal, fileInfoList[j].Name, 1));
            }
        }
    }

    IEnumerator MoveFile(string originPath, string fileName, int repeat)
    {
        yield return null;
        try
        {
            if (repeat > 1)
                File.Move(originPath, DatabasePath + fileName.Replace(".txt", repeat + ".txt"));
            else
                File.Move(originPath, DatabasePath + fileName);
        }
        catch (System.Exception e)
        {
            if (e.ToString().Contains("ERROR_ALREADY_EXISTS"))
                StartCoroutine(MoveFile(originPath, fileName, repeat + 1));
            else
                print(e.ToString());
        }
    }
}