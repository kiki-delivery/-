using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 짝 맞추기
// 정답 판별
// 리액션
// 도장찍기
// 스테이지 넘어가기
// 뒤집기

public class FindGameManager : MonoBehaviour
{
    [Header("-----------자동-----------")]
    public FindStageManager playStage;

    [Header("-----------수동-----------")]
    [SerializeField]
    FindEndUiManager endUI;

    [Header("ScoreManager")]
    [SerializeField]
    FindScoreManager scoreManager;

    // 마우스로 실험 할 때 사용
    Ray ray;
    RaycastHit hitinfo;

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {            
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // 마우스 터치 위치에서 레이 발사
            if (Physics.Raycast(ray, out hitinfo))
            {
                if (hitinfo.transform.name.Contains("Card"))
                {
                    Touch(hitinfo.transform);
                    
                }
            }
        }
    }

    public bool wait;

    public void Touch(Transform transform)
    {
        if(wait)    // 미리보기 돌아가는 중에 클릭 안되게 하려고
        {
            return;
        }
        transform.GetComponent<CardInfo>().CardClick();
        
    }

    public void Init()
    {
        matchList = new List<CardInfo>();
        cardList = new List<CardInfo>();
    }

    // 정답 체크용 리스트
    List<CardInfo> matchList = new List<CardInfo>();

    [SerializeField]
    List<CardInfo> cardList = new List<CardInfo>();

    [Header("내 오디오")]
    [SerializeField]
    AudioSource audio;

    [Header("정답 효과음")]
    [SerializeField]
    AudioClip goodSound;

    [Header("오답 효과음")]
    [SerializeField]
    AudioClip failSound;

    void GoodSound()
    {
        StartCoroutine(GoodSoundGo());
    }

    IEnumerator GoodSoundGo()
    {
        yield return new WaitForSeconds(0.5f);
        SoundPlay(goodSound);
    }


    void FailSound()
    {
        StartCoroutine(FailSoundGo());
    }

    IEnumerator FailSoundGo()
    {
        yield return new WaitForSeconds(1f);
        SoundPlay(failSound);
    }



    void SoundPlay(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }

    float cardSoundTime;

    public void Match(CardInfo card)    // 짝 맞추기
    {
        matchList.Add(card);    // 카드 픽하면 리스트에 넣고

        if (matchList.Count == 2)    // 리스트에 2개 있으면 짝 맞는지 검사
        {
            // 검사하는데 이거 하는 동안 다른 카드 만지면 안되면 관련 기능 추가하기(스크린 게임이라 사용 안하는걸로 결정됨)

            if (matchList[0].gameObject.name == matchList[1].gameObject.name)  // 둘이 맞으면
            {
                // 해당 스테이지의 정답 갯수 ++
                playStage.clearCount--;

                // 카드들은 정답을 맞췄을 때의 애니메이션 진행
                matchList[0].CardGood();
                matchList[1].CardGood();

                // 정답 맞췄을 때 사운드 재생(축하 효과음)
                GoodSound();

                // 카드가 가지고 있는 사운드 재생(이름 재생)
                matchList[1].SoundPlay();

                // 맞춘애가 사운드가 있으면 사운드 길이 가져와서
                if (matchList[1].sound)
                {
                    cardSoundTime = matchList[1].sound.length;
                }
                else
                {
                    cardSoundTime = 0;
                }
                

                // 카드 리스트에 담기(나중에 사라지는 용도)
                cardList.Add(matchList[0]);
                cardList.Add(matchList[1]);

                matchList = new List<CardInfo>();
                scoreManager.Score();
            }
            else // 틀렸을 경우
            {
                matchList[0].CardFail();
                matchList[1].CardFail();
                FailSound();
                matchList = new List<CardInfo>();
            }
        }

        if (playStage.clearCount == 0) // 클리어 했으니까 다음맵으로
        {
            playStage.TimeStop();
            StartCoroutine(NextStage());    // 다음 맵으로
        }
    }


    [Header("리액션")]
    [SerializeField]
    GameObject reaction;


    IEnumerator NextStage()
    {
        if(playStage.nextStage) // 다음 스테이지가 있으면
        {
            // 아래 시간을 마지막으로 맞춘애 소리 길이 + 0.5f
            yield return new WaitForSeconds(cardSoundTime + 1f);    // 1초 후에 리액션

            reaction.SetActive(true);  
            
            for(int i =0; i< cardList.Count; i++)
            {
                cardList[i].CardEnd();  // 모든 카드가 다음 스테이지로 갈 때의 애니메이션 재생
            }


            cardList = new List<CardInfo>();

            yield return new WaitForSeconds(1.5f);    // 리액션 1초 후에 다음 맵으로

            reaction.SetActive(false);


            playStage.gameObject.SetActive(false);
            playStage.nextStage.SetActive(true);


        }
        else // 다음 스테이지가 없다 = 미션 성공을 했다
        {
            yield return new WaitForSeconds(cardSoundTime + 1f);    // 1초 후에 리액션
            reaction.SetActive(true);
            yield return new WaitForSeconds(2f);    // 3초 후에 성공 UI
            endUI.success = true;
            endUI.gameObject.SetActive(true);
        }
    }
}
