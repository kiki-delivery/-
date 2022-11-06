using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ���� ��� ǥ���ϱ�
/// </summary>
public class ArkRankManager : MonoBehaviour
{
    [SerializeField]
    ArkRankScore[] rankScore;

    [SerializeField]
    bool init;

    private void Awake()
    {
        if (!init)
        {
            Load(); 
        }
        else
        {
            ResetRank();
            Load();
        }
    }

    void ResetRank()
    {
        for (int i = 0; i < rankScore.Length; i++)
        {
            num = i.ToString();
            PlayerPrefs.SetInt(num, 0);
            if (PlayerPrefs.HasKey(num))
            {
                rankScore[i].me.text = PlayerPrefs.GetInt(num).ToString();
            }
        }
    }


    string num;

    /// <summary>
    /// ������ ����� ��ũ ������ ��������
    /// </summary>
    public void Load()
    {       
        for (int i =0; i<rankScore.Length; i++)
        {
            num = i.ToString();
            if (PlayerPrefs.HasKey(num))
            {
                rankScore[i].me.text = PlayerPrefs.GetInt(num).ToString();
            }
        }
    }

    bool[] rankCheck;

    List<int> rankScoreList = new List<int>();

    // ó���� �޾��� ��
    // ���� ������ �ִ� 5����
    // 1~2���� ����Ʈ�� ������(1P, 2P)
    // �� ����Ʈ�� �������� ����
    // �����ϸ鼭 �������µ� ���� �� ������ ������ ������
    // ��ũ ����

    /// <summary>
    /// ���� ����� ����� ������ ����
    /// </summary>
    int scoreCount;

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="scoreList">�Ѿ�� ���� ����Ʈ</param>
    /// <param name="same">����� ������ 1������ 2������</param>
    public void Save(List<int> scoreList, bool same)
    {
        rankScoreList = new List<int>();    // ���� ���� ��
        rankCheck = new bool[scoreList.Count];  // �ش� ������ �� �������� Ȯ���ϴ� �뵵

        for (int i = 0; i < rankScore.Length; i++)      // 5�� �� ���Ұž�
        {
            rankScoreList.Add(int.Parse(rankScore[i].me.text)); // ���� ������ �ִ� 5�� ����
        }

        for(int i =0; i < scoreList.Count; i++)
        {
            rankScoreList.Add(scoreList[i]);    // ���� ������ 1~2�� �߰�(1P�� 1��, 2P�� 2��)
        }

        // �������� ����
        rankScoreList.Sort();
        rankScoreList.Reverse();


        // ���� 1P, 2P�� ������ ���ٸ�
        if(same)
        {
            scoreCount = 1;
        }
        else
        {
            scoreCount = 2;
        }
        


        for (int i = 0; i < 5; i++)      // ���� 5���� ȭ�鿡 ǥ��
        {
            rankScore[i].me.text = rankScoreList[i].ToString();

            PlayerPrefs.SetInt(i.ToString(), rankScoreList[i]); // ������ ����

            // �÷��̾��� ������ ������� �ִ��� Ȯ��
            for(int k = 0; k< scoreCount; k++) 
            {
                if (rankScoreList[i] == scoreList[k])   // ���� ��� ���� �ִµ�
                {
                    if (!rankCheck[k])  // ���� ǥ�ø� ���� ������
                    {
                        rankCheck[k] = true;    // ǥ�� �ߴٰ� üũ�ϰ�

                        // �ش� ���� ������ ��ũ�� ǥ���ض�
                        if (same && scoreList.Count == 2)
                        {
                            rankScore[i].PlayerMark(2);
                        }
                        else
                        {
                            rankScore[i].PlayerMark(k);
                        }
                        break;
                    }
                }
            }
        }

    }

    


}
