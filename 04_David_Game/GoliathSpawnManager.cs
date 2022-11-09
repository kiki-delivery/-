using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


// Ư�� ������ ���� �� ������ ��ȯ�Ǵ� ������ �þ��


public class GoliathSpawnManager : MonoBehaviour
{
    int enemyRange = 1; // �� ���� ��ȯ���� ����

    [Header("��ȯ ����")]
    [SerializeField]
    GoliathSpawn[] spawnArea; // ���� ��ȯ�� ����

    [Header("Ÿ�� ����")]
    [SerializeField]
    MoveTargetManager[] moveTarget; // ���� ��ǥ�� �޷��� ��

    /// <summary>
    /// ���� ��
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





    public int EnemyRange   // �긦 �ܺο��� �����ϸ�, �� ������ �þ
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
    /// ó���� �� ���� ��ȯ����
    /// </summary>
    int startSpawnNum;

    /// <summary>
    ///  ��� ��ȯ����
    /// </summary>
    int spawnAreaNum;

    /// <summary>
    /// ��� ���� ����
    /// </summary>
    int moveTargetNum;

    private void OnEnable()
    {
        // ó���� ���� �� �������� ����
        startSpawnNum = Random.Range(1, 4);
        spawnLimit = 5;

       // ù ��ȯ
        for (int i =0; i< startSpawnNum; i++)
        {
                     
            moveTargetNum = Random.Range(0, moveTarget.Length);

            // ���� Ÿ������ ������ ���� ���� �̹� ���� �ְ� ������
            while(moveTarget[moveTargetNum].have)
            {
                moveTargetNum = Random.Range(0, moveTarget.Length); // ���� ������ �ٽ� ����(Ÿ���� ��ġ�� ����鵵 ��ħ)
            }

            spawnAreaNum = Random.Range(0, spawnArea.Length);

            // ���� ��ȯ�ϱ�� ���� ������ �̹� ��ȯ�� �غ� ���̸�
            while (spawnArea[spawnAreaNum].spawn)
            {
                spawnAreaNum = Random.Range(0, spawnArea.Length);   // ��ȯ�� �� �ٽ� ����
            }

            // ��ȯ���� ��ǥ�� �� �� ���� ��
            spawnArea[spawnAreaNum].moveTarget = moveTarget[moveTargetNum]; 
            // ��ǥ������ ���� ���� ���簡 �ִ°� üũ ��
            moveTarget[moveTargetNum].have = true;
            // ��ȯ�� ����� ++
            spawnCount++;
            // ��ȯ���� ��ȯ�� �غ��� �������� üũ
            spawnArea[spawnAreaNum].spawn = true;

        }
    }

    private void Update()
    {
        // ��ȯ���� 100���� �� �� ���� ��������.. �������� ��ȯ ���� �뵵�� �����..(spawnCount = 100..)
        // ��ȯ���� 100 �̸��� ���� ���� ��ȯ�� ��� �õ�
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
