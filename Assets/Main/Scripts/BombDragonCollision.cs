using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class BombDragonCollision : MonoBehaviour
{
    Dragon dragon;
    public Dragon otherDragon;
    [SerializeField] private ParticleSystem Explosion;
    [SerializeField] private AudioSource ExpSound;
    private void Awake()
    {
        dragon = GetComponent<Dragon>();
    }

    private void OnCollisionEnter(Collision collision)
    {
         otherDragon = collision.gameObject.GetComponent<Dragon>();
        if(collision.gameObject.layer == 6)
        {
            Vector3 contactPoint = transform.position;
            contactPoint.y += 0.75f;
            boom(contactPoint);
            Shake();
            Destroy(transform.gameObject);
        }

         if (otherDragon != null)
         {
            Vector3 contactPoint = transform.position;
            contactPoint.y += 0.75f;
            boom(contactPoint);
            Shake();
            for (int i = 0; i < collision.contactCount; i++)
            {
                Destroy( collision.contacts[i].otherCollider.gameObject);
            }
             Destroy(transform.gameObject);
         }        
    }
    
    void Shake()
    {
        for (int i = 0; i < DragonIndicator.Instance.dr.Count; i++)
        {
            DragonIndicator.Instance.dr[i].enabled = false;
        }
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        Camera.main.transform.DOShakePosition(0.2f,0.35f,15,0,false,false).OnComplete(() => Enable());
    }

    private void Enable()
    {
        for (int i = 0; i < DragonIndicator.Instance.dr.Count; i++)
        {
            DragonIndicator.Instance.dr[i].enabled = true;
        }
    }

    void boom(Vector3 pos)
    {
        AudioSource exp = Instantiate(ExpSound, transform.position, transform.rotation);
        Destroy(exp.gameObject, 3f);
        exp.Play();
        ParticleSystem sd = Instantiate(Explosion, pos, Explosion.transform.rotation);
        sd.Play();
        Destroy(sd, 1f);
    }
}