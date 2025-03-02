using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Data
{
     public float angles;
     public float current;
     public byte rotate;

     public byte controlCode;

     public byte errorCode;
}
public class MainUI : MonoBehaviour
{

     public Data data;
     public Button btn_manual;
  
     public Button btn_zero;

     public Button btn_sine;

     public float maxCurrent = 0.8f;

     public Slider slider_i;
     public Text txt_message;
 
     private Wheel wheel;

     private bool manual =true;
     void Awake()
     {
          wheel = FindObjectOfType<Wheel>();
     }

     void Start()
     {
          btn_manual.onClick.AddListener
          (()=>{
               manual = !manual;

               btn_manual.GetComponentInChildren<Text>().text = manual ? "手动模式" : "自动模式";
               data.controlCode = (byte)(manual ? 0x00 : 0x01);
          });

          btn_zero.onClick.AddListener
          (() =>
          {
               data.controlCode = 0x02;

          });
          btn_sine.onClick.AddListener(() =>
          {
               data.controlCode = 0x03;
          });
     }

     public void CodeUpdated()
     {
          if(data.controlCode == 0x00)
          {
               manual = true;
          }
          if(data.controlCode == 0x01)
          {
               manual = false;
          }
          btn_manual.GetComponentInChildren<Text>().text = manual ? "手动模式" : "自动模式";
     }

     void Update()
     {
          if(Input.GetKey(KeyCode.RightArrow)){
               data.rotate = 0x01;
          }
          else if(Input.GetKey(KeyCode.LeftArrow)){
               data.rotate = 0x02;
          }
          else
               data.rotate= 0x00;
          slider_i.value = data.current / maxCurrent;
          CodeUpdated();
          switch (data.errorCode)
          {
               case 0x00:
                    txt_message.text = "正常运行中";
                    break;
               case 0x01:
                    txt_message.text = "过流警告！";
                    break;
               case 0x02:
                    txt_message.text = "过压警告！";
                    break;
               case 0x03:
                    txt_message.text = "欠压警告！";
                    break;
               case 0x04:
                    txt_message.text = "过温警告！";
                    break;
               default:
                    txt_message.text = "";
                    break;
          }
     }
}
