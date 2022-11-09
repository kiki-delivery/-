using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Eagle : MonoBehaviour
{
    // 사냥 하기

    bool hunt;

    [SerializeField]
    GameObject fly;

    [SerializeField]
    GameObject pickFish;

    [SerializeField]
    GameObject pickRice;

    [SerializeField]
    GameObject Effect;

    // 여기서부터 베지어관련

    [Header("중간점")] // 곡선 포인트 
    [SerializeField]
    Transform middlePoint;

    [Header("도착점")] // 곡선 포인트
    [SerializeField]
    Transform endPoint;

    [Header("이동 스피드")]
    [SerializeField]
    float speed = 0.15f;

    Vector2[] point = new Vector2[4]; // 시작점, 중간포인트, 사냥점, 끝점

    float currentTime;

    Vector3 targetPo;   // 사냥위치, 곡선 포인트

    private void Awake()
    {
        // 아래 둘은 변하지 않음
        point[0] = transform.position;  // 곡선 시작점
        point[3] = endPoint.position;   // 곡선 끝점
    }

    private void OnEnable()
    {
        transform.position = point[0];
        Init(); // 초기화
    }

    void Init()
    {      
        currentTime = 0;

        if (transform.name.Contains("1"))   // 왼쪽 꺼 사냥할거임
        {
            FindHuntObjectByTag("GoodEat", 1);  // 타겟 정하고
            point[1] = new Vector3(targetPo.x, targetPo.y - 300);
            point[2] = middlePoint.position;
        }
        else                  // 오른쪽 사냥
        {
            FindHuntObjectByTag("GoodEat", 2);  // 타겟 정하고
            point[2] = targetPo;
            point[1] = middlePoint.position;
        }
    }

    private void Update()
    {
        if (currentTime > 1) return;    // 곡선 이동은 포인를 선으로 이어서 0~1 사이를 움직이는 개념 = 1에 끝점 가있음

        currentTime = currentTime + Time.deltaTime * speed;
        Move();

        /*
        if(Mathf.Abs(transform.position.x - targetPo.x) < 50)
        {
            huntPower.enabled = true;
        }
        */
    }

    void Move()
    {
        transform.position = new Vector2(
            BezierPoint(point[0].x, point[1].x, point[2].x, point[3].x),
            BezierPoint(point[0].y, point[1].y, point[2].y, point[3].y)
            );
    }

    float BezierPoint(float a, float b, float c, float d)   // 곡선 포인트 계산 용도, 개념 모름
    {
        return Mathf.Pow((1 - currentTime), 3) * a
            + Mathf.Pow((1 - currentTime), 2) * 3 * currentTime * b
            + Mathf.Pow(currentTime, 2) * 3 * (1 - currentTime) * c
            + Mathf.Pow(currentTime, 3) * d;
    }


    private void FindHuntObjectByTag(string tag, int num)    // 사냥감 찾기
    {
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();  // 특정 태그 오브젝트 전부 리스트로 저장


        var neareastObject = objects    // 오름차순 정렬
            .OrderBy(obj =>
            {
                return Mathf.Abs(transform.position.x - obj.transform.position.x);  // 여기 확인
            });

        if (num == 1)   // 내가 왼쪽꺼 사냥할 새면
        {
            foreach (GameObject targetObject in neareastObject)
            {
                if ((transform.position.y - targetObject.transform.position.y > 400) && targetObject.transform.position.x < 0)    // 나보다 낮게 있고 왼쪽꺼면
                {
                    targetPo = targetObject.transform.position;  // 타겟으로 삼아라
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject targetObject in neareastObject)
            {
                if ((transform.position.y - targetObject.transform.position.y > 400) && targetObject.transform.position.x > 0)    // 나보다 낮게 있고 오른쪽꺼면
                {
                    targetPo = targetObject.transform.position;  // 타겟으로 삼아라
                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        hunt = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 좋은 떡이나 물고기인데, 나보다 아래에 있고 가까우면 집어가기
        if (other.transform.name.Contains("Good") && Mathf.Abs(transform.position.y - other.transform.position.y) < 100 && Mathf.Abs(transform.position.x - other.transform.position.x) < 200) 
        {
            if (!hunt)
            {
                hunt = true;
                other.transform.GetComponent<TargetObject>().HP--;
                Instantiate(Effect, transform.position, Quaternion.identity); // 닿은 위치에 파티클 생성

                fly.SetActive(false);

                if (other.transform.name.Contains("Fish"))
                {
                    pickFish.SetActive(true);
                }
                else if (other.transform.name.Contains("Rice"))
                {
                    pickRice.SetActive(true);
                }
            }
        }

    }
    
}
