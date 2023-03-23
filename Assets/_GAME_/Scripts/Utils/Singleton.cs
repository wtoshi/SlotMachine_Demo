using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    [SerializeField] public bool dontDestroyOnLoad = false;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {

                    Debug.LogWarning("Singleton hasnt been finded");

                    GameObject gameObject = new GameObject();

                    gameObject.name = typeof(T).Name;

                    instance = gameObject.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            //AwakeSingleton();
        }
        else
        {

            Destroy(gameObject.GetComponent<T>());
        }

        //AllSingletons.Add(this.gameObject);
    }

    //protected abstract void AwakeSingleton();

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
        applicationIsQuitting = true;
    }

}