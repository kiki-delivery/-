using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환진이 가지고 있는 스크립트(5개가 각각 가지고 있는셈)
/// </summary>
public class GoliathSpawn : MonoBehaviour
{
    [Header("1타입 병사")]
    [Header("=========병사 종류=========")]
    [SerializeField]
    GoliathEnemy[] type1soliders;

    [Header("2타입 병사")]
    [SerializeField]
    GoliathEnemy[] type2soliders;

    [Header("3타입 병사")]
    [SerializeField]
    GoliathEnemy[] type3soliders;

    GoliathEnemy[] soliders;

    [Header("최소 시간")]
    [Header("=========소환 설정=========")]
    [SerializeField]
    int minTime;

    [Header("최소 시간")]
    [SerializeField]
    int maxTime;

    [Header("소환 시간")]
    [SerializeField]
    float spawnSpeed;

    /// <summary>
    /// 소환되는 병사 종류 수 지정
    /// </summary>
    public int enemyRange;

    int spawnTime;
    float currentTime;   // 생성시간, 현재 시간   

    GameObject enemy;   // 생성한애 임시로 담을 곳

    [Header("공격 대상")]
    [Header("=========소환 전에 달아줄 애들=========")]
    [SerializeField]
    Player attackTarget;

    [Header("목표로 움직일 대상")]  // 쳐다볼 대상으로하면 너무 가까이가서  
    public MoveTargetManager moveTarget;    // 스폰 매니저한테 지정 받음

    [Header("골리앗 게임 매니저")]
    [SerializeField]
    GoliathGameManager gm;

    [Header("바라볼 애")]
    [SerializeField]
    GameObject lookTarget;



    public bool spawn;  // 스폰 매니저 참조용


    private void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// 재 시작할 때 설정값 초기화
    /// </summary>
    void Init()
    {
        currentTime = 0;
        MaxTime = 3;
        spawnTime = Random.Range(minTime, maxTime);
        enemyRange = 1;
    }

    /// <summary>
    /// 어느 타입의 병사(검, 방패, 말)
    /// </summary>
    int type;

    /// <summary>
    /// 같은 타입에서도 어느 종류로 할지
    /// </summary>
    int num;


    /// <summary>
    /// 병사 소환
    /// </summary>
    void Spawn()
    {
        type = Random.Range(0, enemyRange);

        switch(type)
        {
            case 0:
                {
                    soliders = type1soliders;
                    break;
                }
            case 1:
                {
                    soliders = type2soliders;
                    break;
                }
            case 2:
                {
                    soliders = type3soliders;
                    break;
                }
        }

        num = Random.Range(0, soliders.Length);

        // 소환하면서 병사에 넣어줘야 하는 세팅들 여기서 넣어줌
        soliders[num].gm = gm;
        soliders[num].attackTarget = attackTarget;
        soliders[num].moveTarget = moveTarget;
        soliders[num].GoliathLook.lookTarget = lookTarget;


        enemy = Instantiate(soliders[num].gameObject, transform.position, Quaternion.identity, moveTarget.transform);    // 소환
        moveTarget.have = true;

        spawn = false;
    }

    private void Update()
    {
        // 소환해야 되는 상태면
        if (spawn)
        {
            // 흘러가는 시간 측정
            currentTime = currentTime + Time.deltaTime * spawnSpeed;

            if (currentTime > spawnTime) // 소환시간 되면
            {
                Spawn();
                currentTime = 0;    
                spawnTime = Random.Range(minTime, maxTime); // 소환시간 다시 정하기
            }
            
        }
    }

    /// <summary>
    /// 소환 진마다 소환 주기 랜덤하게 할 때 사용
    /// </summary>
    public int MaxTime
    {
        get
        {
            return maxTime;
        }
        set
        {
            maxTime = value;
        }
    }
}
