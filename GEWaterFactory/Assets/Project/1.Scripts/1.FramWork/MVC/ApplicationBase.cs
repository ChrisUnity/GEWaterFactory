using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MVC
{
    public abstract class ApplicationBase<T> : Singleton<T>
    where T : MonoBehaviour
    {
        //注册控制器
        protected void RegisterController(string eventName, Type controllerType)
        {
            MVC.RegisterController(eventName, controllerType);
        }

        protected void RegisterView(View v)
        {
            MVC.RegisterView(v);
        }

        protected void SendEvent(string eventName, object data = null)
        {
            MVC.SendEvent(eventName, data);
        }
    }
}
