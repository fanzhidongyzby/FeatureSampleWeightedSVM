using System;
using System.Collections.Generic;
using System.Text;
//
using System.Data.SqlClient;
using System.Data;
namespace Prj
{
    class Reduct
    {
        List<DataRow> trainCollection;//训练集
        DataColumnCollection keyCollection;//关键属性集
        public Reduct(List<DataRow> trainCollection, DataColumnCollection attrCollection)
        {
            this.trainCollection = trainCollection;
            keyCollection = attrCollection;
        }
        /// <summary>
        /// 获取差别矩阵
        /// </summary>
        public DiffMatrix getDiffMatrix(List<double>valLength)
        {
            DiffMatrix matrix=new DiffMatrix();//差别矩阵
            int sampleNum = trainCollection.Count;//相似样本个数
            //组合训练样本
            for (int i = 0; i < sampleNum-1; i++)
            {
                DataRow first = trainCollection[i];
                for (int j = i + 1; j < sampleNum; j++)
                {
                    DataRow second = trainCollection[j];
                    //计算差别
                    if (!first[0].ToString().Equals(second[0].ToString()))//决策属性不同
                    {
                        //产生差别矩阵元素
                        DiffMatrixElem elem = new DiffMatrixElem(i, j);
                        int keyNum = keyCollection.Count;
                        for (int k = 1; k < keyNum; k++)//第0号属性是决策属性，不是关键属性
                        {
                            //关键属性
                            DataColumn keyAttr = keyCollection[k];
                            if (keyAttr.DataType.ToString().Equals("System.Single")
                                || keyAttr.DataType.ToString().Equals("System.Double")
                                || keyAttr.DataType.ToString().Equals("System.Int32"))
                            {
                                //提取浮点数据
                                double a = Convert.ToDouble(first[k]);
                                double x = Convert.ToDouble(second[k]);
                                //计算差别程度
                                //double diff = Math.Abs(Math.Abs(x - a) / a);
                                double diff = 0;
                                if (valLength[k-1] != 0)
                                    diff = Math.Abs(x - a) / valLength[k - 1];
                                if(diff>0)//有区别,没有约简，置为0
                                    elem.addDiffAttr(k, diff);//添加差别属性
                            }
                            else
                            {
                                //文本二元化
                                if (!first[k].ToString().Equals(second[k].ToString()))
                                {
                                    elem.addDiffAttr(k, 1);//添加差别属性
                                }
                            }
                        }
                        //添加差别元素
                        matrix.addElem(elem);
                    }
                }
            }
            //matrix.write();
            return matrix;
        }
        /// <summary>
        /// 计算关键属性的依赖度
        /// </summary>
        /// <returns>依赖度集合1-13存储在0-12</returns>
        public List<double> calDependance()
        {
            List<double> keyDependance=new List<double>();//关键属性的依赖度
            int sampleNum = trainCollection.Count;//相似样本个数
            //计算决策属性的划分
            Depend dec = new Depend(true);
            for (int j = 0; j < sampleNum; j++)//遍历相似样本，进行属性划分
            {
                dec.addRecord(trainCollection[j][0].ToString(), j);
            }
            int keyNum=keyCollection.Count;
            for (int i = 1; i < keyNum; i++)//遍历属性求出相应的依赖度
            {
                //计算关键属性的划分
                Depend depend = new Depend(true);//不考虑属性类型，一律按照文本处理
                for (int j = 0; j < sampleNum; j++)//遍历样本，进行属性划分
                {
                    depend.addRecord(trainCollection[j][i].ToString(), j);
                }
                //求出依赖度
                keyDependance.Add(depend.getDependance(dec, sampleNum));
            }
            //for (int i = 0; i < keyDependance.Count; i++)
            //{
            //    Console.Write(keyDependance[i].ToString() + " ");
            //}
            return keyDependance;
        }
    }
}
