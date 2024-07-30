using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject enemyPrefab;

    private EntityManager entityManager;
    private BlobAssetStore _blobAssetStore;

    public Entity enemyEntity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
