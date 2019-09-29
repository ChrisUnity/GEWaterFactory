using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

public class UIManager : SingleInstance<UIManager> {

    public void Start()
    {
        //ResourceManager.Instance.MoveBtn.onClick.AddListener(() => { CameraManager.Instance.DisableCamera(); });
        //ResourceManager.Instance.StopMoveBtn.onClick.AddListener(() => { CameraManager.Instance.EnableCamera(); });

        ResourceManager.Instance.LocalBtn.onClick.AddListener(() => {ProjectEntry.Instance.ShowWorld(); });



    }

}
