using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace Nayoh.Class
{
    public class Quiz
    {
        public string Question;
        public string Answer;

        public Quiz()
        {

        }

        public string GetQuestion()
        {
            return Question;
        }

        public string GetAnswer()
        {
            return Answer;
        }

        public List<Quiz> GetList(string Exe_Path)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<Quiz> quiz = new List<Quiz>();
            string content = File.ReadAllText(Exe_Path + "/System Files/data.json");
            quiz = js.Deserialize<Quiz[]>(content).ToList();

            return quiz;
        }
    }

}
