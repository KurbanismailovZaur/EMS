using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System;

public class RoutineHelper : MonoSingleton<RoutineHelper>
{
    private Dictionary<string, Coroutine> _wrapRoutines = new Dictionary<string, Coroutine>();

    private Dictionary<string, Coroutine> _routines = new Dictionary<string, Coroutine>();

    public void StartCoroutine(string name, IEnumerator enumerator)
    {
        if (_wrapRoutines.ContainsKey(name)) throw new ArgumentException($"Coroutine with the name \"{name}\" already started");

        _wrapRoutines.Add(name, StartCoroutine(WrapRoutine(name, enumerator)));
    }

    private IEnumerator WrapRoutine(string name, IEnumerator enumerator)
    {
        _routines.Add(name, StartCoroutine(enumerator));

        yield return _routines[name];

        StopCoroutine(name);
    }

    public new void StopCoroutine(string name)
    {
        if (!_wrapRoutines.ContainsKey(name)) throw new ArgumentException($"Coroutine with the name \"{name}\" not exist");

        StopCoroutine(_wrapRoutines[name]);
        _wrapRoutines.Remove(name);

        StopCoroutine(_routines[name]);
        _routines.Remove(name);
    }

    public void StopCoroutine(string name, float delay) => StartCoroutine(StopCoroutineWithDelay(name, delay));

    private IEnumerator StopCoroutineWithDelay(string name, float delay)
    {
        yield return new WaitForSeconds(delay);

        StopCoroutine(name);
    }

    public void StopCoroutine(string name, int skipFrames) => StartCoroutine(StopCoroutineWithWithSkipFrames(name, skipFrames));

    private IEnumerator StopCoroutineWithWithSkipFrames(string name, int skipFrames)
    {
        skipFrames = Mathf.Max(skipFrames, 0);

        while (skipFrames-- != 0) yield return null;

        StopCoroutine(name);
    }
}