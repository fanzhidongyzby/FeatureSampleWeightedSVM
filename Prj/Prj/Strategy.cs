using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using System.IO;
using SVM;
namespace Prj
{
    class OneClass:IComparable
    {
        double distance;//测试样本与类重心距离
        double label;//类标签
        double[] center;//类重心
        List<DataRow> samples;//同类样本
        List<double> similars;//样本相似度
        public OneClass(double label)
        {
            this.label = label;
            samples = new List<DataRow>();
            similars = new List<double>();
        }
        //public double rDistance(OneClass one)
        //{
        //    int num = center.Length;//属性个数
        //    double dis = 0.0;//距离
        //    for (int i = 0; i < num; i++)
        //    {
        //        dis+=Math.Pow(one.center[i]-center[i],2);
        //    }
        //    return Math.Sqrt(dis);
        //}
        public void addSample(DataRow data,double s)
        {
            samples.Add(data);//添加数据
            similars.Add(s);//添加相似度
        }
        public double getDistance()
        {
            return distance;
        }
        public double getLabel()
        {
            return label;
        }
        public void init(DataRow test,List<int>core,List<double>allweights)
        {
            int attrNum=core.Count;//属性个数
            center=new double[attrNum];//重心
            int sampleNum = samples.Count;//样本数
            double sumSimilar = 0;//总相似度
            //累加样本
            for (int i = 0; i < sampleNum; i++)
            {
                DataRow sample = samples[i];
                sumSimilar += similars[i];//累加相似度
                for (int j = 0; j < attrNum; j++)
                {
                    //样本加权
                    int id = core[j];
                    center[j] += similars[i] * double.Parse(sample[id].ToString());
                }
            }
            //计算重心和距离
            for (int i = 0; i < attrNum; i++)
            {
                //不进行属性加权
                center[i] = center[i] * allweights[core[i] - 1] / sumSimilar;
                double tmpVal=double.Parse(test[core[i]].ToString());
                distance += Math.Pow((tmpVal * allweights[core[i] - 1] - center[i]), 2);
            }
            distance = Math.Sqrt(distance);
        }
        public int CompareTo(object o)
        {
            OneClass one=(OneClass)o;
            if (distance > one.distance)
                return 1;
            else
                return -1;
        }
        public double compare(OneClass one, DataRow test, List<int> core)
        {
            List<Node[]> _X = new List<Node[]>();
            List<double> _Y = new List<double>();
            Problem.sampleSimilar.Clear();
            int attrNum=core.Count;
            int sampleNum = samples.Count;
            for (int i = 0; i < sampleNum; i++)
            {
                //初始化样本权值
                Problem.sampleSimilar.Add(similars[i]);
                DataRow sample = samples[i];//训练样本
                List<Node> sampleVal = new List<Node>();
                for (int j = 0; j < attrNum; j++)
                {
                    sampleVal.Add(new Node(j + 1, Convert.ToDouble(sample[core[j]])));
                }
                _X.Add(sampleVal.ToArray());//关键属性
                _Y.Add(Convert.ToDouble(sample[0]));//决策属性
            }
            sampleNum = one.samples.Count;//另一类样本个数
            for (int i = 0; i < sampleNum; i++)
            {
                //初始化样本权值
                Problem.sampleSimilar.Add(one.similars[i]);
                DataRow sample = one.samples[i];//训练样本
                List<Node> sampleVal = new List<Node>();
                for (int j = 0; j < attrNum; j++)
                {
                    sampleVal.Add(new Node(j + 1, Convert.ToDouble(sample[core[j]])));
                }
                _X.Add(sampleVal.ToArray());//关键属性
                _Y.Add(Convert.ToDouble(sample[0]));//决策属性
            }
            //构造测试样本数据
            List<Node> testVal = new List<Node>();
            for (int j = 0; j < attrNum; j++)
            {
                testVal.Add(new Node(j + 1,/*Problem.attrWeight[core[j]-1]**/Convert.ToDouble(test[core[j]])));
            }
            //构造Svm测试
            Svm svm=new Svm(_X.ToArray(), _Y.ToArray());
            double preRes=svm.predict(testVal.ToArray());
            if (preRes != label)
            {
                return label;
            }
            else
                return one.label;
        }
    }
    class Strategy
    {
        Dictionary<int, double> id_u;
        List<int> ids;
        List<int> core;
        List<double> allweights;
        List<OneClass> classes;//分类记录
        public Strategy(Dictionary<int, double> id_u,List<double> allweights,List<int>core)
        {
            this.id_u = id_u;
            this.core = core;
            ids = new List<int>();
            Dictionary<int, double>.Enumerator similarIds = id_u.GetEnumerator();
            while (similarIds.MoveNext())
            {
                KeyValuePair<int, double> cur = similarIds.Current;
                ids.Add(cur.Key);
            }
            this.allweights = allweights;
            classes = new List<OneClass>();
            Problem.attrWeight.Clear();//清除属性权值
            int attrNum = core.Count;//属性个数
            //初始化属性权值
            for (int i = 0; i < attrNum; i++)
            {
                Problem.attrWeight.Add(allweights[core[i]-1]);
            }
        }

        public void classify(List<DataRow> trainCollection)
        {

            //将训练样本按照类别分类，标号有序
            int sampleNum=ids.Count;//训练样本数
            double oldLabel = -1;//临时记录标签
            OneClass one = null;
            for (int i = 0; i < sampleNum; i++)
            {
                DataRow sample = trainCollection[ids[i]];//测试样本
                double label=double.Parse(sample[0].ToString());//类标签
                if (label==oldLabel)//同类
                {
                    one.addSample(sample, id_u[ids[i]]);
                }
                else //新类
                {
                    oldLabel = label;//更新临时标签
                    one = new OneClass(label);//构造类别
                    one.addSample(sample, id_u[ids[i]]);//添加样本及其相似度
                    classes.Add(one);
                }
            }
        }
        void  sortClassByDistance(DataRow test)
        {
            int num = classes.Count;//分类数
            for (int i = 0; i < num; i++)
            {
                classes[i].init(test, core, allweights);
            }
            classes.Sort();
        }
        public bool predict(DataRow test)
        {
            //按照距离排序
            this.sortClassByDistance(test);
            int num=classes.Count;//类个数
            int i,j;
            for ( i= 0, j = num - 1; i != j; )
            {
                double notlabel=classes[i].compare(classes[j],test,core);
                if (notlabel == classes[i].getLabel())//不属于i类
                {
                    i++;
                }
                else
                    j--;
            }
            if (classes[i].getLabel() != double.Parse(test[0].ToString()))
            {
                //Console.WriteLine(" real=" + double.Parse(test[0].ToString()) + " pre=" + classes[i].getLabel());
                File.AppendAllText("./rslt.txt", " real=" + double.Parse(test[0].ToString()) + " pre=" + classes[i].getLabel()+Environment.NewLine);
            }
            else
            {
                //Console.WriteLine(" real=" + double.Parse(test[0].ToString()));
                File.AppendAllText("./rslt.txt", " real=" + double.Parse(test[0].ToString())+Environment.NewLine);
            }
            return classes[i].getLabel() == double.Parse(test[0].ToString());
        }
    }
}
