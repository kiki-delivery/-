using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¦ ���߱�
// ���� �Ǻ�
// ���׼�
// �������
// �������� �Ѿ��
// ������

public class FindGameManager : MonoBehaviour
{
    [Header("-----------�ڵ�-----------")]
    public FindStageManager playStage;

    [Header("-----------����-----------")]
    [SerializeField]
    FindEndUiManager endUI;

    [Header("ScoreManager")]
    [SerializeField]
    FindScoreManager scoreManager;

    // ���콺�� ���� �� �� ���
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
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // ���콺 ��ġ ��ġ���� ���� �߻�
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
        if(wait)    // �̸����� ���ư��� �߿� Ŭ�� �ȵǰ� �Ϸ���
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

    // ���� üũ�� ����Ʈ
    List<CardInfo> matchList = new List<CardInfo>();

    [SerializeField]
    List<CardInfo> cardList = new List<CardInfo>();

    [Header("�� �����")]
    [SerializeField]
    AudioSource audio;

    [Header("���� ȿ����")]
    [SerializeField]
    AudioClip goodSound;

    [Header("���� ȿ����")]
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

    public void Match(CardInfo card)    // ¦ ���߱�
    {
        matchList.Add(card);    // ī�� ���ϸ� ����Ʈ�� �ְ�

        if (matchList.Count == 2)    // ����Ʈ�� 2�� ������ ¦ �´��� �˻�
        {
            // �˻��ϴµ� �̰� �ϴ� ���� �ٸ� ī�� ������ �ȵǸ� ���� ��� �߰��ϱ�(��ũ�� �����̶� ��� ���ϴ°ɷ� ������)

            if (matchList[0].gameObject.name == matchList[1].gameObject.name)  // ���� ������
            {
                // �ش� ���������� ���� ���� ++
                playStage.clearCount--;

                // ī����� ������ ������ ���� �ִϸ��̼� ����
                matchList[0].CardGood();
                matchList[1].CardGood();

                // ���� ������ �� ���� ���(���� ȿ����)
                GoodSound();

                // ī�尡 ������ �ִ� ���� ���(�̸� ���)
                matchList[1].SoundPlay();

                // ����ְ� ���尡 ������ ���� ���� �����ͼ�
                if (matchList[1].sound)
                {
                    cardSoundTime = matchList[1].sound.length;
                }
                else
                {
                    cardSoundTime = 0;
                }
                

                // ī�� ����Ʈ�� ���(���߿� ������� �뵵)
                cardList.Add(matchList[0]);
                cardList.Add(matchList[1]);

                matchList = new List<CardInfo>();
                scoreManager.Score();
            }
            else // Ʋ���� ���
            {
                matchList[0].CardFail();
                matchList[1].CardFail();
                FailSound();
                matchList = new List<CardInfo>();
            }
        }

        if (playStage.clearCount == 0) // Ŭ���� �����ϱ� ����������
        {
            playStage.TimeStop();
            StartCoroutine(NextStage());    // ���� ������
        }
    }


    [Header("���׼�")]
    [SerializeField]
    GameObject reaction;


    IEnumerator NextStage()
    {
        if(playStage.nextStage) // ���� ���������� ������
        {
            // �Ʒ� �ð��� ���������� ����� �Ҹ� ���� + 0.5f
            yield return new WaitForSeconds(cardSoundTime + 1f);    // 1�� �Ŀ� ���׼�

            reaction.SetActive(true);  
            
            for(int i =0; i< cardList.Count; i++)
            {
                cardList[i].CardEnd();  // ��� ī�尡 ���� ���������� �� ���� �ִϸ��̼� ���
            }


            cardList = new List<CardInfo>();

            yield return new WaitForSeconds(1.5f);    // ���׼� 1�� �Ŀ� ���� ������

            reaction.SetActive(false);


            playStage.gameObject.SetActive(false);
            playStage.nextStage.SetActive(true);


        }
        else // ���� ���������� ���� = �̼� ������ �ߴ�
        {
            yield return new WaitForSeconds(cardSoundTime + 1f);    // 1�� �Ŀ� ���׼�
            reaction.SetActive(true);
            yield return new WaitForSeconds(2f);    // 3�� �Ŀ� ���� UI
            endUI.success = true;
            endUI.gameObject.SetActive(true);
        }
    }
}
