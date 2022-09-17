using Nayoh.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Nayoh.Intersect
{
    public class Files
    {
        private readonly string Exe_Path = AppDomain.CurrentDomain.BaseDirectory;

        public Tuple<List<Quiz>, Dictionary<string, object>, string, string, bool, int> GenerateFiles()
        {
            List<Quiz> quiz = new List<Quiz>();
            string r1, r2, r3, error, message;
            Dictionary<string, object> conf = new Dictionary<string, object>();
            bool failure;
            int target;
            r1 = GenerateDirectory();
            r2 = GenerateQuiz();
            r3 = GenerateConf();

            if (r1 == "null" && r2 == "null" && r3 == "null")
            {
                (quiz, error, message, failure, target) = QuizAnalysis();
                if (error == "null" && message == "null")
                {
                    (conf, error, message, failure, target) = ConfAnalysis();
                    return Tuple.Create(quiz, conf, error, message, failure, target);
                }
                else
                {
                    return Tuple.Create(quiz, conf, error, message, failure, target);
                }
            }
            else
            {
                if (r1 != "null")
                {
                    return Tuple.Create(quiz, conf, r1, "System Generation Error: Folder Problem. (Type <ok> to Reset)", true, 0);
                }
                else if (r2 != "null")
                {
                    return Tuple.Create(quiz, conf, r2, "System Generation Error: Quiz Problem. (Type <ok> to Reset)", true, 1);
                }
                else
                {
                    return Tuple.Create(quiz, conf, r3, "System Generation Error: Configuration Problem. (Type <ok> to Reset)", true, 2);
                }
            }
        }

        public string GenerateDirectory()
        {
            string reply = "null";

            try
            {
                Directory.CreateDirectory(Exe_Path + "/System Files");
                Directory.CreateDirectory(Exe_Path + "/System Files/Intersect");
            }
            catch (System.Exception e)
            {
                reply = e.ToString();
            }

            return reply;
        }

        public string GenerateQuiz()
        {
            string reply = "null";

            try
            {
                if (!File.Exists(Exe_Path + "/System Files/data.json"))
                {
                    File.WriteAllText(Exe_Path + "/System Files/data.json", "[\n\t{\"Question\": \"Knock Knock.\", \"Answer\": \"I'm Here.\"}, \n\t{\"Question\": \"1 or 11 ?\", \"Answer\": \"Aces, Charles.\"}\n]");
                }
            }
            catch (System.Exception e)
            {
                reply = e.ToString();
            }

            return reply;
        }

        public string GenerateConf()
        {
            string reply = "null";

            try
            {
                if (!File.Exists(Exe_Path + "/System Files/conf.json"))
                {
                    File.WriteAllText(Exe_Path + "/System Files/conf.json", "{\n\t\"Intersect Image Delay\": 50, \n\t\"Intersect Message\": \"Activation Complete.\", \n\t\"Welcome Message\": \"Hello Son.\", \n\t\"Clipboard\": \"You're Special, Son.\", \n\t\"Skip\": true, \n\t\"Debug\": true\n}");
                }
            }
            catch (System.Exception e)
            {
                reply = e.ToString();
            }

            return reply;
        }

        public string DeleteQuiz()
        {
            try
            {
                File.Delete(Exe_Path + "/System Files/data.json");
                return "null";
            }
            catch (System.Exception e)
            {
                return e.ToString();
            }
        }

        public string DeleteConf()
        {
            try
            {
                File.Delete(Exe_Path + "/System Files/conf.json");
                return "null";
            }
            catch (System.Exception e)
            {
                return e.ToString();
            }
        }

        public Tuple<List<Quiz>, string, string, bool, int> QuizAnalysis()
        {
            List<Quiz> quiz = new List<Quiz>();
            Quiz jsonQuizData = new Quiz();
            string r1, r2;

            try
            {
                if (new FileInfo(Exe_Path + "/System Files/data.json").Length == 0)
                {
                    r1 = DeleteQuiz();
                    r2 = GenerateQuiz();
                    if (r1 == "null" && r2 == "null")
                    {
                        quiz = jsonQuizData.GetList(Exe_Path);
                        return Tuple.Create(quiz, "null", "null", false, 1);
                    }
                    else if (r1 != "null")
                    {
                        return Tuple.Create(quiz, r1, "System Analysis Error: Quiz Delete Problem. (Type <ok> to Reset)", true, 1);
                    }
                    else
                    {
                        return Tuple.Create(quiz, r2, "System Analysis Error: Quiz Generate Problem. (Type <ok> to Reset)", true, 1);
                    }
                }
                else
                {
                    quiz = jsonQuizData.GetList(Exe_Path);

                    if (quiz.Count < 1)
                    {
                        return Tuple.Create(quiz, "System Analysis Error: List Content Must Be >= 1", "System Error. (Type <ok> to Reset)", true, 1);
                    }
                    else
                    {
                        return Tuple.Create(quiz, "null", "null", false, 1);
                    }
                }

            }
            catch (Exception e)
            {
                return Tuple.Create(quiz, e.ToString(), "System Quiz Analysis Error. (Type <ok> to Reset)", true, 1);
            }
        }

        public Tuple<Dictionary<string, object>, string, string, bool, int> ConfAnalysis()
        {
            JavaScriptSerializer SerializerJson = new JavaScriptSerializer();
            Dictionary<string, object> conf = new Dictionary<string, object>();
            string r1, r2;

            try
            {
                if (new FileInfo(Exe_Path + "/System Files/conf.json").Length == 0)
                {
                    r1 = DeleteConf();
                    r2 = GenerateConf();

                    if (r1 == "null" && r2 == "null")
                    {
                        conf = (Dictionary<string, object>)SerializerJson.Deserialize<object>(File.ReadAllText(Exe_Path + "/System Files/conf.json"));
                        return Tuple.Create(conf, "null", "null", false, 2);
                    }
                    else if (r1 != "null")
                    {
                        return Tuple.Create(conf, r1, "System Analysis Error: Quiz Delete Problem. (Type <ok> to Reset)", true, 2);
                    }
                    else
                    {
                        return Tuple.Create(conf, r2, "System Analysis Error: Quiz Generate Problem. (Type <ok> to Reset)", true, 2);
                    }
                }
                else
                {
                    conf = (Dictionary<string, object>)SerializerJson.Deserialize<object>(File.ReadAllText(Exe_Path + "/System Files/conf.json"));
                    if (conf["Intersect Image Delay"] is int && conf["Intersect Message"] is string && conf["Welcome Message"] is string && conf["Clipboard"] is string && conf["Skip"] is bool && conf["Debug"] is bool)
                    {
                        return Tuple.Create(conf, "null", "null", false, 2);
                    }
                    else
                    {
                        return Tuple.Create(conf, "System Serializer Wrong Value Type.", "System Serializer Error. (Type <ok> to Reset)", true, 2);
                    }
                }
            }
            catch (Exception e)
            {
                return Tuple.Create(conf, e.ToString(), "System Conf. Analysis Error. (Type <ok> to Reset)", true, 2);
            }
        }
    }
}