using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
  #region Pool
  class Pool
  {
    public GameObject Origianl { get; private set; }
    public Transform Root { get; set; }
    Stack<Poolable> _poolStack = new Stack<Poolable>();

    public void Init(GameObject orginal, int count = 5)
    {
      Origianl = orginal;
      Root = new GameObject().transform;
      Root.name = "${orginal.name}_Root";

      for (int i = 0; i < count; i++)
        Push(Create());
    }
    Poolable Create()
    {
      GameObject go = Object.Instantiate<GameObject>(Origianl);
      go.name = Origianl.name;
      return go.GetOrAddComponet<Poolable>();
    }
    public void Push(Poolable poolable)
    {
      if (poolable == null)
        return;

      poolable.transform.parent = Root;
      poolable.gameObject.SetActive(false);
      poolable.IsUsing = false;

      _poolStack.Push(poolable);
    }
    public Poolable Pop(Transform parent)
    {
      Poolable poolable;
      if (_poolStack.Count > 0)
        poolable = _poolStack.Pop();
      else
        poolable = Create();

      poolable.gameObject.SetActive(true);
      poolable.transform.parent = parent;
      poolable.IsUsing = true;

      return poolable;
    }
  }
  #endregion
  Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
  Transform _root;
  public void Init()
  {
    if (_root == null)
    {
      _root = new GameObject { name = "Pool_Root" }.transform;
      Object.DontDestroyOnLoad(_root);
    }
  }

  public void CreatePool(GameObject orginal, int count = 5)
  {
    Pool pool = new Pool();
    pool.Init(orginal, count);
    pool.Root.parent = _root;

    _pool.Add(orginal.name, pool);
  }
  // public void Push(Poolable poolable)
  // {
  //   string name = poolable.gameObject.name;
  //   if 

  //   _pool[name].Push(poolable);
  // }

  public Poolable Pop(GameObject original, Transform parent = null)
  {
    if (_pool.ContainsKey(original.name) == false)
      CreatePool(original);

    return _pool[original.name].Pop(parent);
  }

  public GameObject GetOriginal(string name)
  {
    if (_pool.ContainsKey(name) == false)
      return null;

    return _pool[name].Origianl;
  }
}