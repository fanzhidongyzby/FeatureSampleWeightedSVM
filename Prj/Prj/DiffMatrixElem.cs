using System;
using System.Collections.Generic;
using System.Text;

//
namespace Prj
{
    class DiffMatrixElem
    {
        int i;//被比较样本在训练集的索引
        int j;//比较样本在训练集的索引
        List<int> diffAttrs;//差别属性在属性集合中索引的集合
        List<double> diffs;//差别程度
        /// <summary>
        /// 初始化差别矩阵元素
        /// </summary>
        /// <param name="i">被比较样本在训练集的索引</param>
        /// <param name="j">比较样本在训练集的索引</param>
        public DiffMatrixElem(int i, int j)
        {
            this.i = i;
            this.j = j;
            this.diffAttrs = new List<int>();
            this.diffs = new List<double>();
        }
        /// <summary>
        /// 添加一个差别属性索引
        /// </summary>
        /// <param name="attr">属性索引值</param>
        /// <param name="diff">属性区别程度</param>
        public void addDiffAttr(int attr,double diff)
        {
            diffAttrs.Add(attr);
            diffs.Add(diff);
        }
        /// <summary>
        /// 为一条差别记录加权
        /// </summary>
        /// <param name="depend">依赖度集合</param>
        /// <param name="expert">专家参数集合</param>
        /// <param name="weights">总权值，需要累加</param>
        public void weightedRec(List<double> depend, List<double> expert, List<double> weights)
        {
            int attrNum = diffAttrs.Count;//记录属性的个数
            int keyNum=depend.Count;//属性的总个数
            //临时记录每个属性的权值            
            List<double> weight = new List<double>();
            for (int k = 0; k < keyNum; k++)
            {
                weight.Add(0);
            }
            double weightSum=0;//所有权值的总和
            //遍历差别属性集合,求出权值
            for (int k = 0; k < attrNum; k++)
            {
                int key = diffAttrs[k];
                key--;//修正索引的偏移
                weight[key] = diffs[k] * depend[key] * expert[key];//权值=差别度*依赖度*专家参数
                weightSum += weight[key];//累积
            }
            //把权值累加在一起
            for (int k = 0; k < keyNum; k++)
            {
                double w=weight[k];
                if (w != 0)
                    weights[k] += w/weightSum;
            }
        }
        /// <summary>
        /// 判断某个属性是否在矩阵元素中
        /// </summary>
        /// <param name="id">属性的索引</param>
        /// <returns>是否包含</returns>
        public bool hasAttr(int id)
        {
            int attrNum = diffAttrs.Count;//包含属性个数
            int k;
            for (k = 0; k < attrNum; k++)
            {
                if (diffAttrs[k] == id)
                    break;
            }
            return (k != attrNum && diffs[k] > 0.9);//包含的属性差别度大于0.2
        }
        //public bool test()
        //{
        //    int num = diffAttrs.Count;
        //    for (int m = 0; m < num; m++)
        //    {
        //        if (diffs[m] > 0.4)
        //            return true;
        //    }
        //    return false;
        //}
        //public void write()
        //{
        //    if (diffAttrs.Contains(2))
        //        return;
        //    Console.Write(i + "," + j + ":");
        //    int num = diffAttrs.Count;
        //    for (int m = 0; m < num; m++)
        //    {
        //        Console.Write(diffAttrs[m]+"/");
        //    }
        //    Console.WriteLine();
        //}
    }
}
