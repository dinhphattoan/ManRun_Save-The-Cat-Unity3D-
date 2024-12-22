using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanic;
using UnityEngine;
using UnityEngine.Splines;

public class CarNavigationController : MonoBehaviour
{
    [SerializeField] private List<EventNavigation> eventCars;
    [SerializeField] private float navSpeed;
    Transform playerTransform;
    public void Initialize()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        for (int i = 0; i < eventCars.Count; i++)
        {
            var tuple = GetSplinePointAndRotation(eventCars[i].SplineContainer, 0);
            eventCars[i].CarPrefab.transform.position = tuple.position;
            eventCars[i].CarPrefab.transform.rotation = tuple.rotation;
            eventCars[i].isTriggered = false;
        }
    }
    void Update()
    {
        if (GameManager.GameStart)
        {
            HandleEvent();
        }

    }
    IEnumerator Coroutine_NavEvent(EventNavigation eventNavigation)
    {
        if (eventNavigation.CarPrefab == null || eventNavigation.SplineContainer == null)
        {
            Debug.LogError("Invalid EventNavigation data");
            yield break;
        }

        GameObject carPrefab = eventNavigation.CarPrefab;
        float t = 0f;


        if (eventNavigation.SplineContainer == null)
        {
            Debug.LogError("SplineContainer missing SplineComputer component");
            yield break;
        }

        while (t < 1f)
        {
            (Vector3 position, Quaternion rotation) = GetSplinePointAndRotation(eventNavigation.SplineContainer, t);

            carPrefab.transform.position = position;
            carPrefab.transform.rotation = rotation;

            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * navSpeed);
            yield return null;
        }
        yield return null;
    }

    (Vector3 position, Quaternion rotation) GetSplinePointAndRotation(SplineContainer splineContainer, float t)
    {
        Vector3 position = splineContainer.EvaluatePosition(t);
        Vector3 tangent = splineContainer.EvaluateTangent(t);
        return (position, Quaternion.LookRotation(tangent));
    }


    void HandleEvent()
    {
        for (int i = 0; i < eventCars.Count; i++)
        {
            //Check the player z is reached
            if (!eventCars[i].isTriggered && eventCars[i].TransformEventTrigger.position.z <= playerTransform.position.z)
            {
                StartCoroutine(Coroutine_NavEvent(eventCars[i]));
                eventCars[i].isTriggered = true;
            }
        }
    }

}

[Serializable]
public class EventNavigation
{
    public GameObject CarPrefab;
    public Transform TransformEventTrigger;
    public SplineContainer SplineContainer;
    public string nameEvent;
    public bool isTriggered = false;
}

