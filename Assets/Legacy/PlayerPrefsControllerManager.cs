using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsControllerManager : MonoBehaviour
{
    public bool isLeftHand;
    void Start()
    {
       if (isLeftHand)
       {
            transform.localRotation = LoadQuaternion("Left Hand Rotation Offset");
            transform.localPosition = LoadVector3("Left Hand Position Offset");
       }
       else
       {
            transform.localRotation = LoadQuaternion("Right Hand Rotation Offset");
            transform.localPosition = LoadVector3("Right Hand Position Offset");
       }
    }

    public void SaveQuaternion(string key, Quaternion quaternion)
    {
        PlayerPrefs.SetFloat(key + "_x", quaternion.x);
        PlayerPrefs.SetFloat(key + "_y", quaternion.y);
        PlayerPrefs.SetFloat(key + "_z", quaternion.z);
        PlayerPrefs.SetFloat(key + "_w", quaternion.w);
    }

    public Quaternion LoadQuaternion(string key)
    {
        if (PlayerPrefs.HasKey(key + "_x") && PlayerPrefs.HasKey(key + "_y") &&
            PlayerPrefs.HasKey(key + "_z") && PlayerPrefs.HasKey(key + "_w"))
        {
            Quaternion quaternion = new Quaternion
            {
                x = PlayerPrefs.GetFloat(key + "_x"),
                y = PlayerPrefs.GetFloat(key + "_y"),
                z = PlayerPrefs.GetFloat(key + "_z"),
                w = PlayerPrefs.GetFloat(key + "_w")
            };
            return quaternion;
        }
        else return Quaternion.identity;
    }


    public void SaveVector3(string key, Vector3 vector)
    {
        PlayerPrefs.SetFloat(key + "_x", vector.x);
        PlayerPrefs.SetFloat(key + "_y", vector.y);
        PlayerPrefs.SetFloat(key + "_z", vector.z);
    }

    public Vector3 LoadVector3(string key)
    {
        Vector3 vector = new Vector3
        {
            x = PlayerPrefs.GetFloat(key + "_x"),
            y = PlayerPrefs.GetFloat(key + "_y"),
            z = PlayerPrefs.GetFloat(key + "_z")
        };
        return vector;
    }
}
