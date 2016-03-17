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

                CrawChapter(href);
            }
        }

        private void CrawChapter(string url)
        {
            var pageContent = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(url);

            var htmlDoc = new HtmlAgilityPack.HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
            htmlDoc.LoadHtml(pageContent);

            var cc = htmlDoc.DocumentNode.InnerHtml;
        }
    }
}