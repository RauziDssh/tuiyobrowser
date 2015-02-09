using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tuiyoblowser01
{
    class jsonwriter
    {
        public void createJSONFile(string json) {
            //Shift JISで書き込む
            //書き込むファイルが既に存在している場合は、上書きする
            System.IO.StreamWriter sw = new System.IO.StreamWriter("settings.txt",
                false,
                System.Text.Encoding.GetEncoding("shift_jis"));
            sw.Write(json);
            //閉じる
            sw.Close();
        }
        public void createFavoriteJSONFile(string json) { 
            //Shift JISで書き込む
            //書き込むファイルが既に存在している場合は、上書きする
            System.IO.StreamWriter sw = new System.IO.StreamWriter("favorite.txt",
                false,
                System.Text.Encoding.GetEncoding("utf-8"));
            sw.Write(json);
            //閉じる
            sw.Close();
        }
    }
}
