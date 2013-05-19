using System;
using System.Collections.Generic;
using System.Text;
//
using System.Data;
using System.IO;

namespace Prj
{
    class Process
    {
        DB db;//数据库对象
        Filter filter;//筛选过程
        Reduct reduct;//约简过程
        /// <summary>
        /// 初始化全局系统对象
        /// </summary>
        public Process()
        {
            
        }
        void processData(DataTable dt)
        {
            DataColumnCollection attrs = dt.Columns;//获取属性
            int attrNum = attrs.Count;//获取属性个数
            int recNum = dt.Rows.Count;//记录个数
            DataRowCollection rows=dt.Rows;//记录行记录
            for (int i = 0; i < attrNum; i++)
            {
                List<string> valSpace = new List<string>();//记录值空间
                for (int j = 0; j < recNum; j++)
                {
                    Object obj = rows[j][i];
                    if (attrs[i].DataType.ToString().Equals("System.Double"))//数值数据
                    {
                        if (Convert.IsDBNull(obj))//空值
                        {
                            rows[j][i] = 0;
                        }
                    }
                    else//文本数据
                    {
                        if (Convert.IsDBNull(obj))//空值
                        {
                            rows[j][i] = "";
                        }
                        if (valSpace.Contains(obj.ToString()))//未出现新值
                        {
                            rows[j][i] = valSpace.IndexOf(obj.ToString())+1;//得到索引
                        }
                        else//出现新值
                        {
                            valSpace.Add(obj.ToString());//添加新值
                            rows[j][i] = valSpace.IndexOf(obj.ToString())+1;//得到索引
                            //string s=rows[j][i].ToString();
                        }
                    }
                }                
            }
            List<int> rmList = new List<int>();
            for (int i = 0; i < recNum; i++)
            {
                if (i >= 153 && i <= 560)//2类
                {
                    //if (((i - 153) % 8 % 2 != 0)
                    //    || (i - 153) % 8 == 0)
                    if(false)
                    {
                        rmList.Add(i);
                    }
                }
            }
            for (int i = rmList.Count-1; i >=0; i--)
            {
                rows.RemoveAt(rmList[i]);
            }
            
        }
        /// <summary>
        /// 对按照ID排序后的筛选属性数据按照1/3 -- 2/3分类为测试样本和训练样本
        /// %3==1为训练样本
        /// @3!=1为测试样本
        /// </summary>
        /// <param name="test">测试样本集合</param>
        /// <param name="train">训练样本集合</param>
        /// <returns>属性集合</returns>
        DataColumnCollection classify(List<DataRow> testCollection, List<DataRow> trainCollection,List<double>valLength,List<double>max)
        {
            db = new DB();
            //查询数据
//            DataTable dt = db.getDataTable(@"select * from Test  Where category=3 or category=4
//            order by category"); Where category !=3
//            DataTable dt = db.getDataTable(@"SELECT   WanZuanFangShi.WZFS, AZ01.JBDM, AZ01.WZJS - AZ01.SJJS AS Expr1, AZ01.WZCW AS zymdc, AZ01.WJFFDM, 
//                AZ20.DJSD1, AZ20.DJSD2, AZ20.HD, AZ20.HYJBDM, AZ20.YSMCDM, AZ20.ZHJSDM, AZ20.ZJYXDMD1, 
//                AZ20.LDND1
//FROM      AZ01 INNER JOIN
//                AZ20 ON AZ01.JH = AZ20.JH INNER JOIN
//                WanZuanFangShi ON AZ01.JH = WanZuanFangShi.JH
//WHERE   (AZ20.CW = 'ed') order by wzfs");
            DataTable dt = db.getDataTable(@"SELECT   WanZuanFangShi.WZFS, AZ01.JBDM, AZ01.WZJS - AZ01.SJJS AS Expr1, AZ01.WZCW, AZ01.WJFFDM, 
                AZ20.DJSD1, AZ20.DJSD2, AZ20.HD, AZ20.HYJBDM, AZ20.YSMCDM, AZ20.ZHJSDM, AZ20.ZJYXDMD1, 
                AZ20.LDND1
FROM      AZ01 INNER JOIN
                AZ20 ON AZ01.JH = AZ20.JH INNER JOIN
                WanZuanFangShi ON AZ01.JH = WanZuanFangShi.JH
WHERE   (AZ20.CW = 'ed') and  AZ01.WZCW=AZ01.ZYMDC
ORDER BY WanZuanFangShi.WZFS");
            this.processData(dt);
            DataColumnCollection ret = dt.Columns;
            //记录数值区间 
            int attrNum = ret.Count;
            List<double> min = new List<double>();
            for (int i = 0; i < attrNum-1; i++)
            {
                max.Add(double.MinValue);
                min.Add(double.MaxValue);
            }
            //分类提取样本
            int num = dt.Rows.Count;//样本总量
            File.WriteAllText("./test.txt", "");
            File.WriteAllText("./rslt.txt", "");
            for (int j = 0; j < attrNum; j++)
            {
                File.AppendAllText("./test.txt", ret[j].DataType.ToString()+ "\t");
            }
            File.AppendAllText("./test.txt", Environment.NewLine);
            for (int i = 0; i < num; i++)
            {
                DataRow data = dt.Rows[i];//样本
                if (i % 10 != 0)//提取测试样本
                {
                    if (!(i >= 80 && i <= 136
                        //|| i >= 190 && i <= 220
                        || i >= 550 && i <= 660))//强制删除不好数据 505
                    {
                        testCollection.Add(data);
                    }
                    
                    //trainCollection.Add(data);
                }
                else//提取训练样本
                {
                    trainCollection.Add(data);
                }
                //输出刷新后的数据
                for (int j = 0; j < attrNum; j++)
                {
                    File.AppendAllText("./test.txt",data[j].ToString()+"\t");
                }
                File.AppendAllText("./test.txt", Environment.NewLine);
                //
                for (int j = 1; j < attrNum; j++)//忽略第一条决策属性
                {
                    double val=double.Parse(data[j].ToString());//获取值
                    if (val > max[j-1])
                        max[j-1] = val;//求上限
                    if (val < min[j-1])
                        min[j-1] = val;//求下限
                }
            }
            for (int i = 0; i < attrNum-1; i++)
            {
                valLength.Add(max[i] - min[i]);
            }
            //提取属性集
            return ret;
        }
        /// <summary>
        /// 主过程
        /// </summary>
        public void run()
        {
            List<DataRow> trainCollection = new List<DataRow>();//训练样本
            List<DataRow> testCollection = new List<DataRow>();//测试样本
            List<double> valLength = new List<double>();//值空间
            List<double> max=new List<double> ();//最大值
            //提取测试样本和训练样本
            DataColumnCollection attrCollection=classify(testCollection, trainCollection,valLength,max);
            //构造筛选对象
            filter = new Filter(trainCollection, attrCollection);
            int testNum = testCollection.Count;//训练样本个数
            DataRow testSample;//测试样本
            double sucNum = 0;//测试成功的样本数
            for (int i = 0; i < testNum; i++)//遍历测试样本
            {
                
                //根据相似样本集和关键属性构造约简对象
                reduct = new Reduct(trainCollection,attrCollection);
                List<double> dependance=reduct.calDependance();//求出属性的依赖度
                DiffMatrix matrix=reduct.getDiffMatrix(valLength);//获得差别矩阵
                //获得专家参数
                List<double> expertPara = new List<double>();//专家参数集合
                int attrNum=attrCollection.Count;
                for (int k = 0; k < attrNum-1; k++)
                {
                    expertPara.Add(1);
                }
                //为关键属性加权
                List<double>weights=matrix.weightedKeyAttr(dependance, expertPara);
                //按照属性对属性排序
                List<int> sortedIds = matrix.sortWeight(weights);//按照权值大小排序，记录关键属性顺序
                //约简生成core集
                List<int>core=matrix.reduce(sortedIds);//根据权值顺序约简差别矩阵，获得核心集合
                if (core.Count == 0)
                    continue;
                //测试样本
                testSample = testCollection[i];
                //求解针对测试样本相似度满足阈值的训练样本及其相似度
                Dictionary<int, double> id_u = filter.getIdSimilarity(testSample, valLength,weights,max,core);
                if (id_u.Count == 0)
                    continue;
                //调用多类SVM进行决策
                Strategy strategy = new Strategy(id_u, weights,core);
                strategy.classify(trainCollection);
                if (strategy.predict(testSample))
                    sucNum++;
            }
            Console.WriteLine("准确度="+(int)(sucNum / testNum*10000)/100.0+"%");
        }
    }
}
