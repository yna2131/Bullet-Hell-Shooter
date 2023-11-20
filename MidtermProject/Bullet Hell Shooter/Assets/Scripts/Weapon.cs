using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 무기 타입, 데미지, 공속, 범위, 효과 변수 생성
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public SphereCollider rangeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public float timeBetweenShotsRange = 10;
    
    public void Use()
    {
        if (type == Type.Range) {
            StartCoroutine("Shot");
        }
    }

    IEnumerator Shot()
    {
        int shotsFired = 0;
        int maxShots = 1; // Adjust this value based on your preference

        while (shotsFired < maxShots)
        {
            // 1. 총알 발사
            // Bullet 프리팹을 bulletPos 위치에 bulletPos의 회전값으로 생성
            GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
            // 인스턴스화 된 총알에 속도 적용하기
            bulletRigid.velocity = bulletPos.forward * 50;

            // 2. 탄피 배출
            GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
            Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
            // 인스턴스화된 탄피에 랜덤한 힘 가하기
            Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
            caseRigid.AddForce(caseVec, ForceMode.Impulse);
            caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

            shotsFired++;

            // 발사 간격 조절
            float timeBetweenShots = (type == Type.Range) ?  timeBetweenShotsRange : 0.5f;
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
