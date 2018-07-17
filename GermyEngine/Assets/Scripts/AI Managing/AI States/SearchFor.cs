using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchFor : IState
{

    private LayerMask SearchLayer;
    private GameObject OwnerGameObject;
    private float SearchRadius;
    private string TagToSearchFor;
    public bool SearchCompleted;

    private System.Action<SearchResults> searchResultsCallback;

    public SearchFor(LayerMask searchLayer, GameObject ownerGameObject, float searchRadius, string tagToSearchFor, Action<SearchResults> searchResultsCallback)
    {
        this.SearchLayer = searchLayer;
        this.OwnerGameObject = ownerGameObject;
        this.SearchRadius = searchRadius;
        this.TagToSearchFor = tagToSearchFor;
        this.searchResultsCallback = searchResultsCallback;
    }

    public void Enter()
    {
        Debug.Log("Entered SearchFor State;");
    }

    public void Execute()
    {
        if (!SearchCompleted)
        {
            var hitObjects = Physics.OverlapSphere(this.OwnerGameObject.transform.position, SearchRadius, SearchLayer);
            var allObjectsWithRequiredTag = new List<Collider>();

            for (int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].CompareTag(this.TagToSearchFor))
                {
                    //this.navMeshAgent.SetDestination(hitObjects[i].transform.position);
                    allObjectsWithRequiredTag.Add(hitObjects[i]);
                }
            }

            var searchResults = new SearchResults(hitObjects, allObjectsWithRequiredTag);
            // this is where we should send information back
            this.searchResultsCallback(searchResults);

            this.SearchCompleted = true;
        }
    }

    public void Exit()
    {

    }
}

public class SearchResults
{
    public Collider[] AllHitObjectsInSearchRadius;

    public List<Collider> AllHitObjectsWithRequiredTag;

    public SearchResults(Collider[] allHitObjectsInSearchRadius, List<Collider> allHitObjectsWithRequiredTag)
    {
        this.AllHitObjectsInSearchRadius = allHitObjectsInSearchRadius;
        this.AllHitObjectsWithRequiredTag = allHitObjectsWithRequiredTag;

    }

}
