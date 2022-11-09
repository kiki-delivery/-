using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


// 특정 조건이 만족 될 때마다 소환되는 종류가 늘어난다


public class GoliathSpawnManager : MonoBehaviour
{
    int enemyRange = 1; // 몇 종류 소환할지 지정

    [Header("소환 구역")]
    [SerializeField]
    GoliathSpawn[] spawnArea; // 내가 소환할 구역

    [Header("타겟 구역")]
    [SerializeField]
    MoveTargetManager[] moveTarget; // 내가 목표로 달려갈 곳

    /// <summary>
    /// 스폰 수
    /// </summary>
    public int spawnCount;

    int spawnLimit;

    public int SpawnLimit
    {
        get
        {
            return spawnLimit;
        }
        set
        {
            spawnLimit = value;
            switch(spawnLimit)
            {
                case 8:
                    {
                        for(int i =0; i< spawnArea.Length; i++)
                        {
                            spawnArea[i].MaxTime = 2;
                        }
                        break;
                    }
                case 12:
                    {
                        for(int i =0; i< spawnArea.Length; i++)
                        {
                            spawnArea[i].MaxTime = 1;
                        }
                        break;
                    }
            }
        }
    }





    public int EnemyRange   // 얘를 외부에서 조정하면, 적 종류가 늘어남
    {
        get { return enemyRange; }
        set
        {
            enemyRange = value;
            for(int i =0; i< spawnArea.Length; i++)
            {
                spawnArea[i].enemyRange = enemyRange;
            }
        }
    }

    /// <summary>
    /// 처음에 몇 마리 소환할지
    /// </summary>
    int startSpawnNum;

    /// <summary>
    ///  어디서 소환할지
    /// </summary>
    int spawnAreaNum;

    /// <summary>
    /// 어디를 향해 갈지
    /// </summary>
    int moveTargetNum;

    private void OnEnable()
    {
        // 처음에 나올 수 랜덤으로 지정
        startSpawnNum = Random.Range(1, 4);
        spawnLimit = 5;

       // 첫 소환
        for (int i =0; i< startSpawnNum; i++)
        {
                     
            moveTargetNum = Random.Range(0, moveTarget.Length);

            // 내가 타겟으로 지정한 곳을 향해 이미 오는 애가 있으면
            while(moveTarget[moveTargetNum].have)
            {
                moveTargetNum = Random.Range(0, moveTarget.Length); // 없을 때까지 다시 지정(타겟이 겹치면 병사들도 겹침)
            }

            spawnAreaNum = Random.Range(0, spawnArea.Length);

            // 내가 소환하기로 정한 곳에서 이미 소환을 준비 중이면
            while (spawnArea[spawnAreaNum].spawn)
            {
                spawnAreaNum = Random.Range(0, spawnArea.Length);   // 소환할 곳 다시 정함
            }

            // 소환진에 목표로 할 곳 전달 후
            spawnArea[spawnAreaNum].moveTarget = moveTarget[moveTargetNum]; 
            // 목표지점을 향해 가는 병사가 있는걸 체크 후
            moveTarget[moveTargetNum].have = true;
            // 소환된 병사수 ++
            spawnCount++;
            // 소환진이 소환을 준비한 상태임을 체크
            spawnArea[spawnAreaNum].spawn = true;

        }
    }

    private void Update()
    {
        // 소환수가 100으로 갈 수 없는 구조지만.. 보스전때 소환 막는 용도로 사용중..(spawnCount = 100..)
        // 소환수가 100 미만일 때는 병사 소환을 계속 시도
        if (spawnCount < 100)
        {
            
            moveTargetNum = Random.Range(0, moveTarget.Length);

            if(moveTarget[moveTargetNum].have)
            {
                return;
            }

            spawnAreaNum = Random.Range(0, spawnArea.Length);

            if (spawnArea[spawnAreaNum].spawn)
            {
                return;
            }

            spawnArea[spawnAreaNum].moveTarget = moveTarget[moveTargetNum];
            moveTarget[moveTargetNum].have = true;
            spawnCount++;
            spawnArea[spawnAreaNum].spawn = true;
        }
    }



    private void OnDisable()
    {
        for(int i =0; i< moveTarget.Length; i++)
        {
            moveTarget[i].have = false;
        }
        for (int i = 0; i < spawnArea.Length; i++)
        {
            spawnArea[i].spawn = false;
        }
    }
}
