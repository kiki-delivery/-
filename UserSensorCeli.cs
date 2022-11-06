using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HKY;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UtilsModule;
using System;


// 사용자가 켈리브레이션 하는 용도

public class UserSensorCeli : MonoBehaviour
{
    [Header("참조 오브젝트 모음")]
    [SerializeField]
    URGSensorObjectDetector sensor;

    [SerializeField]
    SeonsorCeliPo seonsorPo;

    [SerializeField]
    CeliPositionTest positionTest;

    [SerializeField]
    CeliDataManager dataManager;


    // 스텝에서 쓰는 애들

    [Header("스크린 넓이")]
    [SerializeField]
    Text inputWidth;

    [Header("스크린 높이")]
    [SerializeField]
    Text inputHeight;    

        
    [Header("내가 구현한 게임의 넓이")]

    [Header("스텝 3 호모그래피 계산")]
    [SerializeField]
    float gameWidth;

    [Header("내가 구현한 게임의 높이")]
    [SerializeField]
    float gameHeight;

    [SerializeField]
    List<float> xList = new List<float>();
    [SerializeField]
    List<float> yList = new List<float>();


    [SerializeField]
    public homo[] homoList;

    [System.Serializable]
    public struct homo
    {
        [SerializeField]
        public float[] homoNum;
    }

    [SerializeField]
    Text leftUpX;
    [SerializeField]
    Text leftUpY;
    [SerializeField]
    Text rightUpX;
    [SerializeField]
    Text rightUpY;
    [SerializeField]
    Text leftDownX;
    [SerializeField]
    Text leftDownY;
    [SerializeField]
    Text rightDownX;
    [SerializeField]
    Text rightDownY;


    // 스텝 1
    // 센서사용하는 스크린 크기 입력받고 데이터 저장(게임들에 스크린 크기 알려주는 용도)
    public void Step1_ScreenSizeInput()
    {
        sensor.detectRectWidth = int.Parse(inputWidth.text);
        sensor.detectRectHeight = int.Parse(inputHeight.text); ;
    }

    float temp;

    // 스텝2 센서가 찍는 포지션 알림
    // 센서 위치가 어디든 사용 가능하도록 개선 필요
    // 지금은 사용자가 지정한 위치를 순서대로 찍어야만 켈리브레이션 가능

    public void Step2_SeonsorPoNotice()
    {
        xList = seonsorPo.xList;
        yList = seonsorPo.yList;

        leftDownX.text = xList[0].ToString();
        leftDownY.text = yList[0].ToString();
        leftUpX.text = xList[1].ToString();
        leftUpY.text = yList[1].ToString();
        rightUpX.text = xList[2].ToString();
        rightUpY.text = yList[2].ToString();
        rightDownX.text = xList[3].ToString();
        rightDownY.text = yList[3].ToString();
    }
    

    // 스텝3 호모그래피 행렬 계산
    
    public void Step3_HomoStart()
    {
        // 여기는 게임 크기
        float gameX = gameWidth / 2;
        float gameY = gameHeight;

        // 벡터 형식을 포인트 형식으로 변환 후(OpenCV 자료형인듯..)
        // 인게임 해상도 기반
        Point a1 = new Point(-gameX, 0);  // 왼아래(1)
        Point a2 = new Point(gameX, 0);   // 오른아래(2)
        Point a3 = new Point(-gameX, gameY);   // 왼위(3)
        Point a4 = new Point(gameX, gameY);    // 오른위(4)
       
        // 실제 사용하는 스크린 크기, 이렇게 하고 보니 사용자가 찍을 필요 없이 입력하도록 해도 됐을듯..
        Point b1 = new Point(float.Parse(leftDownX.text), float.Parse(leftDownY.text));  // 왼아래(1)
        Point b2 = new Point(float.Parse(rightDownX.text), float.Parse(rightDownY.text));   // 오른아래(2)
        Point b3 = new Point(float.Parse(leftUpX.text), float.Parse(leftUpY.text));   // 왼위(3)
        Point b4 = new Point(float.Parse(rightUpX.text), float.Parse(rightUpY.text));    // 오른위(4)


        // 포인트 배열로 만들어서
        Point[] arrayA = new Point[] { a1, a2, a3, a4 };
        Point[] arrayB = new Point[] { b1, b2, b3, b4 };


        // 그걸로 MAT포인트 형식으로 만들고(호모그래피 행렬 변환을 위해..)
        MatOfPoint2f pointA = new MatOfPoint2f(arrayA);
        MatOfPoint2f pointB = new MatOfPoint2f(arrayB);

        // 호모그래피 행렬 계산(3.3행렬)
        Mat homo = Calib3d.findHomography(pointB, pointA);

        // 문자열로 담아서
        string homoString = homo.dump();


        // 원본
        string[] homoOriginal;

        // 가공 데이터
        string[] homoSplitList;

        // 실사용 가능한 형식으로 여기에 담음
        List<string> realHomoList = new List<string>();

        homoOriginal = homoString.Split(',');

        // [dfdf.dfdf;] 이런식으로 33행렬이 구해짐
        // 결과값에서 숫자만 빼려고 짠 코드        

        for (int i = 0; i < homoOriginal.Length; i++)
        {
            if (i == 0)
            {
                homoSplitList = homoOriginal[i].Split('[');
                realHomoList.Add(homoSplitList[1]);
            }
            else if (i == 2 || i == 4)
            {
                homoSplitList = homoOriginal[i].Split(';');
                for (int j = 0; j < homoSplitList.Length; j++)
                {
                    realHomoList.Add(homoSplitList[j]);
                }
            }
            else if (i == 6)
            {
                homoSplitList = homoOriginal[i].Split(']');
                realHomoList.Add(homoSplitList[0]);
            }
            else
            {
                realHomoList.Add(homoOriginal[i]);
            }
        }

        int count = 0;

        // Json 파일의 1, 2번째는 게임의 넓이와 높이
        dataManager.AddHomo((float)sensor.detectRectWidth);
        dataManager.AddHomo((float)sensor.detectRectHeight);

        // 3~11은 호모그래피 행렬( 33행렬이니 9개)
        for (int j = 0; j < positionTest.homoList.Length; j++)
        {
            for (int k = 0; k < positionTest.homoList[j].homoNum.Length; k++)
            {
                positionTest.homoList[j].homoNum[k] = float.Parse(realHomoList[count]);
                homoList[j].homoNum[k] = float.Parse(realHomoList[count]);
                dataManager.AddHomo(homoList[j].homoNum[k]);
                count++;
            }
        }


        sensor.distanceThresholdForMerge = 1; // 막 따라다니도록
    }


    public void Step3_Good()
    {
        dataManager.SaveHomo();
        Application.Quit();

    }

    public void Step3_Bad()
    {
        Init();
    }


    public void Init()
    {
        xList = new List<float>();
        yList = new List<float>();
    }
}
