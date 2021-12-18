//using System;
//using UnityEngine;
//#if PLATFORM_IOS
//using UnityEngine.iOS;
//using UnityEngine.Apple.ReplayKit;

//public class Replay : MonoBehaviour
//{
//    void OnGUI()
//    {
//        if (ReplayKit.recordingAvailable)
//        {
//            if (GUI.Button(new Rect(10, 350, 500, 200), "Preview"))
//            {
//                ReplayKit.Preview();
//            }
//            if (GUI.Button(new Rect(10, 560, 500, 200), "Discard"))
//            {
//                ReplayKit.Discard();
//            }
//        }
//    }

//    public void StartRecording()
//    {
//        ReplayKit.StartRecording(false, false);
//    }

//    public void StopRecording()
//    {
//        ReplayKit.StopRecording();
//    }
//}
//#endif