using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using SVM;
namespace Prj
{
    class Svm
    {
        Model model;//训练模型
        public Svm(Node[][] _X,double[]_Y)
        {
            //构造训练集
            Problem problem = new Problem(_X.Length, _Y.ToArray(), _X.ToArray(),_X[0].Length);
            //RangeTransform range = RangeTransform.Compute(problem);
            //problem = range.Scale(problem);
            //初始化参数
            Parameter param = new Parameter();
            param.C = 1;
            param.Gamma =2;
            //param.Nu = 0.08;
            //param.SvmType = SvmType.ONE_CLASS;//C_SVC
            //构造分类模型
            model = Training.Train(problem, param);
        }
        public double predict(Node[] test)
        {
            double preRes=Prediction.Predict(model, test);
            return preRes;
        }
    }
}
