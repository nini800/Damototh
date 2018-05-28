using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] float health;


    public void TakeHit(HitInfos hit)
    {
        StartCoroutine(attackCoroutine());

        health -= hit.AttackStats.damages;

        if (health <= 0)
            Die(hit);
    }

    IEnumerator attackCoroutine()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        Color oColor = mat.color;
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        mat.color = oColor;
    }

    private void Die(HitInfos hit)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform t = transform.GetChild(i);
            t.gameObject.SetActive(true);

            t.GetComponent<Rigidbody>().velocity = (((t.position - hit.HitPoint).normalized * (hit.AttackStats.knockback.magnitude * .5f) + hit.AttackObject.transform.TransformVector(hit.AttackStats.knockback) * 0.5f) * (Vector3.Distance(t.position, hit.HitPoint) > 0 ? 1.5f - Vector3.Distance(t.position, hit.HitPoint) : 0)) * 5f;

            t.SetParent(Game_NG.PhysicsHolder);
        }

        Destroy(gameObject);
    }
}
