using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProjectGDAL
{
    public class Data
    {
        private MyPoint k;      //координаты на преобразованном изображении
        private MyPoint t;      //трансформированные координаты из исходного изображения в преобразованное
        
        public Data(MyPoint k, MyPoint t)
        {
            this.k = k;
            this.t = t;
        }

        public MyPoint getK()
        {
            return k;
        }

        public MyPoint getT()
        {
            return t;
        }
    }
}
