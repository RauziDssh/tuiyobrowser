using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetSharp;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace tuiyoblowser01
{
    public partial class form1 : Form
    {
        Uri nowpage = new Uri("http://google.com");
        Uri prepage;
        List<string> namelist = new List<string>();
        List<string> urilist = new List<string>();
        favorite fv = new favorite();

        public form1()
        {
            InitializeComponent();
            this.Text = "ツイーヨブラウザ：";
            listBox2.Visible = false;
            //ApplicationExitイベントハンドラを追加
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            //label1.Visible = false;
            //お気に入りファイルの読み込み
            
            if (System.IO.File.Exists("favorite.txt"))
            {
                string jsonsource = System.IO.File.ReadAllText("favorite.txt");
                JsonParser JP = new JsonParser();
                fv = JP.getFavoriteFromJSON(jsonsource);
            }
            //リストにお気に入りを追加する
            for (int i = 0; i < fv.name.Length; i++) {
                namelist.Add(fv.name[i].ToString());
                urilist.Add(fv.url[i].ToString());
                listBox2.Items.Add(namelist[i]);
            }
        }

        //お気に入りボタン押下時、お気に入りリストを出す
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (listBox2.Visible == false)
            {
                listBox2.Visible = true;
            }
            else {
                listBox2.Visible = false;
            }
        }

        //お気に入り項目ダブルクリック時、ページ遷移
        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            int index = listBox2.SelectedIndex;
            string url = urilist[index].ToString();
            webBrowser1.Url = new Uri(url);
        }

        //お気に入りに追加する
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            namelist.Add(webBrowser1.DocumentTitle.ToString());
            urilist.Add(webBrowser1.Url.ToString());
            listBox2.Items.Add(webBrowser1.DocumentTitle.ToString());
        }

        //アプリ終了時の動作
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            //お気に入りファイルを保存する
            string[] name = new string[namelist.Count];
            string[] uri = new string[urilist.Count];
            for (int i = 0; i < namelist.Count; i++) {
                name[i] = namelist[i].ToString();
                uri[i] = urilist[i].ToString();
            }
            fv.name = name;
            fv.url = uri;
            //JSONとしてシリアライズする
            string json = JsonConvert.SerializeObject(fv);
            jsonwriter JW = new jsonwriter();
            JW.createFavoriteJSONFile(json);
            
        }

        //戻るボタン
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //なぜか二回たたかないと動かない
            webBrowser1.GoBack();
            webBrowser1.GoBack();
        }

        //進むボタン
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //なぜか二回たたかないと動かない
            webBrowser1.GoForward();
            webBrowser1.GoForward();
        }
        
        //更新ボタン
        private void 更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = nowpage;
        }

        //検索
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            string keyword = toolStripTextBox1.Text;
            keyword = "http://www.google.co.jp/search?q=" + keyword;
            webBrowser1.Url = new Uri(keyword);
        }

        //ウェブページ読み込み時
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            nowpage = webBrowser1.Url;
        }

        //ボタンに文字数カウントさせる
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Text = textBox1.TextLength.ToString();
            if (textBox1.TextLength <= 120)
            {
                button1.ForeColor = Color.Black;
            }
            else
            {
                button1.ForeColor = Color.Red;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //タイトルをフォームのテキストに表示
        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.Text = "ツイーヨブラウザ：" + webBrowser1.DocumentTitle;
        }

        //URL短縮を行う
        public string getShortenURI(string uri)
        {

            string shortServiceURL = "https://www.googleapis.com/urlshortener/v1/url";
            string json = "{\"longUrl\": \"" + uri + "\"}";
            WebClient client = new WebClient { Encoding = Encoding.UTF8 };
            client.Headers["Content-Type"] = "application/json";
            string getjsonsource = client.UploadString(shortServiceURL, json);
            JsonParser JP1 = new JsonParser();
            string shortenURI = JP1.getStingFromJSON(getjsonsource,"id");
            return shortenURI;
        }

        //Twitter認証部
        TwitterService service;
        string verifycode;
        OAuthRequestToken myRequetToken;
        OAuthAccessToken myAccessToken;
        Uri oauthUri;

        //ログイン処理
        private void ログインToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            string filepath = "settings.txt";
            if (System.IO.File.Exists("settings.txt"))
            {
                //二回目以降ログイン時
                string jsonsource = System.IO.File.ReadAllText(filepath);
                JsonParser JP1 = new JsonParser();
                settings.requestSecretToken = JP1.getStingFromJSON(jsonsource,"requestSecretToken");
                settings.requestToken = JP1.getStingFromJSON(jsonsource, "requestToken");
                settings.accessSecretToken = JP1.getStingFromJSON(jsonsource, "accessSecretToken");
                settings.accessToken = JP1.getStingFromJSON(jsonsource,"accessToken");
                label1.Text = string.Format("R = {0},RS = {1},A = {2},AS = {3}", settings.requestToken, settings.requestSecretToken, settings.accessSecretToken, settings.accessToken);
                GetToken(settings);
            }
            else
            {*/
                //初回ログイン時
                prepage = webBrowser1.Url;

                service = new TwitterService("key", "secret");
                

                myRequetToken = service.GetRequestToken();
                oauthUri = service.GetAuthenticationUrl(myRequetToken);
                oauthing = true;
                webBrowser1.Url = oauthUri;
            //}
        }

        TwitterSettings settings = new TwitterSettings();

        /*
        private void GetToken(TwitterSettings ts){
            service.AuthenticateWith(ts.accessToken,ts.accessSecretToken);
            startStream();
        }*/

        private void GetToken(){
            myAccessToken = service.GetAccessToken(myRequetToken,verifycode);
            service.AuthenticateWith(myAccessToken.Token, myAccessToken.TokenSecret);
            
            //アクセストークン・リクエストトークンを保存
            settings.accessToken = myAccessToken.Token;
            settings.accessSecretToken = myAccessToken.TokenSecret;
            settings.requestToken = myRequetToken.Token;
            settings.requestSecretToken = myRequetToken.TokenSecret;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
            jsonwriter JW = new jsonwriter();
            JW.createJSONFile(json);
            //認証前の画面にとばす
            webBrowser1.Url = prepage;
            //ユーザーストリーム開始
            startStream();
        }

        //ユーザーストリーム
        private void startStream() {
            service.StreamUser((artfct, rspns) =>
                {
                    if (artfct is TwitterUserStreamStatus) {
                        TwitterUserStreamStatus status = (TwitterUserStreamStatus)artfct;
                        mystatus = status;
                        Thread t = new Thread(new ThreadStart(handler));
                        t.Start();
                    }
                });
        }

        delegate void userstreamdelegate();
        TwitterUserStreamStatus mystatus;

        void handler(){
            Invoke(new userstreamdelegate(writeToList));
        }

        WebClient wc = new WebClient();

        //ユーザーストリーム取得時の処理
        List<string> getUser = new List<string> { };
        private void writeToList()
        {
            string dir = mystatus.Author.ScreenName.ToString() + ".jpg";
            string filepath = @"icon\" + dir;

            //iconをダウソしてなかったらダウソ
            if (System.IO.File.Exists(filepath) == false)
            {
                getUser.Add(mystatus.Author.ScreenName.ToString());
                wc.DownloadFile(mystatus.Author.ProfileImageUrl, filepath);
            }
            
            Image icon = Image.FromFile(filepath);
            listBox1.Items.Add(icon);
            listBox1.Items.Add(mystatus.Author.ScreenName);
            listBox1.Items.Add(mystatus.Text);
            listBox1.Items.Add("");
            listBox1.Items.Add("--------------------------------------------------");
            listBox1.TopIndex = listBox1.Items.Count - listBox1.Height / listBox1.ItemHeight;
            
        }

        bool oauthing = false;
        int count = 0;

        //PINを取得、トークンを投げる
        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Text = "ツイーヨブラウザ：" + webBrowser1.DocumentTitle;
            
            if (oauthing == true) {
                count++;
                if (count >= 2)
                {
                    string code = webBrowser1.DocumentText;
                    int loc = code.IndexOf("<CODE>");
                    if (loc > 0)
                    {
                        verifycode = code.Substring(loc, 13);
                        verifycode = verifycode.Replace("<CODE>", "");
                        GetToken();
                        oauthing = false;
                    }
                }
            }
        }
        
        //ツイートボタン
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength <= 140)
            {
                string content = textBox1.Text;
                sendTweet(content);
                textBox1.Clear();
                button1.Text = "つぶやいた";
            }
            else {
                button1.Text = "文字数Over";
            }
        }

        //閲覧中のサイトをつぶやく
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string shortenURI = this.getShortenURI(webBrowser1.Url.ToString());
            textBox1.Text = shortenURI.ToString() + " -閲覧中";
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) {
                return;
            }
            if (e.Index % 5 == 0)
            {
                Image thumbnail = (Image)listBox1.Items[e.Index];
                // 画像を右端に表示
                e.Graphics.DrawImage(thumbnail,
                  0,
                  e.Bounds.Y + (e.Bounds.Height - thumbnail.Height) / 2);
            }
            else {
                string txt = ((ListBox)sender).Items[e.Index].ToString();
                Rectangle rec = new Rectangle(e.Bounds.X + 50, e.Bounds.Y - 20, e.Bounds.Width - 64,e.Bounds.Height);
                Graphics g = e.Graphics;
                Color col = Color.Black;
                //文字列を描画する
                TextRenderer.DrawText(g, txt, e.Font, rec, col, TextFormatFlags.Default);
                //フォーカスを示す四角形を描画
                e.DrawFocusRectangle(); 
            }
        }

        //ツイート送信
        private void sendTweet(string strTweet)
        {
            service.SendTweet(new SendTweetOptions { Status = strTweet });
        }

        

        

        
            
    }
}

