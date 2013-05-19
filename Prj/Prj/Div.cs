using System;
using System.Collections.Generic;
using System.Text;

namespace Prj
{
    class Div
    {
        string val;//记录数据
        List<int> recIds;//记录相同值的记录集合
        /// <summary>
        /// 构造划分对象
        /// </summary>
        /// <param name="val">构造划分根据的值</param>
        public Div(string val)
        {
            this.val = val;
            recIds = new List<int>();
        }
        /// <summary>
        /// 获取划分的集合元素个数
        /// </summary>
        /// <returns>元素个数</returns>
        public int getRecNum()
        {
            return recIds.Count;
        }
        /// <summary>
        /// 获取构造划分的根据的值
        /// </summary>
        /// <returns>依据值</returns>
        public string getVal()
        {
            return val;
        }
        /// <summary>
        /// 跟划分添加一个记录
        /// </summary>
        /// <param name="id">记录索引</param>
        public void addRec(int id)
        {
            recIds.Add(id);
        }
        /// <summary>
        /// 划分是否包含指定的记录
        /// </summary>
        /// <param name="id">指定记录索引</param>
        /// <returns>是否包含索引</returns>
        bool hasId(int id)
        {
            int num = recIds.Count;//记录集合数量
            for (int i = 0; i < num; i++)
            {
                if (id == recIds[i])//找到元素
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 判断划分是否包含另一个划分
        /// </summary>
        /// <param name="d">另一个划分</param>
        /// <returns>是否包含划分</returns>
        public bool containDiv(Div d)
        {
            List<int> dRecIds = d.recIds;
            int num = dRecIds.Count;//被包含集合的个数
            if (num > recIds.Count)//被包含的集合数量大于包含集合的个数，肯定不包含
                return false;
            for (int i = 0; i < num; i++)//遍历被包含的集合
            {
                if (!hasId(dRecIds[i]))//有一个元素不在包含的集合中，肯定不包含
                    return false;
            }
            return true;
        }
    }
}
