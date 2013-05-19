using System;
using System.Collections.Generic;
using System.Text;
//
namespace Prj
{
    class Depend
    {
        List<Div>divs;//划分集合的集合

        bool isText;//是否记录的文本数据[针对实际数据的特性，浮点数按照文本进行划分]isText===true
        /// <summary>
        /// 构造依赖对象
        /// </summary>
        /// <param name="text">分类属性的类型</param>
        public Depend(bool text)
        {
            divs = new List<Div>();
            isText=text;
        }
        /// <summary>
        /// 添加一条数据记录
        /// </summary>
        /// <param name="s">数据值</param>
        /// <param name="id">数据记录索引</param>
        public void addRecord(string s,int id)
        {
            int i;//记录划分的索引
            Div div=new Div("");//记录划分
            int num = divs.Count;
            for (i=0; i < num; i++)
            {
               div=divs[i];
               if (isText)//文本数据
               {
                   if (div.getVal().Equals(s))//找到要添加的划分
                   {
                       break;
                   }
               }
               else
               {
                   double val = Convert.ToDouble(div.getVal());
                   double addval = Convert.ToDouble(s);
                   if (Math.Abs(Math.Abs(addval - val) / val) < 0.01)//误差范围内
                   {
                       break;
                   }
               }
            }
            if (i == num)//没有找到相应的划分
            {
                //创建新的划分
                Div d = new Div(s);
                divs.Add(d);
                //添加新的记录
                d.addRec(id);
            }
            else
            {
                //添加新的记录
                div.addRec(id);
            }
        }
        /// <summary>
        /// 获取关系依赖度
        /// </summary>
        /// <param name="dec">决策划分依赖对象</param>
        /// <param name="allNum">样本总数</param>
        /// <returns>依赖度</returns>
        public double getDependance(Depend dec,int allNum)
        {
            double sum=0;//正域的数量
            int divNum = divs.Count;//当前划分的个数
            List<Div> decDivs=dec.divs;//决策属性的划分
            int decDivNum = decDivs.Count;//决策划分的个数

            for (int i = 0; i < divNum; i++)//遍历划分集合
            {
                Div keyDiv = divs[i];
                for (int j = 0; j < decDivNum; j++)
                {
                    Div decDiv = decDivs[j];
                    if (decDiv.containDiv(keyDiv))
                    {
                        sum += keyDiv.getRecNum();
                        break;
                    }
                }
            }
            return sum / allNum;
        }
    }
}
