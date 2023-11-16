using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    // 총알 프리팹
    public GameObject BulletPrefab;
    // 플레이어 오브젝트
    public GameObject Shooter;
    // 각 발사 패턴 간의 대기 시간
    private float step = 2.0f;
    // 한 번에 발사되는 총알 라인의 수
    private int lines = 20;
    // 발사될 총알의 초기 각도
    private float angle = 0.0f;
    // 현재 생성된 전체 총알 수를 추적
    private int numBullets = 0;

    // UI 텍스트 업데이트를 위한 변수
    public TextMeshProUGUI bulletCounterText;

    void Start()
    {
        // 원형으로 총알을 발사하는 코루틴을 시작
        StartCoroutine(CircleShot());
    }

    // Update is called once per frame
    void Update()
    {
        // 매 프레임마다 UI 텍스트를 업데이트
        updateCounter();
    }

    // 현재 생성된 전체 총알 수를 반환
    public int getCurrentBullets()
    {
        return numBullets;
    }

    // 현재 생성된 전체 총알 수를 설정
    public void setCurrentBullets(int newBullets)
    {
        numBullets = newBullets;
    }

    // 원형으로 총알을 발사하는 코루틴
    private IEnumerator CircleShot()
    {
        angle = 360 / lines;
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < lines; i++)
            {
                Instantiate(BulletPrefab, Shooter.transform.position, Quaternion.identity).GetComponent<Bullet>().SetAngle(angle * i);
                numBullets++;
            }
            // step 값의 3분의 1만큼 대기한다
            yield return new WaitForSeconds(step / 3);
        }

        StartCoroutine(SpinShot());
    }

    // 하트 모양으로 총알을 발사하는 코루틴
private IEnumerator HeartShot()
{
    angle = 360 / lines;
    for (int j = 0; j < 10; j++)
    {
        for (int i = 0; i < lines; i++)
        {
            // Calculate the angle in radians
            float angleRad = Mathf.Deg2Rad * (angle * i);

            // Calculate the heart shape using parametric equations
            float x = 16 * Mathf.Pow(Mathf.Sin(angleRad), 3);
            float y = 0; // Centered on the y-axis
            float z = 13 * Mathf.Cos(angleRad) - 5 * Mathf.Cos(2 * angleRad) - 2 * Mathf.Cos(3 * angleRad) - Mathf.Cos(4 * angleRad);

            // Instantiate bullets with the calculated position
            Instantiate(BulletPrefab, new Vector3(x, y, z) + Shooter.transform.position, Quaternion.identity)
                .GetComponent<Bullet>().SetAngle(angle * i);

            numBullets++;
        }
        yield return new WaitForSeconds(step / 3);
    }

    StartCoroutine(CircleShot());
}

    // 스핀 공격을 발사하는 코루틴
    private IEnumerator SpinShot()
    {

        while (true)
        {
            // 360도 회전하면서 총알을 발사
            for (int i = 0; i < 360;)
            {
                // 4방향으로 각도를 90도씩 더해가며 총알을 발사
                // Instantiate()는 생성된 오브젝트를 반환하므로, GetComponent()를 통해 Bullet 스크립트를 가져온다
                Instantiate(BulletPrefab, Shooter.transform.position, Quaternion.identity).GetComponent<Bullet>().SetAngle(i);
                Instantiate(BulletPrefab, Shooter.transform.position, Quaternion.identity).GetComponent<Bullet>().SetAngle(i + 90);
                Instantiate(BulletPrefab, Shooter.transform.position, Quaternion.identity).GetComponent<Bullet>().SetAngle(i + 180);
                Instantiate(BulletPrefab, Shooter.transform.position, Quaternion.identity).GetComponent<Bullet>().SetAngle(i + 270);
                i += 5;
                numBullets += 4;
                yield return new WaitForSeconds(step / 10);
            }
            yield return new WaitForSeconds(step / 10);
        }

        StartCoroutine(HeartShot());
    }

    // UI 텍스트를 업데이트하는 함수
    private void updateCounter()
    {
        if (numBullets > 0)
        {
            bulletCounterText.text = "Bullets: " + numBullets.ToString();
        }
        else
        {
            bulletCounterText.text = "Bullets: 0";
        }
    }
}