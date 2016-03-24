using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
            txtUrl.Text = @"http://vechai.info/one-piece";
        }

        private void btnCraw_Click(object sender, System.EventArgs e)
        {
            var url = txtUrl.Text;

            var pageContent = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(url);

            var htmlDoc = new HtmlAgilityPack.HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
            htmlDoc.LoadHtml(pageContent);

            var parentChapterListNote = htmlDoc.DocumentNode.Descendants().FirstOrDefault(x => x.Id == "chapterList");
            if (parentChapterListNote == null)
            {
                return;
            }

            var chapterListNote = parentChapterListNote.Descendants().LastOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("inner"));
            if (chapterListNote == null)
            {
                return;
            }


            var chapters = chapterListNote.Descendants("a").ToList();
            foreach (var chapter in chapters)
            {
                var href = chapter.Attributes["href"] != null ? chapter.Attributes["href"].Value : "";
                var title = chapter.InnerText;

                CrawChapter(href, title);
            }
        }

        private void CrawChapter(string url, string title)
        {
            var pageContent = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(url);

            var htmlDoc = new HtmlAgilityPack.HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
            htmlDoc.LoadHtml(pageContent);

            RemoveNoNeedHtml(htmlDoc);

            var contentNote = htmlDoc.DocumentNode.Descendants().FirstOrDefault(x => x.Attributes["id"] != null && x.Attributes["id"].Value == "contentChapter");
            if (contentNote == null)
            {
                return;
            }
            var imgs = contentNote.Descendants().Where(x => x.Name == "img").ToList();
            if (imgs.Any() == false)
            {
                return;
            }

            foreach (var imgNote in imgs)
            {
                var srcAttr = imgNote.Attributes["src"];
                var altAttr = imgNote.Attributes["alt"];

                if (srcAttr == null)
                {
                    continue;
                }

                using (var webClient = new WebClient())
                {
                    try
                    {
                        var uri = new Uri(srcAttr.Value);
                        var query = uri.Query + "";
                        var path = uri.AbsolutePath.Replace(query, "");
                        var subStringIndex = path.LastIndexOf(@"/") + 1;
                        var fileName = path.Substring(subStringIndex, path.Length - subStringIndex);

                        if (fileName.Contains("."))
                        {
                            webClient.DownloadFile(srcAttr.Value, fileName);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        private void RemoveNoNeedHtml(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            htmlDoc.DocumentNode.Descendants()
               .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "noscript")
               .ToList()
               .ForEach(n => n.Remove());
        }
    }
}