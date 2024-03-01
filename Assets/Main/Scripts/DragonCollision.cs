using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonCollision : MonoBehaviour
{
    Dragon dragon;
    public Dragon otherDragon;
    private bool merge, MergeEff;
    public Material Mat;
    [SerializeField] private ParticleSystem UnicornMergeEff;
    float rimMaxValue, rimMinValue;
    [SerializeField] private bool Unicorn;

    private void Awake()
    {
        dragon = GetComponent<Dragon>();
        Mat = Instantiate(DragonSpawner.Instance.EmissionMat);
        Mat.color = Color.black;
    }

    private void Update()
    {
        
        if (MergeEff)
        {
            Mat.DOColor(Color.white, 0.2f);

            rimMaxValue -= Time.deltaTime * 2;
            rimMinValue -= Time.deltaTime * 2;

            rimMaxValue = Mathf.Min(0, rimMaxValue);
            rimMinValue = Mathf.Min(0, rimMinValue);

            foreach (var item in dragon.drMat)
            {
                item.SetColor("_RimColor", Mat.color);
                item.SetFloat("_RimMax", rimMaxValue);
                item.SetFloat("_RimMin", rimMinValue);
            }
            foreach (var item in otherDragon.drMat)
            {
                item.SetColor("_RimColor", Mat.color);
                item.SetFloat("_RimMax", rimMaxValue);
                item.SetFloat("_RimMin", rimMinValue);
            }
            if (Mat.color == Color.white)
                MergeEff = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag != "Ground" && (collision.gameObject.tag != "Wall" || collision.gameObject.layer == 6))
        {
            dragon.mesh.material = DragonSpawner.Instance.drHighF;
        }

        if (collision.gameObject.tag == "Dragon" && collision.gameObject.GetComponent<Dragon>().id != dragon.id)
        {
            dragon.isMain = false; 
        }

        if (!merge)
        {
            otherDragon = collision.gameObject.GetComponent<Dragon>();

            if (otherDragon != null && !otherDragon.isMergeing && !dragon.isMergeing)
            {
                if (Unicorn)
                    dragon.id = otherDragon.id;

                if (dragon.id == otherDragon.id && dragon.hashId > otherDragon.hashId)
                {
                    merge = true;
                    dragon.isMergeing = true;
                    otherDragon.isMergeing = true;
                    Vector3 contactPoint = collision.contacts[0].point;
                    contactPoint.y += 0.75f;
                    dragon.drRigidbody.isKinematic = true;
                    otherDragon.drRigidbody.isKinematic = true;

                    if (dragon.id != MoveController.Instance.dragonData.Count - 1)
                    {
                        StartCoroutine(DragonSpawner.Instance.MergeDragons(contactPoint, dragon.transform.rotation, dragon, Mat));
                    }
                    StartCoroutine(MergeDragons(dragon.transform, otherDragon.transform, contactPoint, dragon.transform.rotation));
                }
            }
            
        }
    }

    IEnumerator MergeDragons(Transform dr1, Transform dr2, Vector3 contactPoint, Quaternion contactRot)
    { 
        dr1.DOMove(contactPoint, 0.2f);
        dr2.DOMove(contactPoint, 0.2f);
        dr1.DORotateQuaternion(contactRot, 0.2f);
        dr2.DORotateQuaternion(contactRot, 0.2f);
        dr1.DOScale(dr1.localScale - (dr1.localScale * 0.5f), 0.2f);
        dr2.DOScale(dr2.localScale - (dr2.localScale * 0.5f), 0.2f);
        rimMaxValue = dragon.drMat[0].GetFloat("_RimMax");
        rimMinValue = dragon.drMat[0].GetFloat("_RimMin");
        Mat.color = dragon.drMat[0].color;
        MergeEff = true;
        yield return new WaitForSeconds(0.1f);
        if (Unicorn)
        {
            AudioSource sd = Instantiate(DragonSpawner.Instance.MergeSound, contactPoint, contactRot);
            Destroy(sd.gameObject, 3f);
            sd.Play();
            ParticleSystem exp = Instantiate(UnicornMergeEff, contactPoint, contactRot);
            Destroy(exp.gameObject, 3f);
            exp.Play();
        }
        else
            DragonSpawner.Instance.MergeEf(contactPoint, contactRot);
        yield return new WaitForSeconds(0.1f);
        Destroy(dr1.transform.gameObject);
        Destroy(dr2.transform.gameObject);
    }
}
