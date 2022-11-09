using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȯ���� ������ �ִ� ��ũ��Ʈ(5���� ���� ������ �ִ¼�)
/// </summary>
public class GoliathSpawn : MonoBehaviour
{
    [Header("1Ÿ�� ����")]
    [Header("=========���� ����=========")]
    [SerializeField]
    GoliathEnemy[] type1soliders;

    [Header("2Ÿ�� ����")]
    [SerializeField]
    GoliathEnemy[] type2soliders;

    [Header("3Ÿ�� ����")]
    [SerializeField]
    GoliathEnemy[] type3soliders;

    GoliathEnemy[] soliders;

    [Header("�ּ� �ð�")]
    [Header("=========��ȯ ����=========")]
    [SerializeField]
    int minTime;

    [Header("�ּ� �ð�")]
    [SerializeField]
    int maxTime;

    [Header("��ȯ �ð�")]
    [SerializeField]
    float spawnSpeed;

    /// <summary>
    /// ��ȯ�Ǵ� ���� ���� �� ����
    /// </summary>
    public int enemyRange;

    int spawnTime;
    float currentTime;   // �����ð�, ���� �ð�   

    GameObject enemy;   // �����Ѿ� �ӽ÷� ���� ��

    [Header("���� ���")]
    [Header("=========��ȯ ���� �޾��� �ֵ�=========")]
    [SerializeField]
    Player attackTarget;

    [Header("��ǥ�� ������ ���")]  // �Ĵٺ� ��������ϸ� �ʹ� �����̰���  
    public MoveTargetManager moveTarget;    // ���� �Ŵ������� ���� ����

    [Header("�񸮾� ���� �Ŵ���")]
    [SerializeField]
    GoliathGameManager gm;

    [Header("�ٶ� ��")]
    [SerializeField]
    GameObject lookTarget;



    public bool spawn;  // ���� �Ŵ��� ������


    private void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// �� ������ �� ������ �ʱ�ȭ
    /// </summary>
    void Init()
    {
        currentTime = 0;
        MaxTime = 3;
        spawnTime = Random.Range(minTime, maxTime);
        enemyRange = 1;
    }

    /// <summary>
    /// ��� Ÿ���� ����(��, ����, ��)
    /// </summary>
    int type;

    /// <summary>
    /// ���� Ÿ�Կ����� ��� ������ ����
    /// </summary>
    int num;


    /// <summary>
    /// ���� ��ȯ
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

        // ��ȯ�ϸ鼭 ���翡 �־���� �ϴ� ���õ� ���⼭ �־���
        soliders[num].gm = gm;
        soliders[num].attackTarget = attackTarget;
        soliders[num].moveTarget = moveTarget;
        soliders[num].GoliathLook.lookTarget = lookTarget;


        enemy = Instantiate(soliders[num].gameObject, transform.position, Quaternion.identity, moveTarget.transform);    // ��ȯ
        moveTarget.have = true;

        spawn = false;
    }

    private void Update()
    {
        // ��ȯ�ؾ� �Ǵ� ���¸�
        if (spawn)
        {
            // �귯���� �ð� ����
            currentTime = currentTime + Time.deltaTime * spawnSpeed;

            if (currentTime > spawnTime) // ��ȯ�ð� �Ǹ�
            {
                Spawn();
                currentTime = 0;    
                spawnTime = Random.Range(minTime, maxTime); // ��ȯ�ð� �ٽ� ���ϱ�
            }
            
        }
    }

    /// <summary>
    /// ��ȯ ������ ��ȯ �ֱ� �����ϰ� �� �� ���
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
