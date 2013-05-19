using System;
using System.Collections.Generic;
using System.Text;

namespace Prj
{
    class DiffMatrix
    {
        List<DiffMatrixElem> diff;//记录差别矩阵的非空信息
        /// <summary>
        /// 构造差别矩阵
        /// </summary>
        public DiffMatrix()
        {
            diff = new List<DiffMatrixElem>();
        }
        /// <summary>
        /// 添加矩阵元素
        /// </summary>
        /// <param name="e">添加的元素</param>
        public void addElem(DiffMatrixElem e)
        {
            diff.Add(e);
        }
        /// <summary>
        /// 为关键属性加权
        /// </summary>
        /// <param name="depend">依赖度集合</param>
        /// <param name="expert">专家参数集合</param>
        /// <returns>关键属性权值集合</returns>
        public List<double> weightedKeyAttr(List<double> depend, List<double> expert)
        {
            List<double> weights = new List<double>();
            int keyNum = depend.Count;//关键属性个数
            for (int i = 0; i < keyNum; i++)
            {
                weights.Add(0);
            }
            int elemNum = diff.Count;//矩阵元素个数
            for (int i = 0; i < elemNum; i++)
            {
                diff[i].weightedRec(depend, expert, weights);
            }
            //权值归一化
            double maxWeight = 0;
            for (int i = 0; i < keyNum; i++)
            {
                if (maxWeight < weights[i])
                    maxWeight = weights[i];
            }
            for (int i = 0; i < keyNum; i++)
            {
                weights[i] /= maxWeight;
            }
            return weights;
        }
        /// <summary>
        /// 按照属性权值排序
        /// </summary>
        /// <param name="ws">属性权值集合</param>
        /// <returns>排序后的属性顺序</returns>
        public List<int> sortWeight(List<double> ws)
        {
            //复制权值集合
            List<double> weights = new List<double>();
            int keyNum = ws.Count;//关键属性个数
            List<int> weightSortKeys = new List<int>();//排序后的属性顺序
            for (int i = 0; i < keyNum; i++)
            {
                weightSortKeys.Add(i + 1);
                weights.Add(ws[i]);
            }
            //选择排序            
            for (int i = 0; i < keyNum - 1; i++)
            {
                int k = i;
                int tmpId;
                double tmpWeight;
                for (int j = i + 1; j < keyNum; j++)
                {
                    if (weights[j] > weights[k])
                        k = j;
                }
                if (k != i)//找到更大的，交换
                {
                    //排列权值
                    tmpWeight = weights[i];
                    weights[i] = weights[k];
                    weights[k] = tmpWeight;
                    //排列索引
                    tmpId = weightSortKeys[i];
                    weightSortKeys[i] = weightSortKeys[k];
                    weightSortKeys[k] = tmpId;
                }
            }
            //for (int i = 0; i < keyNum; i++)
            //{
            //    Console.WriteLine(weights[i] + " -- " + weightSortKeys[i]);
            //}
            return weightSortKeys;
        }
        /// <summary>
        /// 约简关键属性
        /// </summary>
        /// <param name="weights">加权后的属性权值</param>
        /// <returns>core集合</returns>
        public List<int> reduce(List<int> sortedIds)
        {
            List<int> core = new List<int>();//核心集
            int keyNum = sortedIds.Count;//关键属性个数
            for (int i = 0; i < keyNum; i++)
            {
                List<DiffMatrixElem> tmpDiff=new List<DiffMatrixElem>();
                int elemNum=diff.Count;//元素个数
                for (int j = 0; j < elemNum; j++)
                {
                    if (!diff[j].hasAttr(sortedIds[i]))//约简
                    {
                        tmpDiff.Add(diff[j]);
                    }
                }
                diff.Clear();
                diff = tmpDiff;
                if (diff.Count < elemNum)//约简成功
                {
                    core.Add(sortedIds[i]);//添加core集
                }
                //Console.WriteLine("剩余元素个数" + elemNum);
            }
            //for (int x = 0; x < core.Count; x++)
            //{
            //    Console.Write(core[x]+" ");
            //}
            return core;
            
            //return sortedIds;//消除约简效果的影响，测试使用
        }
        //public void test()
        //{
        //    int sum = 0;
        //    for (int i = 0; i < diff.Count; i++)
        //    {
        //        if (!diff[i].test())
        //        {
        //            sum++;
        //            //diff[i].write();
        //        }
        //    }
        //    Console.WriteLine(sum);
        //}
        //public void write()
        //{
        //    for (int i = 0; i < diff.Count; i++)
        //    {
        //        diff[i].write();
        //        //System.Threading.Thread.Sleep(100);
        //    }
        //}
    }
}
