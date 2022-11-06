using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HKY;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UtilsModule;
using System;


// ����ڰ� �̸��극�̼� �ϴ� �뵵

public class UserSensorCeli : MonoBehaviour
{
    [Header("���� ������Ʈ ����")]
    [SerializeField]
    URGSensorObjectDetector sensor;

    [SerializeField]
    SeonsorCeliPo seonsorPo;

    [SerializeField]
    CeliPositionTest positionTest;

    [SerializeField]
    CeliDataManager dataManager;


    // ���ܿ��� ���� �ֵ�

    [Header("��ũ�� ����")]
    [SerializeField]
    Text inputWidth;

    [Header("��ũ�� ����")]
    [SerializeField]
    Text inputHeight;    

        
    [Header("���� ������ ������ ����")]

    [Header("���� 3 ȣ��׷��� ���")]
    [SerializeField]
    float gameWidth;

    [Header("���� ������ ������ ����")]
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


    // ���� 1
    // ��������ϴ� ��ũ�� ũ�� �Է¹ް� ������ ����(���ӵ鿡 ��ũ�� ũ�� �˷��ִ� �뵵)
    public void Step1_ScreenSizeInput()
    {
        sensor.detectRectWidth = int.Parse(inputWidth.text);
        sensor.detectRectHeight = int.Parse(inputHeight.text); ;
    }

    float temp;

    // ����2 ������ ��� ������ �˸�
    // ���� ��ġ�� ���� ��� �����ϵ��� ���� �ʿ�
    // ������ ����ڰ� ������ ��ġ�� ������� ���߸� �̸��극�̼� ����

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
    

    // ����3 ȣ��׷��� ��� ���
    
    public void Step3_HomoStart()
    {
        // ����� ���� ũ��
        float gameX = gameWidth / 2;
        float gameY = gameHeight;

        // ���� ������ ����Ʈ �������� ��ȯ ��(OpenCV �ڷ����ε�..)
        // �ΰ��� �ػ� ���
        Point a1 = new Point(-gameX, 0);  // �޾Ʒ�(1)
        Point a2 = new Point(gameX, 0);   // �����Ʒ�(2)
        Point a3 = new Point(-gameX, gameY);   // ����(3)
        Point a4 = new Point(gameX, gameY);    // ������(4)
       
        // ���� ����ϴ� ��ũ�� ũ��, �̷��� �ϰ� ���� ����ڰ� ���� �ʿ� ���� �Է��ϵ��� �ص� ������..
        Point b1 = new Point(float.Parse(leftDownX.text), float.Parse(leftDownY.text));  // �޾Ʒ�(1)
        Point b2 = new Point(float.Parse(rightDownX.text), float.Parse(rightDownY.text));   // �����Ʒ�(2)
        Point b3 = new Point(float.Parse(leftUpX.text), float.Parse(leftUpY.text));   // ����(3)
        Point b4 = new Point(float.Parse(rightUpX.text), float.Parse(rightUpY.text));    // ������(4)


        // ����Ʈ �迭�� ����
        Point[] arrayA = new Point[] { a1, a2, a3, a4 };
        Point[] arrayB = new Point[] { b1, b2, b3, b4 };


        // �װɷ� MAT����Ʈ �������� �����(ȣ��׷��� ��� ��ȯ�� ����..)
        MatOfPoint2f pointA = new MatOfPoint2f(arrayA);
        MatOfPoint2f pointB = new MatOfPoint2f(arrayB);

        // ȣ��׷��� ��� ���(3.3���)
        Mat homo = Calib3d.findHomography(pointB, pointA);

        // ���ڿ��� ��Ƽ�
        string homoString = homo.dump();


        // ����
        string[] homoOriginal;

        // ���� ������
        string[] homoSplitList;

        // �ǻ�� ������ �������� ���⿡ ����
        List<string> realHomoList = new List<string>();

        homoOriginal = homoString.Split(',');

        // [dfdf.dfdf;] �̷������� 33����� ������
        // ��������� ���ڸ� ������ § �ڵ�        

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

        // Json ������ 1, 2��°�� ������ ���̿� ����
        dataManager.AddHomo((float)sensor.detectRectWidth);
        dataManager.AddHomo((float)sensor.detectRectHeight);

        // 3~11�� ȣ��׷��� ���( 33����̴� 9��)
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


        sensor.distanceThresholdForMerge = 1; // �� ����ٴϵ���
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
