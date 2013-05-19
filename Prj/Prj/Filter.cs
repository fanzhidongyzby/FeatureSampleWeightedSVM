using System;
using System.Collections.Generic;
using System.Text;
//
using System.Data;
using System.IO;
namespace Prj
{
    class Filter
    {
        List<DataRow> trainCollection;//训练集
        DataColumnCollection filterCollection;//记录筛选属性
        /// <summary>
        /// 根据已有的数据库对象查询筛选属性的数据
        /// </summary>
        /// <param name="db">已有的数据库对象</param>
        public Filter(List<DataRow> trainCollection, DataColumnCollection filterCollection)
        {
            //记录训练集
            this.trainCollection = trainCollection;
            //记录筛选属性
            this.filterCollection = filterCollection;
        }
        /// <summary>
        /// 计算两条样本之间的总差别度，针对实际数据特性，全部按照文本数据处理。浮点值需要考虑误差精度
        /// </summary>
        /// <param name="testSample">测试样本</param>
        /// <param name="trainSample">训练样本</param>
        /// <returns>两条样本之间的总差别度</returns>
        double getDiffs(DataRow testSample, DataRow trainSample, List<double> valLength,List<double>weights,List<double>max,List<int>core)
        {
            double diffs = 0;
            //遍历筛选属性集
//            int attrNum = filterCollection.Count;//属性个数
            int attrNum = core.Count;//约简后属性个数
            DataColumn filterAttr;//属性
            for (int i = 0; i < attrNum; i++)
            {
                int k = core[i];
                //筛选属性
                filterAttr = filterCollection[k];
                if (filterAttr.DataType.ToString().Equals("System.Single")
                    || filterAttr.DataType.ToString().Equals("System.Double")
                    || filterAttr.DataType.ToString().Equals("System.Int32"))
                {
                    //提取浮点数据
                    double a = Convert.ToDouble(testSample[k]);
                    double x = Convert.ToDouble(trainSample[k]);
                    //计算差别程度
                    //当d线性同比放大不对结果有影响 当d平方放大 对区分分离性较大的类效果很好
                    //对于混杂的两类 对d开方 效果很好
                    double d = 1;
                    if (valLength[k - 1] != 0)
                    {
                        d = Math.Abs(x - a) / valLength[k - 1] * weights[k - 1];
                        //d = Math.Sqrt(d);
                        //d *= d;
                    }
                        //d = Math.Abs(x - a) / max[k - 1] * weights[k - 1];
                        //d = Math.Abs(x - a) / max[k - 1] / valLength[k - 1] * weights[k - 1];
                    diffs += d;
                }
                else
                {
                    //文本数据相似度二元化
                    if (testSample[filterAttr].ToString().Equals(trainSample[filterAttr].ToString()))
                        diffs+=weights[k-1];
                }
            }
            return diffs;
        }
        /// <summary>
        /// 求解一个测试样本和训练样本之间的相似度
        /// </summary>
        /// <param name="testSample">测试样本</param>
        /// <param name="trainCollection">训练样本集合</param>
        /// <returns>样本相似度集合</returns>
        public Dictionary<int,double> getIdSimilarity(DataRow testSample,List<double>valLength,List<double>weights,List<double>max,List<int>core)
        {
            int trainNum = trainCollection.Count;//训练样本个数
            DataRow trainSample;//训练样本
            Dictionary<int,double> id_u = new Dictionary<int,double> ();//相似度集合
            List<double> allDiffs = new List<double> ();//差别度
            double maxDiffs = 0;//最大差别度
            int[] n = new int[3];
            //计算所有的差别集合
            for (int i = 0; i < trainNum; i++)
            {
                trainSample = trainCollection[i];
                double diffs = getDiffs(testSample, trainSample,valLength,weights,max,core);//求解相似度
                allDiffs.Add(diffs);
                if (maxDiffs < diffs)
                    maxDiffs = diffs;
            }
            for (int i = 0; i < trainNum; i++)
            {
                double similar=1-allDiffs[i]/ maxDiffs;
                if (similar >=0.0)
                {
                    id_u.Add(i, similar);
                    for (int j = 0; j < n.Length; j++)
                    {
                        if (trainCollection[i][0].ToString().Equals((j + 1).ToString()))
                        {
                            n[j]++;
                        }
                    }
                }
            }
            for (int i = 0; i < n.Length; i++)
            {
                //Console.Write(n[i] + " ");
                File.AppendAllText("./rslt.txt", n[i] + " ");
            }
            return id_u;
        }
    }
}
