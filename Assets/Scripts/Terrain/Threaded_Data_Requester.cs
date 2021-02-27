using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Threaded_Data_Requester : MonoBehaviour
{
    static Threaded_Data_Requester Instance;
    Queue<Thread_Info> Data_Queue = new Queue<Thread_Info>();

    private void Awake()
    {
        Instance = FindObjectOfType<Threaded_Data_Requester>();
    }
    public static void Request_Data(Func<object> Generate_Data, Action<object> Callback)
    {
        ThreadStart Thread_Start = delegate
        {
            Instance.Data_Thread(Generate_Data, Callback);
        };

        new Thread(Thread_Start).Start();
    }

    void Data_Thread(Func<object> Generate_Data, Action<object> Callback)
    {
        object Data = Generate_Data();
        lock (Data_Queue)
        {
            Data_Queue.Enqueue(new Thread_Info(Callback, Data));
        }
    }

    private void Update()
    {
        if (Data_Queue.Count > 0)
        {
            for (int i = 0; i < Data_Queue.Count; i++)
            {
                Thread_Info Thread_Info = Data_Queue.Dequeue();
                Thread_Info.Callback(Thread_Info.Parameter);
            }
        }
    }

    struct Thread_Info
    {
        public readonly Action<object> Callback;
        public readonly object Parameter;

        public Thread_Info(Action<object> Callback, object Parameter)
        {
            this.Callback = Callback;
            this.Parameter = Parameter;
        }
    }
}
