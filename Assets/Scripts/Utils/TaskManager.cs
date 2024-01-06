using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TaskManager.cs
///
/// This is a convenient coroutine API for Unity.
///
/// Example usage:
///   IEnumerator MyAwesomeTask()
///   {
///       while(true) {
///           // ...
///           yield return null;
////      }
///   }
///
///   IEnumerator TaskKiller(float delay, Task t)
///   {
///       yield return new WaitForSeconds(delay);
///       t.Stop();
///   }
///
///   // From anywhere
///   Task my_task = new Task(MyAwesomeTask());
///   new Task(TaskKiller(5, my_task));
///
/// The code above will schedule MyAwesomeTask() and keep it running
/// concurrently until either it terminates on its own, or 5 seconds elapses
/// and triggers the TaskKiller Task that was created.
///
/// Note that to facilitate this API's behavior, a "TaskManager" GameObject is
/// created lazily on first use of the Task API and placed in the scene root
/// with the internal TaskManager component attached. All coroutine dispatch
/// for Tasks is done through this component.
/// 
// Equivalent to StartCoroutine(SomeCoroutine())
//new Task(SomeCoroutine());

// Equivalent to the above, but keeps a handle to the running coroutine
//Task t = new Task(SomeCoroutine());

// Pause the coroutine at next yield
//t.Pause();

// Resume it
//t.Unpause();

// Terminate it
//t.Stop();

// Test if it's still running.
//if (t.Running) { }

// Test if it's paused.
//if (t.Paused) { }

// Receive notification when it terminates.
//t.Finished += delegate (bool manual) {
//    if (manual)
//        Debug.Log("t was stopped manually.");
//    else
//        Debug.Log("t completed execution normally.");
//};

class TaskManager : MonoBehaviour
{
    public class TaskState
    {
        public bool Running
        {
            get
            {
                return running;
            }
        }

        public bool Paused
        {
            get
            {
                return paused;
            }
        }

        public delegate void FinishedHandler(bool manual);
        public event FinishedHandler Finished;

        IEnumerator coroutine;
        bool running;
        bool paused;
        bool stopped;

        public TaskState(IEnumerator c)
        {
            coroutine = c;
        }

        public void Pause()
        {
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
        }

        public void Start()
        {
            running = true;
            singleton.StartCoroutine(CallWrapper());
        }

        public void Stop()
        {
            stopped = true;
            running = false;
        }

        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = coroutine;
            while (running)
            {
                if (paused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        running = false;
                    }
                }
            }

            FinishedHandler handler = Finished;
            if (handler != null)
                handler(stopped);
        }
    }

    static TaskManager singleton;

    public static TaskState CreateTask(IEnumerator coroutine)
    {
        if (singleton == null)
        {
            GameObject go = new GameObject("TaskManager");
            singleton = go.AddComponent<TaskManager>();
        }
        return new TaskState(coroutine);
    }
}
