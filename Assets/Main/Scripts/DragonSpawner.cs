using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class DragonSpawner : MonoBehaviour
{
    public static DragonSpawner Instance {  get; private set; }

    public Material EmissionMat;
    public ParticleSystem MergeEff;
    public AudioSource MergeSound;
    public PhysicMaterial drHighF;
    public int mergeCount;
    [SerializeField] private ParticleSystem[] comboTextParticle;



    private void Awake()
    {
        Instance = this;
    }

    public Dragon SpawnDragon(int id, Vector3 pos, Quaternion rot)
    {
        Dragon dr =  Instantiate(MoveController.Instance.dragonData[id].DragonPrefap, pos, rot, MoveController.Instance.dragonsParent).GetComponent<Dragon>();
        dr.id = MoveController.Instance.dragonData[id].id;
        dr.score = MoveController.Instance.dragonData[id].score;
        dr.hashId = MoveController.Instance.hashId;
        MoveController.Instance.hashId++;
        
        mergeCount++;

        switch (mergeCount)
        {
            case 3:
                comboTextParticle[0].transform.position = new Vector3(pos.x, pos.y, pos.z - 2);
                comboTextParticle[0].Play();
                break;

            case 4:
                comboTextParticle[1].transform.position = new Vector3(pos.x, pos.y, pos.z - 2);
                comboTextParticle[1].Play();
                break;

            case 5:
                comboTextParticle[2].transform.position = new Vector3(pos.x, pos.y, pos.z - 2);
                comboTextParticle[2].Play();
                break;

            case 6:
                comboTextParticle[3].transform.position = new Vector3(pos.x, pos.y, pos.z - 2);
                comboTextParticle[3].Play(); ;
                break;
        }

        ScoreCounter.Instance.SetScore(dr.score);
        return dr;
    }

    public IEnumerator MergeDragons(Vector3 contactPoint, Quaternion contactRot, Dragon dragon, Material Mat)
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        if (dragon.id == MoveController.Instance.currentdr && !DragonIndicator.Instance.isOn)
        {
            TouchController.Instance.TakeControl = true;
            ScrollController.Instance.CanCheck = false;
            MoveController.Instance.currentdr++;
            StartCoroutine(DragonIndicator.Instance.Next(MoveController.Instance.currentdr));
            DragonIndicator.Instance.isOn = true;
        }

        yield return new WaitForSeconds(0.19f);
        Dragon newDragon = SpawnDragon(dragon.id + 1, contactPoint, contactRot);
       
        Transform newDrT = newDragon.transform;
        Vector3 scale = newDrT.localScale;
        newDrT.localScale = newDrT.localScale - (newDrT.localScale * 0.5f);
 
        newDrT.DOScale(scale, 0.2f);
        newDragon.drRigidbody.isKinematic = false;
        Mat.color = Color.white;
        newDragon.SetMat(Mat);
        newDragon.isMerged = true;

        newDragon.force = true;

        float pushForce = 30f;
        newDragon.drRigidbody.AddForce(Vector3.up * pushForce, ForceMode.Impulse);
       
        int k = Random.Range(0, 2);
        float randomValue;
        if (k == 0)
            randomValue = Random.Range(-10f, -5f);
        else
            randomValue = Random.Range(5f, 10f);

        Vector3 randomDirection = randomValue * Vector3.one;
        newDragon.drRigidbody.AddTorque(randomDirection,ForceMode.Impulse);      

        Collider[] surroundedDragons = Physics.OverlapSphere(contactPoint, 2f);
        float explosionForce = 400;
        float explosionRadius = 1f;

        foreach (Collider coll in surroundedDragons)
        {
            if (coll.attachedRigidbody != null)
                coll.attachedRigidbody.AddExplosionForce(explosionForce, contactPoint, explosionRadius);
        }
    }
    public void MergeEf(Vector3 contactPoint, Quaternion contactRot)
    {
        ParticleSystem mergeE = Instantiate(MergeEff, contactPoint, contactRot);
        AudioSource sd = Instantiate(MergeSound, contactPoint, contactRot);
        Destroy(sd.gameObject, 3f);
        sd.Play();
        mergeE.Play();
    }
}