using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcWidth
{

    public static void CalcWidthTMP()
    {
        // Получение информации о текстовом объекте
        //TMP_TextInfo textInfo = msgTextField.textInfo;

        // Вычисление ширины текста
        float textWidth = 0f;
        /*for (int i = 0; i < textInfo.lineCount; i++)
        {
            textWidth = Mathf.Max(textWidth, textInfo.lineInfo[i].lineExtents.max.x
                                             - textInfo.lineInfo[i].lineExtents.min.x);
        }*/

        Debug.Log("Ширина текста: " + textWidth);
    }
    
}
