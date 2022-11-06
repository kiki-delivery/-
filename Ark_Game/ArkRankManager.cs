using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 점수 등수 표시하기
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
    /// 기존에 저장된 랭크 데이터 가져오기
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

    // 처음에 받았을 때
    // 내가 가지고 있는 5개에
    // 1~2개를 리스트에 저장후(1P, 2P)
    // 그 리스트를 내림차순 정렬
    // 정렬하면서 내려가는데 만약 내 점수랑 같은게 있으면
    // 마크 실행

    /// <summary>
    /// 내가 등수를 계산할 점수의 갯수
    /// </summary>
    int scoreCount;

    /// <summary>
    /// 점수 저장
    /// </summary>
    /// <param name="scoreList">넘어온 점수 리스트</param>
    /// <param name="same">사용할 점수가 1개인지 2개인지</param>
    public void Save(List<int> scoreList, bool same)
    {
        rankScoreList = new List<int>();    // 점수 담을 곳
        rankCheck = new bool[scoreList.Count];  // 해당 점수가 내 점수인지 확인하는 용도

        for (int i = 0; i < rankScore.Length; i++)      // 5개 다 비교할거야
        {
            rankScoreList.Add(int.Parse(rankScore[i].me.text)); // 내가 가지고 있는 5개 저장
        }

        for(int i =0; i < scoreList.Count; i++)
        {
            rankScoreList.Add(scoreList[i]);    // 내가 가져온 1~2개 추가(1P면 1개, 2P면 2개)
        }

        // 내림차순 정렬
        rankScoreList.Sort();
        rankScoreList.Reverse();


        // 만약 1P, 2P의 점수가 같다면
        if(same)
        {
            scoreCount = 1;
        }
        else
        {
            scoreCount = 2;
        }
        


        for (int i = 0; i < 5; i++)      // 상위 5개만 화면에 표시
        {
            rankScore[i].me.text = rankScoreList[i].ToString();

            PlayerPrefs.SetInt(i.ToString(), rankScoreList[i]); // 데이터 저장

            // 플레이어의 점수가 등수내에 있는지 확인
            for(int k = 0; k< scoreCount; k++) 
            {
                if (rankScoreList[i] == scoreList[k])   // 만약 등수 내에 있는데
                {
                    if (!rankCheck[k])  // 아직 표시를 안한 점수면
                    {
                        rankCheck[k] = true;    // 표시 했다고 체크하고

                        // 해당 점수 주인의 마크를 표시해라
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
