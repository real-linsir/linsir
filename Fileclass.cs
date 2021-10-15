using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace _210926_Ski_Screen3Analysis
{
    
    /// <summary>
    /// Json文件类，用于存放配置文件，人员信息信息文件，历史分析滑行数据，赛道数据，滑行分析数据等。
    /// </summary>
    public class JsonFile
    {
        public string json_string;
        string file_path_name;

        /// <summary>
        /// Json文件构造函数
        /// </summary>
        /// <param name="file_name">文件名，例如：0001_BP.json(训练基本参数文件)，或例如：CF_001.json(人员信息文件)</param>
        /// <param name="path_son">文件路径，例如："\滑行日志数据\0001滑行数据"(训练基本参数文件路径)，或例如："\人员信息"（人员信息文件路径）</param>
        /// <param name="file_type">
        ///      文件类型描述：存在0，1两种
        ///      0：数据库信息文件，命名中无需要生成时间信息；
        ///      1：单次滑行信息文件，命名中需要生成时间信息；
        ///      </param>
        public JsonFile(string file_name, string path_son, UInt16 file_type)
        {
            //判断是否存在目录，若不存在则创建目录；
            DirectoryInfo directoryinfo = new DirectoryInfo(Application.StartupPath + path_son);
            if (!directoryinfo.Exists) { directoryinfo.Create(); }
            //判断文件是否存在；
            string[] name = file_name.Split('.');
            switch (file_type)
            {
                case 0:
                    file_path_name = directoryinfo.ToString() + @"\" + file_name;
                    break;
                case 1:
                    FileInfo[] jsonfiles = directoryinfo.GetFiles("*" + name[0] + "*");
                    if (jsonfiles.Length == 0)
                        file_path_name = directoryinfo.ToString() + @"\" + name[0] + DateTime.Now.ToString("_yyyyMMddhhmmss") + "." + name[1];
                    else
                        file_path_name = directoryinfo.ToString() + @"\" + jsonfiles[0].Name;
                    break;
                default:
                    break;
            }
            if (!File.Exists(file_path_name))
            {
                File.Create(file_path_name);
            }
        }

        //读出文件中过的所有数据放在一个字符串中并返回；
        public string Read()
        {
            json_string = null;
            try
            {
                StreamReader reader = new StreamReader(file_path_name, Encoding.Default);
                while (!reader.EndOfStream)
                {
                    json_string += reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return json_string;
        }

        //向文件中写一个json字符串；
        public void Write(string str)
        {
            StreamWriter writer = new StreamWriter(file_path_name,false,System.Text.Encoding.Default);
            writer.Write(str);
            writer.Close();
        }

        //json读取示例
        //public void jsonreadsample()
        //{
        //    //string jsonstr = "{\"Name\" : \"Jack\", \"Age\" : 34, \"Colleagues\" : [{\"Name\" : \"Tom\" , \"Age\":44},{\"Name\" : \"Abel\",\"Age\":29}] }";
        //    //将json转换为JObject
        //    JObject jo = JObject.Parse(json_string);
        //    JToken ageToken = jo["sites"]; //获取该员工的姓名
        //    //Console.WriteLine(ageToken.ToString());
        //    //Console.WriteLine(jo["Age"].ToString());

        //    //获取该员工同事所有姓名（读取json数组）
        //    //var names = from staff in jo["Colleagues"].Children() select (string)staff["Name"];
        //    //foreach (var name in names)
        //    //    Console.WriteLine(name);
        //    //var ages = from staff in jo["Colleagues"].Children() select (string)staff["Age"];
        //    //foreach (var age in ages)
            
        //    Console.WriteLine(ageToken[1]["url"].ToString());
        //    Console.WriteLine(ageToken[0]["url"].ToString());
        //    Console.WriteLine(ageToken[2]["url"].ToString());
        //}
    }

    
    /// <summary>
    /// csv文本文件，用于存储滑行过程中实时产生的数据。
    ///     使用结束需要调用close（）方法；
    /// </summary>
    public class CsvFile
    {
        public string file_path_name;
        UInt32 file_len = 0;//保存的数据包计数。若为0，则表示没有数据，文件可在退出时删除
        public StreamReader reader;

        /// <summary>
        /// 用于生成csv文件的构造方法；
        /// </summary>
        /// <param name="file_name">文件的ID名称如：0001_PP.csv</param>
        /// <param name="pathSon">局部路径名称如："\0001滑行记录"</param>
        public CsvFile(string file_name,string pathSon)
        {
            //判断目录是否存在，若不存在则创建目录；
            DirectoryInfo directoryinfo = new DirectoryInfo(Application.StartupPath + @"\滑行日志数据" + pathSon);
            if (!directoryinfo.Exists){directoryinfo.Create();}
            //判断文件是否存在，滤去时间信息比对文件名称，若文件存在，使用原文件路径；
            string[] name = file_name.Split('.');
            FileInfo[] csvfiles = directoryinfo.GetFiles("*" + name[0] + "*");
            if (csvfiles.Length == 0)
                file_path_name = directoryinfo.ToString() + @"\" + name[0] + DateTime.Now.ToString("_yyyyMMddhhmmss") + "." + name[1];
            else
            {
                file_path_name = directoryinfo.ToString() + @"\" + csvfiles[0].Name;
                bHaveData = true;
            }

            if (!File.Exists(file_path_name))
            {
                File.Create(file_path_name);
            }
            reader = new StreamReader(file_path_name, Encoding.Default);
        }

        public bool bHaveData = false;//文件中有数据
        public void write(string str)//像文件中写一行数据；
        {
            StreamWriter writer = new StreamWriter(file_path_name, true, Encoding.Default);
            writer.WriteLine(str);
            writer.Flush();
            writer.Close();
            file_len++;
            bHaveData = true;
        }

        public string read()//读出文件中的所有数据放在一个字符串中并返回；
        {
            var s = "";
            try
            {
                StreamReader readerall = new StreamReader(file_path_name, Encoding.Default);
                while (!readerall.EndOfStream)
                {
                    s += readerall.ReadLine() + "\n";
                }
                readerall.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return s;
        }

        public string readline()//逐行读出文件中的所有数据；
        {
            var aline = "";
            try
            {
                aline = reader.ReadLine() + "\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return aline;
        }

        public void close()
        {
            if (!bHaveData)
            {
                reader.Close();
                if (File.Exists(file_path_name))
                {
                    File.Delete(file_path_name);
                }
            }
        }
    }


    /// <summary>
    /// log文件，用于存储 日志文件
    ///     使用结束需要调用close（）方法；
    /// </summary> 
    public class LogFile
    {
        string file_path_name;
        StreamWriter writer;
        UInt32 file_len = 0;//保存的数据包计数。若为0，则表示没有数据，文件可在退出时删除

        /// <summary>
        /// 日志文件的构造方法
        /// </summary>
        /// <param name="file_name">文件名，例如：log.dat</param>
        public LogFile(string file_name)
        {
            if (!Directory.Exists(Application.StartupPath + @"\运行日志文件"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\运行日志文件");
            }
            string[] name = file_name.Split('.');
            file_path_name = Application.StartupPath + @"\运行日志文件\" + name[0] + DateTime.Now.ToString("_yyyyMMddhhmmss") + "." + name[1];
            if (!File.Exists(file_path_name))
            {
                File.Create(file_path_name);
            }

        }
        public bool bHaveData = false;//文件中有数据

        public void write(string str)
        {
            string cc;
            writer = new StreamWriter(file_path_name, true, Encoding.Default);
            cc = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:") + str;
            writer.WriteLine(cc);
            writer.Flush();
            file_len++;
            bHaveData = true;
            writer.Close();
        }

        public string read()
        {
            var s = "";
            try
            {
                StreamReader reader = new StreamReader(file_path_name, Encoding.Default);
                while (!reader.EndOfStream)
                {
                    s += reader.ReadLine() + "\n";
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return s;
        }

        public void close()
        {
            if (!bHaveData)
            {
                if (File.Exists(file_path_name))
                {
                    File.Delete(file_path_name);
                }
            }
        }
    }
}


