using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class Dragon : MonoBehaviour
{
    public int id;
    public int hashId;
    public bool isMain;
    public int score;
    public Rigidbody drRigidbody;
    public bool isMergeing;
    public bool isMerged;
    public Material[] drMat;
    public Material Mat;
    public bool force;
    private float time = 0.25f;
    [HideInInspector]
    public MeshCollider mesh;
    private Color StartColor;
    float rimMaxValue, rimMinValue;
    public bool check;

    private void Awake()
    {
        mesh = GetComponent<MeshCollider>();
    }

    void Start()
    {
        drRigidbody = GetComponent<Rigidbody>();
        drMat = new Material[GetComponent<MeshRenderer>().materials.Length];
        drMat = GetComponent<MeshRenderer>().materials;
        StartColor = drMat[0].GetColor("_RimColor");
    }

    private void Update()
    {
        if (isMerged)
        {
            Mat.DOColor(StartColor, 0.2f);
            rimMaxValue += Time.deltaTime * 4;
            rimMinValue += Time.deltaTime * 4;

            rimMaxValue = Mathf.Min(1f, rimMaxValue);
            rimMinValue = Mathf.Min(0.55f, rimMinValue);
            foreach (var item in drMat)
            {
                item.SetColor("_RimColor", Mat.color);
                item.SetFloat("_RimMax", rimMaxValue);
                item.SetFloat("_RimMin", rimMinValue);
            }
            if (Mat.color == StartColor)
                isMerged = false;
        }
    }

    private void FixedUpdate()
    {

        if (drRigidbody.velocity.magnitude > 5 && drRigidbody.velocity.normalized.z < -0.35f)
        {
            drRigidbody.velocity = Vector3.zero;
        }

        if (force)
        {
            time -= Time.fixedDeltaTime;

            if (time <= 0.0f)
            {
                force = false;
            }
            else
                Force();
        }
    }

    void Force()
    {
        float pushForce = 200;
        drRigidbody.AddForce(Vector3.forward * pushForce, ForceMode.Force);
    }

    public void SetMat(Material mat)
    {
        rimMinValue = 0;
        rimMaxValue = 0;
        Mat = mat;
    }
}